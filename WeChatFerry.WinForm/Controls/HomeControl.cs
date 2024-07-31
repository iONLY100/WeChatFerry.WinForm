using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeChatFerry.WinForm.Helper;
using WeChatFerry.WinForm.HttpApi;
using WeChatFerry.WinForm.WeChatFerry;

namespace WeChatFerry.WinForm.Controls
{
    public partial class HomeControl : UserControl
    {
        public HomeControl()
        {
            InitializeComponent();
            this.inputHttpApiPort.Value = GlobalValue.Config.HttpApiPort;
            this.inputCallBackAddress.Text = string.Join("\r\n", GlobalValue.Config.CallBackUrlList);
            this.Dock = DockStyle.Fill;
            this.Text = "首页";
        }

        private ILogger<HomeControl> _logger = NLogHelper.CreateLogger<HomeControl>();
        public IHost? ApiHost { get; set; }
        private static HomeControl? _instance;

        private List<string>? CallbackUrlList;
        public int ApiPort { get; set; }
        public static HomeControl Instance => _instance ??= new HomeControl();

        public async void RecvText(WxMsg msg)
        {
            _logger.LogInformation(JsonConvert.SerializeObject(msg));
            if (CallbackUrlList==null)
            {
                return;
            }
            foreach (var callbackUrl in CallbackUrlList)
            {
                await callbackUrl.PostJsonAsync(msg);
            }
        }

        private async void switchHttpApi_CheckedChanged(object sender, bool value)
        {

            if (ApiHost == null)
            {
                var port = (int)inputHttpApiPort.Value;
                this.ApiPort = port;
                var urlStr = $"http://*:{port}/";
                ApiHost = Host.CreateDefaultBuilder(null)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseUrls(urlStr);
                        webBuilder.UseStartup<Startup>();
                    }).Build();
            }

            if (value)
            {
                await ApiHost.StartAsync();
            }
            else
            {
                await ApiHost.StopAsync();
                ApiHost = null;
            }
        }

        private async void switchCallBack_CheckedChanged(object sender, bool value)
        {
            if (value)
            {
                var callbackAddressList = inputCallBackAddress.Text.Replace(" ","").Trim().Split("\r\n").ToList();
                var whiteSpaceList=new List<string>();
                foreach (var callbackUrl in callbackAddressList)
                {
                    if (string.IsNullOrWhiteSpace(callbackUrl))
                    {
                        whiteSpaceList.Add(callbackUrl);
                        continue;
                    }
                    if (!CanConnect(callbackUrl))
                    {
                        new Modal.Config(this.ParentForm!, "连接失败", callbackUrl, icon: TType.Error)
                        {
                            CancelText = null
                        }.open();
                        switchCallBack.Checked = false;
                        return;
                    }
                }

                foreach (string whiteSpaceStr in whiteSpaceList)
                {
                    callbackAddressList.Remove(whiteSpaceStr);
                }

                if (callbackAddressList.Count > 0)
                {
                    new Modal.Config(this.ParentForm!, "连接成功", "所有回调地址连接成功", icon: TType.Success)
                    {
                        CancelText = null
                    }.open();
                }
                else
                {
                    new Modal.Config(this.ParentForm!, "连接失败", "所有回调地址连接失败", icon: TType.Error)
                    {
                        CancelText = null
                    }.open();
                    switchCallBack.Checked = false;
                    return;
                }

                CallbackUrlList=callbackAddressList;

                await GlobalValue.WcfClient.EnableRecvMsg(RecvText, switchListenPyq.Checked);
            }
            else
            {
                await GlobalValue.WcfClient.DisableRecvMsg();
                CallbackUrlList=null;
            }
        }

        private bool CanConnect(string url = "")
        {
            try
            {
                using var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var uri = new Uri(url);
                var result = sock.BeginConnect(uri.Host, uri.Port, null, null);

                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));

                if (success)
                {
                    sock.EndConnect(result);
                    sock.Close();
                    return true;
                }

                sock.Close();
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
