using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Flurl.Http;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using nng;
using WeChatFerry.WinForm.Helper;

// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace WeChatFerry.WinForm.WeChatFerry;

public class WcfClient
{
    private bool _isReceivingMsg;
    private readonly ILogger<WcfClient> _logger;
    public IPairSocket CmdSocket;
    public IPairSocket? MsgSocket;
    public IAPIFactory<INngMsg> Factory;
    private readonly string _msgAddress;

    private BackgroundWorker? _backgroundWorker;

    public WcfClient(int port, Action<WxMsg>? func = null)
    {
        _logger = NLogHelper.CreateLogger<WcfClient>();
        const string tcpAddress = "tcp://127.0.0.1";
        var cmdAddress = $"{tcpAddress}:{port}";
        _msgAddress = $"{tcpAddress}:{port + 1}";

        var managedAssemblyPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
        var alc = new NngLoadContext(managedAssemblyPath);
        Factory = NngLoadContext.Init(alc, "nng.Factories.Latest.Factory");

        CmdSocket = Factory.PairOpen().ThenDial(cmdAddress).Unwrap();
    }

    /// <summary>
    /// 是否已启动接收消息功能
    /// </summary>
    /// <returns></returns>
    public bool IsReceivingMsg()
    {
        return _isReceivingMsg;
    }

    /// <summary>
    /// 获取登录二维码，已经登录则返回空字符串
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetQrCode()
    {
        var response = await GetResponse(new Request { Func = Functions.FuncRefreshQrcode });
        return response.Str;
    }

    /// <summary>
    /// 是否已经登录
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsLogin()
    {
        var response = await GetResponse(new Request { Func = Functions.FuncIsLogin });
        return response.Status == 1;
    }

    /// <summary>
    /// 获取登录账户的 wxid
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetSelfWxid()
    {
        var response = await GetResponse(new Request { Func = Functions.FuncGetSelfWxid });
        return response.Str;
    }

    /// <summary>
    /// 获取所有消息类型
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<int, string>> GetMsgTypes()
    {
        var response = await GetResponse(new Request { Func = Functions.FuncGetMsgTypes });
        var mapField = response.Types_.Types_;
        var dict = mapField.ToDictionary(pair => pair.Key, pair => pair.Value);
        return dict;
    }

    /// <summary>
    /// 获取完整通讯录
    /// </summary>
    /// <returns></returns>
    public async Task<List<RpcContact>> GetContacts()
    {
        var response = await GetResponse(new Request { Func = Functions.FuncGetContacts });
        var rpcContacts = response.Contacts.Contacts.ToList();
        return rpcContacts;
    }

    /// <summary>
    /// 获取所有数据库
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetDbNames()
    {
        var response = await GetResponse(new Request { Func = Functions.FuncGetDbNames });
        var list = response.Dbs.Names.ToList();
        return list;
    }

    /// <summary>
    /// 获取 db 中所有表
    /// </summary>
    /// <param name="dbName">数据库名（可通过 `get_dbs` 查询）</param>
    /// <returns>`db` 下的所有表名及对应建表语句</returns>
    public async Task<List<(string, string)>> GetDbTables(string dbName)
    {
        var response = await GetResponse(new Request { Func = Functions.FuncGetDbTables, Str = dbName });
        var dbTables = response.Tables.Tables;
        var list = dbTables.Select(dbTable => (dbTable.Name, dbTable.Sql)).ToList();
        return list;
    }

    /// <summary>
    /// 获取登录账号个人信息
    /// </summary>
    /// <returns></returns>
    public async Task<UserInfo> GetUserInfo()
    {
        var response = await GetResponse(new Request { Func = Functions.FuncGetUserInfo });
        return response.Ui;
    }

    /// <summary>
    /// 获取语音消息并转成 MP3
    /// </summary>
    /// <param name="id">语音消息 id</param>
    /// <param name="dir">MP3 保存目录（目录不存在会出错）</param>
    /// <param name="timeOut">超时时间（秒）</param>
    /// <returns>成功返回存储路径；空字符串为失败，原因见日志</returns>
    public async Task<string> GetAudioMsg(ulong id, string dir, uint timeOut = 3)
    {
        var request = new Request { Func = Functions.FuncGetAudioMsg, Am = new AudioMsg { Id = id, Dir = dir } };
        if (timeOut == 0)
        {
            var response = await GetResponse(request);
            return response.Str;
        }

        var count = 0;
        while (count < timeOut)
        {
            var pathStr = (await GetResponse(request)).Str;
            if (!string.IsNullOrWhiteSpace(pathStr))
            {
                return pathStr;
            }

            await Task.Delay(1000);
            count++;
        }

        return "";
    }

    /// <summary>
    /// 发送文本消息
    /// </summary>
    /// <param name="msg">要发送的消息，换行使用 `\n` （单杠）；如果 @ 人的话，需要带上跟 `aters` 里数量相同的 @</param>
    /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
    /// <param name="aters">要 @ 的 wxid，多个用逗号分隔；`@所有人` 只需要 `notify@all`</param>
    /// <returns>0 为成功，其他失败</returns>
    public async Task<int> SendText(string msg, string receiver, string aters = "")
    {
        var response = await GetResponse(new Request
        {
            Func = Functions.FuncSendTxt, Txt = new TextMsg { Msg = msg, Receiver = receiver, Aters = aters }
        });
        return response.Status;
    }

    /// <summary>
    /// 发送图片
    /// </summary>
    /// <param name="path">图片路径，如：`C:/Projs/WeChatRobot/TEQuant.jpeg` 或 `https://raw.githubusercontent.com/lich0821/WeChatFerry/master/assets/TEQuant.jpg`</param>
    /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
    /// <returns>0 为成功，其他失败</returns>
    public async Task<int> SendImage(string path, string receiver)
    {
        return await SendFile(path, receiver, Functions.FuncSendImg);
    }

    /// <summary>
    /// 发送文件
    /// </summary>
    /// <param name="path">本地文件路径，如：`C:/Projs/WeChatRobot/README.MD` 或 `https://raw.githubusercontent.com/lich0821/WeChatFerry/master/README.MD`</param>
    /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
    /// <returns>0 为成功，其他失败</returns>
    public async Task<int> SendFile(string path, string receiver)
    {
        return await SendFile(path, receiver, Functions.FuncSendFile);
    }

    /// <summary>
    /// 发送 XML
    /// </summary>
    /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
    /// <param name="xml">xml 内容</param>
    /// <param name="type">xml 类型，如：0x21 为小程序</param>
    /// <param name="path">封面图片路径</param>
    /// <returns>0 为成功，其他失败</returns>
    public async Task<int> SendXml(string receiver, string xml, int type, string? path = null)
    {
        throw new NotImplementedException("Not implemented, yet");
        var request = new Request
        {
            Func = Functions.FuncSendXml, Xml = new XmlMsg { Receiver = receiver, Content = xml, Type = type, }
        };
        if (!string.IsNullOrWhiteSpace(path))
        {
            request.Xml.Path = path;
        }

        var response = await GetResponse(request);
        return response.Status;
    }

    /// <summary>
    /// 发送表情
    /// </summary>
    /// <param name="path">本地表情路径，如：`C:/Projs/WeChatRobot/emo.gif`</param>
    /// <param name="receiver">消息接收人，wxid 或者 roomid</param>
    /// <returns>0 为成功，其他失败</returns>
    public async Task<int> SendEmotion(string path, string receiver)
    {
        return await SendFile(path, receiver, Functions.FuncSendEmotion);
    }

    /// <summary>
    /// <para>发送富文本消息</para>
    /// <para>卡片样式：</para>
    /// <para>|-------------------------------------|</para>
    /// <para>|title, 最长两行</para>
    /// <para>|(长标题, 标题短的话这行没有)</para>
    /// <para>|digest, 最多三行，会占位 |--------------|</para>
    /// <para>|digest, 最多三行，会占位 |---thumbUrl---|</para>
    /// <para>|digest, 最多三行，会占位 |--------------|</para>
    /// <para>|(account logo) name</para>
    /// <para>|-------------------------------------|</para>
    /// </summary>
    /// <param name="name">左下显示的名字</param>
    /// <param name="account">填公众号 id 可以显示对应的头像（gh_ 开头的）</param>
    /// <param name="title">标题，最多两行</param>
    /// <param name="digest">摘要，三行</param>
    /// <param name="url">点击后跳转的链接</param>
    /// <param name="thumbUrl">缩略图的链接</param>
    /// <param name="receiver">接收人, wxid 或者 roomid</param>
    /// <returns>0 为成功，其他失败</returns>
    public async Task<int> SendRichText(string name, string account, string title, string digest, string url,
        string thumbUrl, string receiver)
    {
        var response = await GetResponse(new Request
        {
            Func = Functions.FuncSendRichTxt,
            Rt = new RichText
            {
                Name = name,
                Account = account,
                Title = title,
                Digest = digest,
                Url = url,
                Thumburl = thumbUrl,
                Receiver = receiver
            }
        });
        return response.Status;
    }

    /// <summary>
    /// 拍一拍群友
    /// </summary>
    /// <param name="roomid">群 id</param>
    /// <param name="wxid">要拍的群友的 wxid</param>
    /// <returns>1 为成功，其他失败</returns>
    public async Task<int> SendPatMsg(string roomid, string wxid)
    {
        var response = await GetResponse(new Request()
        {
            Func = Functions.FuncSendPatMsg, Pm = new PatMsg { Roomid = roomid, Wxid = wxid }
        });
        return response.Status;
    }

    /// <summary>
    /// 转发消息。可以转发文本、图片、表情、甚至各种 XML；语音也行，不过效果嘛，自己验证吧。
    /// </summary>
    /// <param name="id">待转发消息的 id</param>
    /// <param name="receiver">消息接收者，wxid 或者 roomid</param>
    /// <returns>1 为成功，其他失败</returns>
    public async Task<int> ForwardMsg(ulong id, string receiver)
    {
        var response = await GetResponse(new Request
        {
            Func = Functions.FuncForwardMsg, Fm = new ForwardMsg { Id = id, Receiver = receiver }
        });
        return response.Status;
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="func">回调函数</param>
    /// <param name="pyq">是否监听朋友圈，默认否</param>
    /// <returns></returns>
    public async Task<bool> EnableRecvMsg(Action<WxMsg> func, bool pyq = false)
    {
        try
        {
            if (_isReceivingMsg)
            {
                return true;
            }

            var response = await GetResponse(new Request { Func = Functions.FuncEnableRecvTxt, Flag = pyq });
            if (response.Status != 0)
            {
                return false;
            }

            await Task.Delay(5000);
            _isReceivingMsg = true;
            MsgSocket ??= Factory.PairOpen().ThenDial(_msgAddress).Unwrap();
            _backgroundWorker ??= new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += (s, e) =>
            {
                while (_isReceivingMsg)
                {
                    Debug.WriteLine("213");

                    try
                    {
                        Thread.Sleep(1);
                        if (func == null)
                        {
                            continue;
                        }

                        var nngMsg = MsgSocket.RecvMsg().Unwrap();
                        var msgData = nngMsg.AsSpan().ToArray();
                        var resData = Response.Parser.ParseFrom(msgData);
                        Debug.WriteLine(JsonConvert.SerializeObject(resData.Wxmsg));
                        func(resData.Wxmsg);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            };
            _backgroundWorker.RunWorkerAsync();

            return _isReceivingMsg;
        }
        catch
        {
           await DisableRecvMsg();
           throw;
        }
    }

    /// <summary>
    /// 停止接收消息
    /// </summary>
    /// <returns></returns>
    public async Task<int> DisableRecvMsg()
    {
        if (!IsReceivingMsg())
        {
            return 0;
        }

        var response = await GetResponse(new Request { Func = Functions.FuncDisableRecvTxt });
        
        _backgroundWorker?.CancelAsync();
        _isReceivingMsg = false;
        _backgroundWorker=null;
        return response.Status;
    }

    /// <summary>
    /// 执行 SQL，如果数据量大注意分页，以免 BOOM
    /// </summary>
    /// <param name="db">要查询的数据库</param>
    /// <param name="sql">要执行的 SQL</param>
    /// <returns>查询结果</returns>
    public async Task<List<Dictionary<string, object?>>> QuerySql(string db, string sql)
    {
        var response = await GetResponse(new Request
        {
            Func = Functions.FuncExecDbQuery, Query = new DbQuery { Db = db, Sql = sql }
        });
        var rows = response.Rows.Rows;
        var list = new List<Dictionary<string, object?>>();
        foreach (var dbRow in rows)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var dbRowField in dbRow.Fields)
            {
                var c = DatabaseTypeConverter.ConvertType(dbRowField.Type, dbRowField.Content.ToByteArray());
                dict.Add(dbRowField.Column, c);
            }

            list.Add(dict);
        }

        return list;
    }

    /// <summary>
    /// 通过好友申请
    /// </summary>
    /// <param name="v3">加密用户名 (好友申请消息里 v3 开头的字符串)</param>
    /// <param name="v4">Ticket (好友申请消息里 v4 开头的字符串)</param>
    /// <param name="scene">申请方式 (好友申请消息里的 scene); 为了兼容旧接口，默认为扫码添加 (30)</param>
    /// <returns>1 为成功，其他失败</returns>
    public async Task<int> AcceptNewFriend(string v3, string v4, int scene = 30)
    {
        var response = await GetResponse(new Request
        {
            Func = Functions.FuncAcceptFriend, V = new Verification { V3 = v3, V4 = v4, Scene = scene }
        });
        return response.Status;
    }

    /// <summary>
    /// 获取好友列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<RpcContact>> GetFriends()
    {
        var notFriends = new Dictionary<string, string>
        {
            { "fmessage", "朋友推荐消息" },
            { "medianote", "语音记事本" },
            { "floatbottle", "漂流瓶" },
            { "filehelper", "文件传输助手" },
            { "newsapp", "新闻" }
        };
        var contacts = await GetContacts();
        var list = new List<RpcContact>();
        foreach (var contact in contacts)
        {
            if (contact.Wxid.EndsWith("@chatroom") || contact.Wxid.StartsWith("gh_") ||
                notFriends.ContainsKey(contact.Wxid))
            {
                continue;
            }

            list.Add(contact);
        }

        return list;
    }

    /// <summary>
    /// 接收转账
    /// </summary>
    /// <param name="wxid">转账消息里的发送人 wxid</param>
    /// <param name="transFerId">转账消息里的 transferid</param>
    /// <param name="transActionId">转账消息里的 transactionid</param>
    /// <returns>1 为成功，其他失败</returns>
    public async Task<int> ReceiveTransfer(string wxid, string transFerId, string transActionId)
    {
        var response = await GetResponse(new Request
        {
            Func = Functions.FuncRecvTransfer,
            Tf = new Transfer { Wxid = wxid, Tfid = transFerId, Taid = transActionId }
        });
        return response.Status;
    }

    /// <summary>
    /// 刷新朋友圈
    /// </summary>
    /// <param name="id">开始 id，0 为最新页</param>
    /// <returns>1 为成功，其他失败</returns>
    public async Task<int> RefreshPyq(ulong id = 0)
    {
        var response = await GetResponse(new Request { Func = Functions.FuncRefreshPyq, Ui64 = id });
        return response.Status;
    }

    /// <summary>
    /// 通过 wxid 查询微信号昵称等信息
    /// </summary>
    /// <param name="wxid">联系人 wxid</param>
    /// <returns></returns>
    public async Task<RpcContact?> GetInfoByWxid(string wxid)
    {
        var response = await GetResponse(new Request { Func = Functions.FuncGetContactInfo, Str = wxid });
        var rpcContacts = response.Contacts?.Contacts;
        if (rpcContacts == null || rpcContacts.Count == 0)
        {
            return null;
        }

        return rpcContacts[0];
    }

    /// <summary>
    /// 撤回消息
    /// </summary>
    /// <param name="id">待撤回消息的 id</param>
    /// <returns>1 为成功，其他失败</returns>
    public async Task<int> RevokeMsg(ulong id)
    {
        var response = await GetResponse(new Request { Func = Functions.FuncRevokeMsg, Ui64 = id });
        return response.Status;
    }

    /// <summary>
    /// 获取 OCR 结果。鸡肋，需要图片能自动下载；通过下载接口下载的图片无法识别。
    /// </summary>
    /// <param name="extra">待识别的图片路径，消息里的 extra</param>
    /// <param name="timeout"></param>
    /// <returns>OCR 结果</returns>
    public async Task<string> GetOcrResult(string extra, int timeout = 2)
    {
        async Task<(int, string)> Inner(string extraTmp)
        {
            var response = await GetResponse(new Request { Func = Functions.FuncExecOcr, Str = extraTmp });
            var ocrMsg = response.Ocr;
            return (ocrMsg.Status, ocrMsg.Result);
        }

        var count = 0;
        var result = (-999, "");
        while (count < timeout)
        {
            result = await Inner(extra);
            if (result.Item1 == 0)
            {
                break;
            }

            count++;
            await Task.Delay(1000);
        }

        if (result.Item1 != 0)
        {
            _logger.LogError($"OCR failed, status: {result.Item1}");
        }

        return result.Item2;
    }

    /// <summary>
    /// 下载图片
    /// </summary>
    /// <param name="id">消息中 id</param>
    /// <param name="extra">消息中的 extra</param>
    /// <param name="dir">存放图片的目录（目录不存在会出错）</param>
    /// <param name="timeout">超时时间（秒）</param>
    /// <returns>成功返回存储路径；空字符串为失败，原因见日志。</returns>
    public async Task<string> DownloadImage(ulong id, string extra, string dir, int timeout = 30)
    {
        var downloadAttach = await DownloadAttach(id, "", extra);
        if (downloadAttach != 0)
        {
            _logger.LogError("下载失败");
            return "";
        }

        var count = 0;
        while (count < timeout)
        {
            var path = await DecryptImage(extra, dir);
            if (!string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            count++;
            await Task.Delay(1000);
        }

        _logger.LogError("下载超时");
        return "";
    }

    /// <summary>
    /// 添加群成员
    /// </summary>
    /// <param name="roomid">待加群的 id</param>
    /// <param name="wxids">要加到群里的 wxid，多个用逗号分隔</param>
    /// <returns>1 为成功，其他失败</returns>
    public async Task<int> AddChatRoomMembers(string roomid, string wxids)
    {
        return await HandleChatRoomMembers(Functions.FuncAddRoomMembers, roomid, wxids);
    }

    /// <summary>
    /// 删除群成员
    /// </summary>
    /// <param name="roomid">待加群的 id</param>
    /// <param name="wxids">要删除成员的 wxid，多个用逗号分隔</param>
    /// <returns>1 为成功，其他失败</returns>
    public async Task<int> DelChatRoomMembers(string roomid, string wxids)
    {
        return await HandleChatRoomMembers(Functions.FuncDelRoomMembers, roomid, wxids);
    }

    /// <summary>
    /// 邀请群成员
    /// </summary>
    /// <param name="roomid">待加群的 id</param>
    /// <param name="wxids">要邀请成员的 wxid，多个用逗号分隔</param>
    /// <returns>1 为成功，其他失败</returns>
    public async Task<int> InviteChatRoomMembers(string roomid, string wxids)
    {
        return await HandleChatRoomMembers(Functions.FuncInvRoomMembers, roomid, wxids);
    }

    /// <summary>
    /// 获取群成员
    /// </summary>
    /// <param name="roomid">群的 id</param>
    /// <returns>群成员列表: {wxid1: 昵称1, wxid2: 昵称2, ...}</returns>
    public async Task<Dictionary<string, string>> GetChatRoomMembers(string roomid)
    {
        var members = new Dictionary<string, string>();
        var crs = await QuerySql("MicroMsg.db", $"SELECT RoomData FROM ChatRoom WHERE ChatRoomName = '{roomid}';");
        if (crs.Count == 0)
        {
            return members;
        }

        var bs = crs[0].GetValueOrDefault("RoomData");
        if (bs == null)
        {
            return members;
        }

        var crd = RoomData.Parser.ParseFrom((byte[])bs);

        var contacts = await QuerySql("MicroMsg.db", "SELECT UserName, NickName FROM Contact;");

        foreach (var member in crd.Members)
        {
            var nickName = member.Name;
            if (string.IsNullOrWhiteSpace(nickName))
            {
                nickName = contacts.FirstOrDefault(x => x.ContainsKey(member.Wxid))?[member.Wxid].ToString();
            }

            if (string.IsNullOrWhiteSpace(nickName))
            {
                nickName = await GetAliasInChatRoom(member.Wxid, roomid);
            }

            members.Add(member.Wxid, nickName);
        }

        return members;
    }

    /// <summary>
    /// 获取群名片
    /// </summary>
    /// <param name="wxid">wxid</param>
    /// <param name="roomid">群的 id</param>
    /// <returns>群名片</returns>
    public async Task<string> GetAliasInChatRoom(string wxid, string roomid)
    {
        var querySqlList = await QuerySql("MicroMsg.db", $"SELECT NickName FROM Contact WHERE UserName = '{wxid}';");
        if (querySqlList.Count == 0)
        {
            return "";
        }

        var nickName = querySqlList[0]["NickName"]?.ToString() ?? "";
        var crs = await QuerySql("MicroMsg.db", $"SELECT RoomData FROM ChatRoom WHERE ChatRoomName = '{roomid}';");
        if (crs.Count == 0)
        {
            return "";
        }

        var bs = crs[0]["RoomData"];
        if (bs == null)
        {
            return "";
        }

        // roomData
        var crd = RoomData.Parser.ParseFrom((byte[])bs);
        foreach (var member in crd.Members)
        {
            if (member.Wxid == wxid)
            {
                return string.IsNullOrWhiteSpace(member.Name) ? nickName : member.Name;
            }
        }

        return "";
    }

    public async Task<string> GetContactHeadImgByWxid(string wxid)
    {
        var data = await QuerySql("Misc.db", $"SELECT smallHeadBuf FROM \"ContactHeadImg1\" WHERE \"usrName\" = '{wxid}';");
        if (data.Count > 0)
        {
            if (data[0]["smallHeadBuf"] is byte[] bytes)
            {
                return Convert.ToBase64String(bytes);
            }
        }

        return "";
    }

    /// <summary>
    /// 下载附件（图片、视频、文件）。这方法别直接调用，下载图片使用 `download_image`。
    /// </summary>
    /// <param name="id">消息中 id</param>
    /// <param name="thumb">消息中的 thumb</param>
    /// <param name="extra">消息中的 extra</param>
    /// <returns></returns>
    private async Task<int> DownloadAttach(ulong id, string thumb, string extra)
    {
        var response = await GetResponse(new Request
        {
            Func = Functions.FuncDownloadAttach, Att = new AttachMsg { Id = id, Thumb = thumb, Extra = extra }
        });
        return response.Status;
    }

    /// <summary>
    /// 解密图片。这方法别直接调用，下载图片使用 `download_image`。
    /// </summary>
    /// <param name="src">加密的图片路径</param>
    /// <param name="dir">保存图片的目录</param>
    /// <returns>解密图片的保存路径</returns>
    private async Task<string> DecryptImage(string src, string dir)
    {
        var response = await GetResponse(new Request
        {
            Func = Functions.FuncDecryptImage, Dec = new DecPath { Src = src, Dst = dir }
        });
        return response.Str;
    }

    private async Task<Response> GetResponse(Request request)
    {
        using var asyncContext =
            Factory.CreateSendReceiveAsyncContext(CmdSocket, subtype: SendReceiveContextSubtype.Pair).Unwrap();
        var nngMsg = Factory.CreateMessage();
        nngMsg.Append(request.ToByteArray());
        var sendResult = await asyncContext.Send(nngMsg);
        if (sendResult.IsErr())
        {
            var throwMsg = $"发送失败，request：{JsonConvert.SerializeObject(request)}";
            _logger.LogError(throwMsg);
            throw new Exception(throwMsg);
        }

        var receiveResult = await asyncContext.Receive(CancellationToken.None);
        var recvMsg = receiveResult.Unwrap();
        var recvData = recvMsg.AsSpan().ToArray();
        var response = Response.Parser.ParseFrom(recvData);
        return response;
    }

    /// <summary>
    /// 发送文件、图片、表情
    /// </summary>
    /// <param name="path"></param>
    /// <param name="receiver"></param>
    /// <param name="functions"></param>
    /// <returns></returns>
    private async Task<int> SendFile(string path, string receiver, Functions functions)
    {
        path = await ProcessPath(path);
        if (int.TryParse(path, out int result))
        {
            return result;
        }

        var response = await GetResponse(new Request
        {
            Func = functions, File = new PathMsg { Path = path, Receiver = receiver }
        });
        return response.Status;
    }

    /// <summary>
    /// 下载网络文件
    /// </summary>
    /// <param name="url">网络地址</param>
    /// <returns></returns>
    private async Task<string> DownloadNetworkFile(string url)
    {
        try
        {
            var exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            var savePath = Path.Combine(exeDirectory, "tmpFile");
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var path = await url.WithSettings(settings => { settings.Timeout = TimeSpan.FromSeconds(60); })
                .DownloadFileAsync(savePath);
            return path;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网络资源下载失败");
            return "";
        }
    }

    /// <summary>
    /// 处理路径，如果是网络路径则下载文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private async Task<string> ProcessPath(string path)
    {
        if (path.StartsWith("http"))
        {
            path = await DownloadNetworkFile(path);
            if (string.IsNullOrWhiteSpace(path))
            {
                return "-102";
            }
        }
        else if (!File.Exists(path))
        {
            _logger.LogError(new FileNotFoundException("图片或文件不存在，请检查路径", path), "图片或文件不存在");
            return "-101";
        }

        return path;
    }

    /// <summary>
    /// 添加、删除、邀请群成员
    /// </summary>
    /// <param name="functions">操作方法</param>
    /// <param name="roomid">待加群的 id</param>
    /// <param name="wxids">要加到群里的 wxid，多个用逗号分隔</param>
    /// <returns>1 为成功，其他失败</returns>
    private async Task<int> HandleChatRoomMembers(Functions functions, string roomid, string wxids)
    {
        var response = await GetResponse(new Request
        {
            Func = functions, M = new MemberMgmt { Roomid = roomid, Wxids = wxids.Replace(" ", "") }
        });
        return response.Status;
    }

}