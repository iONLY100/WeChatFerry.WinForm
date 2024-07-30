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
    [ApiController]
    [Route("[controller]/[action]")]
    public class WcfController:ControllerBase
    {
        private readonly ILogger<WcfController> _logger = NLogHelper.CreateLogger<WcfController>();

        [HttpGet, HttpPost]
        public async Task<IActionResult> GetQrCodeAsync()
        {
            var data = await MainForm.Instance.WcfClient.GetQrCode();

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

        [HttpGet, HttpPost]
        public async Task<IActionResult> IsLoginAsync()
        {
            var data=await MainForm.Instance.WcfClient.IsLogin();
            return new UnifiedResponse<string>(HttpStatusCode.OK, data: data.ToString());
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> GetSelfWxidAsync()
        {
            var data = await MainForm.Instance.WcfClient.GetSelfWxid();
            return new UnifiedResponse<string>(HttpStatusCode.OK, data: data);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> GetMsgTypes()
        {
            var data = await MainForm.Instance.WcfClient.GetMsgTypes();
            return new UnifiedResponse<Dictionary<int, string>>(HttpStatusCode.OK, data: data);
        }
        [HttpGet, HttpPost]
        public async Task<IActionResult> GetContacts()
        {
            var data = await MainForm.Instance.WcfClient.GetContacts();
            return new UnifiedResponse<List<RpcContact>>(HttpStatusCode.OK, data: data);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> GetDbNames()
        {
            var data=await MainForm.Instance.WcfClient.GetDbNames();
            return new UnifiedResponse<List<string>>(HttpStatusCode.OK, data: data);
        }
        [HttpGet]
        public async Task<IActionResult> GetDbTables(string dbName)
        {
            return await GetDbTables(new OneParamInputDto<string> { Param = dbName });
        }

        [HttpPost]
        public async Task<IActionResult> GetDbTables(OneParamInputDto<string> input)
        {
            var data = await MainForm.Instance.WcfClient.GetDbTables(input.Param);
            return new UnifiedResponse<List<(string, string)>>(HttpStatusCode.OK, data: data);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> GetUserInfo()
        {
            var data=await MainForm.Instance.WcfClient.GetUserInfo();
            return new UnifiedResponse<UserInfo>(HttpStatusCode.OK, data: data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAudioMsg(ulong id, uint timeOut = 3)
        {
            return await GetAudioMsg(new TwoParamInputDto<ulong, uint>() { Param1 = id, Param2 = timeOut });
        }

        [HttpPost]
        public async Task<IActionResult> GetAudioMsg(TwoParamInputDto<ulong,uint> input)
        {
            
            var savePath = FileHelper.GetTempFileDataPath();
            var data = await MainForm.Instance.WcfClient.GetAudioMsg(input.Param1, savePath, input.Param2);
            if (string.IsNullOrWhiteSpace(data) || !System.IO.File.Exists(data))
            {
                return new UnifiedResponse<object>(HttpStatusCode.NotFound, "获取语音消息失败");
            }

            await using var stream = System.IO.File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
           
            return await Task.FromResult<FileResult>(File(stream, "audio/mpeg"));

        }


        [HttpGet]
        public async Task<IActionResult> SendText(string msg, string receiver, string aters = "")
        {
            return await SendText(new TextMsg { Msg = msg, Receiver = receiver, Aters = aters });
        }

        [HttpPost]
        public async Task<IActionResult> SendText(TextMsg msg)
        {
            var data = await MainForm.Instance.WcfClient.SendText(msg.Msg, msg.Receiver, msg.Aters);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        [HttpGet]
        public async Task<IActionResult> SendImage(string path, string receiver)
        {
            return await SendImage(new PathMsg { Path = path, Receiver = receiver });
        }

        [HttpPost]
        public async Task<IActionResult> SendImage(PathMsg msg)
        {
            var data = await MainForm.Instance.WcfClient.SendImage(msg.Path,msg.Receiver);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }


        [HttpGet]
        public async Task<IActionResult> SendFile(string path, string receiver)
        {
            return await SendFile(new PathMsg { Path = path, Receiver = receiver });
        }

        [HttpPost]
        public async Task<IActionResult> SendFile(PathMsg msg)
        {
            var data = await MainForm.Instance.WcfClient.SendFile(msg.Path, msg.Receiver);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }


        [HttpGet]
        public async Task<IActionResult> SendEmotion(string path, string receiver)
        {
            return await SendEmotion(new PathMsg { Path = path, Receiver = receiver });
        }

        [HttpPost]
        public async Task<IActionResult> SendEmotion(PathMsg msg)
        {
            var data = await MainForm.Instance.WcfClient.SendEmotion(msg.Path, msg.Receiver);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        [HttpGet]
        public async Task<IActionResult> SendXml(string receiver, string xml, int type, string? path = null)
        {
            return await SendXml(new XmlMsg { Receiver = receiver, Content = xml, Type = type, Path = path });
        }

        [HttpPost]
        public async Task<IActionResult> SendXml(XmlMsg msg)
        {
            var data = await MainForm.Instance.WcfClient.SendXml(msg.Receiver,msg.Content,msg.Type,msg.Path);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        [HttpGet]
        public async Task<IActionResult> SendRichText(string name, string account, string title, string digest, string url,
            string thumbUrl, string receiver)
        {
            return await SendRichText(new RichText { Name = name, Account = account, Title = title, Digest = digest, Url = url, Thumburl = thumbUrl, Receiver = receiver });
        }
        [HttpPost]
        public async Task<IActionResult> SendRichText(RichText msg)
        {
            var data = await MainForm.Instance.WcfClient.SendRichText(msg.Name,msg.Account,msg.Title,msg.Digest,msg.Url,msg.Thumburl,msg.Receiver);
            if (data == 0)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }
        [HttpGet]
        public async Task<IActionResult> SendPatMsg(string roomid, string wxid)
        {
            return await SendPatMsg(new PatMsg { Roomid = roomid, Wxid = wxid });
        }
        [HttpPost]
        public async Task<IActionResult> SendPatMsg(PatMsg msg)
        {
            var data = await MainForm.Instance.WcfClient.SendPatMsg(msg.Roomid, msg.Wxid);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        [HttpGet]
        public async Task<IActionResult> ForwardMsg(ulong id, string receiver)
        {
            return await ForwardMsg(new ForwardMsg { Id = id, Receiver = receiver });
        }
        [HttpPost]
        public async Task<IActionResult> ForwardMsg(ForwardMsg msg)
        {
            var data = await MainForm.Instance.WcfClient.ForwardMsg(msg.Id, msg.Receiver);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "发送失败");
        }

        [HttpGet]
        public async Task<IActionResult> QuerySql(string db, string sql)
        {
            return await QuerySql(new DbQuery { Db = db, Sql = sql });
        }
        [HttpPost]
        public async Task<IActionResult> QuerySql(DbQuery msg)
        {
            var data = await MainForm.Instance.WcfClient.QuerySql(msg.Db, msg.Sql);
            return new UnifiedResponse<List<Dictionary<string, object?>>>(HttpStatusCode.OK, data: data);
        }

        [HttpGet]
        public async Task<IActionResult> AcceptNewFriend(string v3, string v4, int scene = 30)
        {
            return await AcceptNewFriend(new Verification { V3 = v3, V4 = v4, Scene = scene });
        }
        [HttpPost]
        public async Task<IActionResult> AcceptNewFriend(Verification msg)
        {
            var data = await MainForm.Instance.WcfClient.AcceptNewFriend(msg.V3, msg.V4, msg.Scene);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "通过好友申请失败");
        }

        [HttpGet,HttpPost]
        public async Task<IActionResult> GetFriends()
        {
            var data=await MainForm.Instance.WcfClient.GetFriends();
            return new UnifiedResponse<List<RpcContact>>(HttpStatusCode.OK, data: data);
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveTransfer(Transfer msg)
        {
            var data = await MainForm.Instance.WcfClient.ReceiveTransfer(msg.Wxid, msg.Tfid, msg.Taid);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "接收转账失败");
        }
        [HttpPost]
        public async Task<IActionResult> RefreshPyq(OneParamInputDto<ulong> input)
        {
            var data = await MainForm.Instance.WcfClient.RefreshPyq(input.Param);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "刷新朋友圈失败");
        }

        [HttpPost]
        public async Task<IActionResult> GetInfoByWxid(OneParamInputDto<string> input)
        {
            var data = await MainForm.Instance.WcfClient.GetInfoByWxid(input.Param);
            return new UnifiedResponse<RpcContact>(HttpStatusCode.OK, data: data);
        }
        [HttpPost]
        public async Task<IActionResult> RevokeMsg(OneParamInputDto<ulong> input)
        {
            var data = await MainForm.Instance.WcfClient.RevokeMsg(input.Param);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }

            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "撤回消息失败");
        }

        [HttpPost]
        public async Task<IActionResult> GetOcrResult(TwoParamInputDto<string, int> input)
        {
            var data = await MainForm.Instance.WcfClient.GetOcrResult(input.Param1, input.Param2);
            return new UnifiedResponse<string>(HttpStatusCode.OK, data: data);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadImage(DownloadImageInputDto input)
        {
            var dir = FileHelper.GetTempFileDataPath();
            var data = await MainForm.Instance.WcfClient.DownloadImage(input.Id,input.Extra,dir,input.Timeout);
            if (string.IsNullOrWhiteSpace(data)||!System.IO.File.Exists(data))
            {
                return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "下载图片失败");
            }
            await using var stream = System.IO.File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return await Task.FromResult<FileResult>(File(stream, "image/jpg"));
        }

        [HttpPost]
        public async Task<IActionResult> AddChatRoomMembers(MemberMgmt input)
        {
            var data = await MainForm.Instance.WcfClient.AddChatRoomMembers(input.Roomid,input.Wxids);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "添加群成员失败");
        }
        [HttpPost]
        public async Task<IActionResult> DelChatRoomMembers(MemberMgmt input)
        {
            var data = await MainForm.Instance.WcfClient.DelChatRoomMembers(input.Roomid, input.Wxids);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "删除群成员失败");
        }
        [HttpPost]
        public async Task<IActionResult> InviteChatRoomMembers(MemberMgmt input)
        {
            var data = await MainForm.Instance.WcfClient.InviteChatRoomMembers(input.Roomid, input.Wxids);
            if (data == 1)
            {
                return new UnifiedResponse<object>(HttpStatusCode.OK);
            }
            return new UnifiedResponse<object>(HttpStatusCode.InternalServerError, "邀请群成员失败");
        }
        [HttpPost]
        public async Task<IActionResult> GetChatRoomMembers(OneParamInputDto<string> input)
        {
            var data = await MainForm.Instance.WcfClient.GetChatRoomMembers(input.Param);
            return new UnifiedResponse<Dictionary<string,string>>(HttpStatusCode.OK, data:data);
        }

        [HttpPost]
        public async Task<IActionResult> GetAliasInChatRoom(TwoParamInputDto<string,string> input)
        {
            if (string.IsNullOrWhiteSpace(input.Param2))
            {
                throw new Exception("param2 is required, must be not null");
            }
            var data = await MainForm.Instance.WcfClient.GetAliasInChatRoom(input.Param1,input.Param2);
            return new UnifiedResponse<string>(HttpStatusCode.OK, data: data);
        }



    }
}
