using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nng;
using QRCoder;
using WeChatFerry.WinForm.Helper;
using WeChatFerry.WinForm.HttpApi.Dtos;
using WeChatFerry.WinForm.WeChatFerry;
using static System.Windows.Forms.LinkLabel;

namespace WeChatFerry.WinForm.HttpApi
{
    /// <inheritdoc/>
    [ApiController]
    [Route("[controller]/[action]")]
    public class WcfController:ControllerBase
    {
        private readonly ILogger<WcfController> _logger = NLogHelper.CreateLogger<WcfController>();

        /// <summary>
        /// 获取登录二维码，已经登录则返回空字符串
        /// <para>好像无法获取正常的登录链接</para>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetQrCodeAsync()
        {
            var data = await GlobalValue.WcfClient.GetQrCode();

            var qrCodeImg = CreateQrCodeImg(data);

            using var ms = new MemoryStream();
            qrCodeImg.Save(ms, ImageFormat.Jpeg);

            return await Task.FromResult<FileResult>(File(ms.ToArray(), "image/jpg"));

            Bitmap CreateQrCodeImg(string strVal)
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(strVal, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var image = qrCode.GetGraphic(90, System.Drawing.Color.Black, System.Drawing.Color.White, icon: null, 20, 0);
                return image;
            }
        }

        /// <summary>
        /// 是否已登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> IsLoginAsync()
        {
            var data=await GlobalValue.WcfClient.IsLogin();
            return new UnifiedResponse<string>(HttpStatusCode.OK, data: data.ToString());
        }

        /// <summary>
        /// 获取登录账户的 wxid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSelfWxidAsync()
        {
            var data = await GlobalValue.WcfClient.GetSelfWxid();
            return new UnifiedResponse<string>(HttpStatusCode.OK, data: data);
        }

        /// <summary>
        /// 获取所有消息类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMsgTypes()
        {
            var data = await GlobalValue.WcfClient.GetMsgTypes();
            return new UnifiedResponse<Dictionary<int, string>>(HttpStatusCode.OK, data: data);
        }

        /// <summary>
        /// 获取完整通讯录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            var data = await GlobalValue.WcfClient.GetContacts();
            return new UnifiedResponse<List<RpcContact>>(HttpStatusCode.OK, data: data);
        }

        /// <summary>
        /// 获取所有数据库
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDbNames()
        {
            var data=await GlobalValue.WcfClient.GetDbNames();
            return new UnifiedResponse<List<string>>(HttpStatusCode.OK, data: data);
        }

        /// <summary>
        /// 获取 db 中所有表
        /// </summary>
        /// <param name="input">>数据库名（可通过 `GetDbNames` 查询）</param>
        /// <returns>`db` 下的所有表名及对应建表语句</returns>
        [HttpPost]
        public async Task<IActionResult> GetDbTables(OneParamInputDto<string> input)
        {
            var data = await GlobalValue.WcfClient.GetDbTables(input.Param);
            var list = data.Select(item => new GetDbTablesItemOutputDto { Name = item.Item1, Sql = item.Item2 }).ToList();
            return new UnifiedResponse<List<GetDbTablesItemOutputDto>>(HttpStatusCode.OK, data: list);
        }

        /// <summary>
        /// 获取登录账号个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            var data=await GlobalValue.WcfClient.GetUserInfo();
            return new UnifiedResponse<UserInfo>(HttpStatusCode.OK, data: data);
        }

        /// <summary>
        /// 获取语音消息并转成 MP3
        /// </summary>
        /// <param name="id">语音消息 id</param>
        /// <param name="timeOut">超时时间（秒）</param>
        /// <returns>成功返回音频，否则返回错误信息</returns>
        [HttpGet]
        public async Task<IActionResult> GetAudioMsg(ulong id, uint timeOut = 3)
        {
            return await GetAudioMsg(new TwoParamInputDto<ulong, uint>() { Param1 = id, Param2 = timeOut });
        }

        /// <summary>
        /// 获取语音消息并转成 MP3
        /// </summary>
        /// <param name="input">参考 GET 请求，param1: id, param2: timeOut</param>
        /// <returns>成功返回音频，否则返回错误信息</returns>
        [HttpPost]
        public async Task<IActionResult> GetAudioMsg(TwoParamInputDto<ulong,uint> input)
        {
            
            var savePath = FileHelper.GetTempFileDataPath();
            var data = await GlobalValue.WcfClient.GetAudioMsg(input.Param1, savePath, input.Param2);
            if (string.IsNullOrWhiteSpace(data) || !System.IO.File.Exists(data))
            {
                return new UnifiedResponse<object>(HttpStatusCode.NotFound, "获取语音消息失败");
            }

            await using var stream = System.IO.File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
           
            return await Task.FromResult<FileResult>(File(stream, "audio/mpeg"));

        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="msg">要发送的消息，换行使用 `\n` （单杠）；如果 @ 人的话，需要带上跟 `aters` 里数量相同的 @</param>
        /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
        /// <param name="aters">要 @ 的 wxid，多个用逗号分隔；`@所有人` 只需要 `notify@all`</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SendText(string msg, string receiver, string aters = "")
        {
            return await SendText(new TextMsg { Msg = msg, Receiver = receiver, Aters = aters });
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendText(TextMsg msg)
        {
            var data = await GlobalValue.WcfClient.SendText(msg.Msg, msg.Receiver, msg.Aters);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="path">图片网络路径</param>
        /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SendImage(string path, string receiver)
        {
            return await SendImage(new PathMsg { Path = path, Receiver = receiver });
        }

        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendImage(PathMsg msg)
        {
            var data = await GlobalValue.WcfClient.SendImage(msg.Path,msg.Receiver);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="path">文件网络路径</param>
        /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SendFile(string path, string receiver)
        {
            return await SendFile(new PathMsg { Path = path, Receiver = receiver });
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendFile(PathMsg msg)
        {
            var data = await GlobalValue.WcfClient.SendFile(msg.Path, msg.Receiver);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        /// <summary>
        /// 发送表情
        /// </summary>
        /// <param name="path">表情图网络路径</param>
        /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SendEmotion(string path, string receiver)
        {
            return await SendEmotion(new PathMsg { Path = path, Receiver = receiver });
        }

        /// <summary>
        /// 发送表情
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendEmotion(PathMsg msg)
        {
            var data = await GlobalValue.WcfClient.SendEmotion(msg.Path, msg.Receiver);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        /// <summary>
        /// 发送 XML [未实现]
        /// </summary>
        /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
        /// <param name="xml">xml 内容</param>
        /// <param name="type">xml 类型，如：0x21 为小程序</param>
        /// <param name="path">封面图片路径</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SendXml(string receiver, string xml, int type, string? path = null)
        {
            return await SendXml(new XmlMsg { Receiver = receiver, Content = xml, Type = type, Path = path });
        }

        /// <summary>
        /// 发送 XML [未实现]
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendXml(XmlMsg msg)
        {
            var data = await GlobalValue.WcfClient.SendXml(msg.Receiver,msg.Content,msg.Type,msg.Path);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        /// <summary>
        /// 发送富文本消息
        /// </summary>
        /// <param name="name">左下显示的名字</param>
        /// <param name="account">填公众号 id 可以显示对应的头像（gh_ 开头的）</param>
        /// <param name="title">标题，最多两行</param>
        /// <param name="digest">摘要，三行</param>
        /// <param name="url">点击后跳转的链接</param>
        /// <param name="thumbUrl">缩略图的链接</param>
        /// <param name="receiver">接收人, wxid 或者 roomid</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SendRichText(string? name, string? account, string title, string digest, string url,
            string thumbUrl, string receiver)
        {
            return await SendRichText(new RichText { Name = name, Account = account, Title = title, Digest = digest, Url = url, Thumburl = thumbUrl, Receiver = receiver });
        }

        /// <summary>
        /// 发送富文本消息
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendRichText(RichText msg)
        {
            var data = await GlobalValue.WcfClient.SendRichText(msg.Name,msg.Account,msg.Title,msg.Digest,msg.Url,msg.Thumburl,msg.Receiver);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        /// <summary>
        /// 拍一拍群友
        /// </summary>
        /// <param name="roomid">群 id</param>
        /// <param name="wxid">要拍的群友的 wxid</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SendPatMsg(string roomid, string wxid)
        {
            return await SendPatMsg(new PatMsg { Roomid = roomid, Wxid = wxid });
        }

        /// <summary>
        /// 拍一拍群友
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendPatMsg(PatMsg msg)
        {
            var data = await GlobalValue.WcfClient.SendPatMsg(msg.Roomid, msg.Wxid);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        /// <summary>
        /// 转发消息。可以转发文本、图片、表情、甚至各种 XML；语音也行，不过效果嘛，自己验证吧。
        /// </summary>
        /// <param name="id">待转发消息的 id</param>
        /// <param name="receiver">消息接收者，wxid 或者 roomid</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ForwardMsg(ulong id, string receiver)
        {
            return await ForwardMsg(new ForwardMsg { Id = id, Receiver = receiver });
        }

        /// <summary>
        /// 转发消息。可以转发文本、图片、表情、甚至各种 XML；语音也行，不过效果嘛，自己验证吧。
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ForwardMsg(ForwardMsg msg)
        {
            var data = await GlobalValue.WcfClient.ForwardMsg(msg.Id, msg.Receiver);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        /// <summary>
        /// 执行 SQL，如果数据量大注意分页，以免 BOOM
        /// </summary>
        /// <param name="db">要查询的数据库</param>
        /// <param name="sql">要执行的 SQL</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> QuerySql(string db, string sql)
        {
            return await QuerySql(new DbQuery { Db = db, Sql = sql });
        }
        
        /// <summary>
        /// 执行 SQL，如果数据量大注意分页，以免 BOOM
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> QuerySql(DbQuery msg)
        {
            var data = await GlobalValue.WcfClient.QuerySql(msg.Db, msg.Sql);
            return new UnifiedResponse<List<Dictionary<string, object?>>>(HttpStatusCode.OK, data: data);
        }

        /// <summary>
        /// 通过好友申请
        /// </summary>
        /// <param name="v3">加密用户名 (好友申请消息里 v3 开头的字符串)</param>
        /// <param name="v4">Ticket (好友申请消息里 v4 开头的字符串)</param>
        /// <param name="scene">申请方式 (好友申请消息里的 scene); 为了兼容旧接口，默认为扫码添加 (30)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AcceptNewFriend(string v3, string v4, int scene = 30)
        {
            return await AcceptNewFriend(new Verification { V3 = v3, V4 = v4, Scene = scene });
        }

        /// <summary>
        /// 通过好友申请
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AcceptNewFriend(Verification msg)
        {
            var data = await GlobalValue.WcfClient.AcceptNewFriend(msg.V3, msg.V4, msg.Scene);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "通过好友申请失败");
        }

        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFriends()
        {
            var data=await GlobalValue.WcfClient.GetFriends();
            return new UnifiedResponse<List<RpcContact>>(HttpStatusCode.OK, data: data);
        }

        /// <summary>
        /// 接收转账
        /// </summary>
        /// <param name="wxid">转账消息里的发送人 wxid</param>
        /// <param name="transFerId">转账消息里的 transferid</param>
        /// <param name="transActionId">转账消息里的 transactionid</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReceiveTransfer(string wxid,string transFerId, string transActionId)
        {
            return await ReceiveTransfer(new Transfer() { Wxid = wxid, Tfid = transFerId, Taid = transActionId });
        }

        /// <summary>
        /// 接收转账
        /// </summary>
        /// <param name="msg">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReceiveTransfer(Transfer msg)
        {
            var data = await GlobalValue.WcfClient.ReceiveTransfer(msg.Wxid, msg.Tfid, msg.Taid);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "接收转账失败");
        }

        /// <summary>
        /// 刷新朋友圈
        /// </summary>
        /// <param name="id">开始 id，0 为最新页</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RefreshPyq(ulong id)
        {
            return await RefreshPyq(new OneParamInputDto<ulong>(){Param = id});
        }

        /// <summary>
        /// 刷新朋友圈
        /// </summary>
        /// <param name="input">参考 GET 请求 param: id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RefreshPyq(OneParamInputDto<ulong> input)
        {
            var data = await GlobalValue.WcfClient.RefreshPyq(input.Param);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "刷新朋友圈失败");
        }

        /// <summary>
        /// 通过 wxid 查询微信号昵称等信息
        /// </summary>
        /// <param name="wxid">联系人 wxid</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetInfoByWxid(string wxid)
        {
            return await GetInfoByWxid(new OneParamInputDto<string>(){Param = wxid});
        }

        /// <summary>
        /// 通过 wxid 查询微信号昵称等信息
        /// </summary>
        /// <param name="input">参考 GET 请求 param: wxid</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetInfoByWxid(OneParamInputDto<string> input)
        {
            var data = await GlobalValue.WcfClient.GetInfoByWxid(input.Param);
            if (data == null)
            {
                return new UnifiedResponse<RpcContact>(HttpStatusCode.NotFound, $"未能获取到 {input.Param} 的信息");
            }
            return new UnifiedResponse<RpcContact>(HttpStatusCode.OK, data: data);
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="id">待撤回消息的 id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RevokeMsg(ulong id)
        {
            return await RevokeMsg(new OneParamInputDto<ulong>() { Param = id });
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="input">参考 GET 请求 param: id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RevokeMsg(OneParamInputDto<ulong> input)
        {
            var data = await GlobalValue.WcfClient.RevokeMsg(input.Param);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "撤回消息失败");
        }

        /// <summary>
        /// 获取 OCR 结果。鸡肋，需要图片能自动下载；通过下载接口下载的图片无法识别。
        /// </summary>
        /// <param name="extra">待识别的图片路径，消息里的 extra</param>
        /// <param name="timeout">超时时间（秒）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOcrResult(string extra,int timeout)
        {
            return await GetOcrResult(new TwoParamInputDto<string, int>() { Param1 = extra, Param2 = timeout });
        }

        /// <summary>
        /// 获取 OCR 结果。鸡肋，需要图片能自动下载；通过下载接口下载的图片无法识别。
        /// </summary>
        /// <param name="input">参考 GET 请求 param1: extra, param2: timeout</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetOcrResult(TwoParamInputDto<string, int> input)
        {
            var data = await GlobalValue.WcfClient.GetOcrResult(input.Param1, input.Param2);
            return new UnifiedResponse<string>(HttpStatusCode.OK, data: data);
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="id">消息中 id</param>
        /// <param name="extra">消息中的 extra</param>
        /// <param name="timeout">超时时间（秒）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DownloadImage(ulong id,string extra,int timeout)
        {
            return await DownloadImage(new DownloadImageInputDto() { Id = id, Extra = extra, Timeout = timeout });
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="input">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DownloadImage(DownloadImageInputDto input)
        {
            var dir = FileHelper.GetTempFileDataPath();
            var data = await GlobalValue.WcfClient.DownloadImage(input.Id,input.Extra,dir,input.Timeout);
            if (string.IsNullOrWhiteSpace(data)||!System.IO.File.Exists(data))
            {
                return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "下载图片失败");
            }
            _logger.LogInformation($"解析后图片地址{data}");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(data);
            return await Task.FromResult<FileResult>(File(fileBytes, "image/jpg"));
            
        }

        /// <summary>
        /// 添加群成员
        /// </summary>
        /// <param name="roomid">待加群的 id</param>
        /// <param name="wxids">要加到群里的 wxid，多个用逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddChatRoomMembers(string roomid, string wxids)
        {
            return await AddChatRoomMembers(new MemberMgmt { Roomid = roomid, Wxids = wxids });
        }

        /// <summary>
        /// 添加群成员
        /// </summary>
        /// <param name="input">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddChatRoomMembers(MemberMgmt input)
        {
            var data = await GlobalValue.WcfClient.AddChatRoomMembers(input.Roomid,input.Wxids);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "添加群成员失败");
        }

        /// <summary>
        /// 删除群成员
        /// </summary>
        /// <param name="roomid">待加群的 id</param>
        /// <param name="wxids">要删除成员的 wxid，多个用逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DelChatRoomMembers(string roomid, string wxids)
        {
            return await DelChatRoomMembers(new MemberMgmt { Roomid = roomid, Wxids = wxids });
        }

        /// <summary>
        /// 删除群成员
        /// </summary>
        /// <param name="input">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DelChatRoomMembers(MemberMgmt input)
        {
            var data = await GlobalValue.WcfClient.DelChatRoomMembers(input.Roomid, input.Wxids);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "删除群成员失败");
        }

        /// <summary>
        /// 邀请群成员
        /// </summary>
        /// <param name="roomid">待加群的 id</param>
        /// <param name="wxids">要邀请成员的 wxid，多个用逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InviteChatRoomMembers(string roomid, string wxids)
        {
            return await InviteChatRoomMembers(new MemberMgmt { Roomid = roomid, Wxids = wxids });
        }

        /// <summary>
        /// 邀请群成员
        /// </summary>
        /// <param name="input">参考 GET 请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> InviteChatRoomMembers(MemberMgmt input)
        {
            var data = await GlobalValue.WcfClient.InviteChatRoomMembers(input.Roomid, input.Wxids);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "邀请群成员失败");
        }

        /// <summary>
        /// 获取群成员
        /// </summary>
        /// <param name="roomid">群的 id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetChatRoomMembers(string roomid)
        {
            return await GetChatRoomMembers(new OneParamInputDto<string>() { Param = roomid });
        }

        /// <summary>
        /// 获取群成员
        /// </summary>
        /// <param name="input">参考 GET 请求 param: roomid</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetChatRoomMembers(OneParamInputDto<string> input)
        {
            var data = await GlobalValue.WcfClient.GetChatRoomMembers(input.Param);
            return new UnifiedResponse<Dictionary<string,string>>(HttpStatusCode.OK, data:data);
        }

        /// <summary>
        /// 获取群名片
        /// </summary>
        /// <param name="wxid">wxid</param>
        /// <param name="roomid">群的 id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAliasInChatRoom(string wxid, string roomid)
        {
            return await GetAliasInChatRoom(new TwoParamInputDto<string, string>() { Param1 = wxid, Param2 = roomid });
        }
        /// <summary>
        /// 获取群名片
        /// </summary>
        /// <param name="input">参考 GET 请求 param1: wxid, param2: roomid</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<IActionResult> GetAliasInChatRoom(TwoParamInputDto<string,string> input)
        {
            if (string.IsNullOrWhiteSpace(input.Param2))
            {
                throw new Exception("param2 is required, must be not null");
            }
            var data = await GlobalValue.WcfClient.GetAliasInChatRoom(input.Param1,input.Param2);
            return new UnifiedResponse<string>(HttpStatusCode.OK, data: data);
        }
        /// <summary>
        /// 根据 wxid 获取头像
        /// </summary>
        /// <param name="wxid">wxid</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetContactHeadImgByWxid(string wxid)
        {
            return await GetContactHeadImgByWxid(new OneParamInputDto<string>() { Param = wxid });
        }

        /// <summary>
        /// 根据 wxid 获取头像
        /// </summary>
        /// <param name="input">参考 GET 请求 param: wxid</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetContactHeadImgByWxid(OneParamInputDto<string> input)
        {
            var data = await GlobalValue.WcfClient.GetContactHeadImgByWxid(input.Param);
            if (string.IsNullOrWhiteSpace(data))
            {
                return new UnifiedResponse<string>(HttpStatusCode.NotFound, $"未能获取到 {input.Param} 的头像");
            }

            var img = ConvertFromBase64ToImage(data);
            return await Task.FromResult<FileResult>(File(img, "image/jpg"));


            MemoryStream ConvertFromBase64ToImage(string base64String)
            {
                // 将base64字符串转换为字节数组
                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                // 使用Bitmap类从字节数组创建图片
                return ms;
            }

        }

    }
}
