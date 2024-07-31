using System.ComponentModel;
using System.Diagnostics;
using AntdUI;
using WeChatFerry.WinForm.Controls;
using WeChatFerry.WinForm.WeChatFerry;
using System.Reflection;
using Microsoft.Extensions.Logging;
using WeChatFerry.WinForm.Helper;
using System.Text;

namespace WeChatFerry.WinForm;

public partial class MainForm : Window
{
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x02000000;
            return cp;
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        DraggableMouseDown();
        base.OnMouseDown(e);
    }

    private static MainForm? _instance;
    public static MainForm Instance => _instance ??= new MainForm();
    private WebView2Control? HttpApiDocControl { get; set; }
    public readonly SdkClass Sdk;
    private readonly DateTime _runStartTime = DateTime.Now;

    private readonly BackgroundWorker _worker = new();
    private ILogger<MainForm> _logger = NLogHelper.CreateLogger<MainForm>();

    public MainForm()
    {


        _logger.LogInformation("starting...");
        InitializeComponent();
        Size = new Size(800, 580);
        labelVer.Text =
            $"ver.{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}";
        labelTitle.MouseDown += (_, _) => { DraggableMouseDown(); };
        avatarLogo.MouseDown += (_, _) => { DraggableMouseDown(); };
        panelLeft.MouseDown += (_, _) => { DraggableMouseDown(); };
        _worker.DoWork += _worker_DoWork;
        Sdk = new SdkClass(GlobalValue.Config.SpyFilePath);
        var loginForm = new LoginForm();
        loginForm.LoginClick += async (obj, _) =>
        {
            if (obj is not Dictionary<string, int> dic) return;
            var debug = dic["debug"] == 1;
            var port = dic["port"];
            InjectSpy(debug, port);
            while (!await GlobalValue.WcfClient.IsLogin())
            {
                await Task.Delay(1000);
            }
        };
        loginForm.ShowDialog();
        AddControl(HomeControl.Instance);

        _worker.RunWorkerAsync();

        // 创建一个 Timer 对象，指定回调方法、状态对象、延时时间和间隔时间

        var timer = new System.Windows.Forms.Timer();
        timer.Interval = 1000;
        timer.Tick += (s, e) =>
        {
            labelRunTotalTime.Invoke(() =>
            {
                labelRunTotalTime.Text = CalculateTotalTime(_runStartTime, DateTime.Now);
            });
        };
        timer.Start();
    }

    private void _worker_DoWork(object? sender, DoWorkEventArgs e)
    {
        while (true)
        {
            System.Threading.Thread.Sleep(1000 * 60);
            try
            {
                var fileList = Directory.GetFiles(FileHelper.GetTempFileDataPath());
                foreach (var file in fileList)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.LastAccessTime.AddMinutes(10) < DateTime.Now)
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }

    private string CalculateTotalTime(DateTime startTime, DateTime endTime)
    {
        var totalSeconds = (int)(endTime - startTime).TotalSeconds;

        var days = totalSeconds / 86400; // 86400 秒是一天的秒数
        var hours = (totalSeconds % 86400) / 3600;
        var minutes = (totalSeconds % 3600) / 60;
        var seconds = totalSeconds % 60;

        // 构建返回字符串
        var result = "已运行：";
        if (days > 0)
        {
            result += $"{days}天";
        }

        result += $"{hours:00}时{minutes:00}分{seconds:00}秒";

        return result;
    }

    public void InjectSpy(bool debug, int port)
    {
        var destroy = Sdk.WxDestroySdk();

        var initSdk = Sdk.WxInitSdk(debug, port);
        if (initSdk != 0)
        {
            new Modal.Config(this, "注入失败", "注入失败", icon: TType.Error) { CancelText = null }.open();
        }
        else
        {
            Notification.success(this, "注入成功", "注入成功", TAlignFrom.Top, Font);
        }

        GlobalValue.WcfClient = new WcfClient(port: port);
    }

    private void AddControl(Control control)
    {
        FloatButton?.Close();
        panel1.Controls.Clear();
        panel1.Controls.Add(control);
        labelTitle.Text = control.Text;
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {

        ExitApp();
    }

    public string inputSvg =
        "<svg t=\"1722153465519\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"7093\" width=\"200\" height=\"200\"><path d=\"M585.142857 365.714286a73.142857 73.142857 0 0 1 73.142857 73.142857v390.095238a73.142857 73.142857 0 0 1-73.142857 73.142857H195.047619a73.142857 73.142857 0 0 1-73.142857-73.142857V438.857143a73.142857 73.142857 0 0 1 73.142857-73.142857h390.095238z m0 73.142857H195.047619v390.095238h390.095238V438.857143z m-73.142857 219.428571v73.142857H268.190476v-73.142857h243.809524zM828.952381 121.904762a73.142857 73.142857 0 0 1 73.142857 73.142857v390.095238a73.142857 73.142857 0 0 1-73.142857 73.142857h-121.904762v-73.142857h121.904762V195.047619H438.857143v121.904762h-73.142857V195.047619a73.142857 73.142857 0 0 1 73.142857-73.142857h390.095238zM512 536.380952v73.142858H268.190476v-73.142858h243.809524z\" p-id=\"7094\"></path></svg>";

    private void btnHome_Click(object sender, EventArgs e)
    {
        AddControl(HomeControl.Instance);
    }

    Form? FloatButton = null;

    private void btnApiDoc_Click(object sender, EventArgs e)
    {
        if (HomeControl.Instance.ApiHost == null)
        {
            Notification.error(this, "请先启用HTTPAPI", "", TAlignFrom.Top, Font);
            return;
        }

        string httpApiUrl = $"http://127.0.0.1:{HomeControl.Instance.ApiPort}/swagger/index.html";
        if (HttpApiDocControl == null || string.IsNullOrWhiteSpace(HttpApiDocControl.SourceUrl) ||
            HttpApiDocControl.SourceUrl != httpApiUrl)
        {
            HttpApiDocControl = new WebView2Control(httpApiUrl, "HttpApi文档");
        }

        AddControl(HttpApiDocControl);
        string svgStr =
            "<svg t=\"1722166814446\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"10727\" width=\"200\" height=\"200\"><path d=\"M970.1888 232.8064c19.9168-68.9664 26.112-203.4688-97.3824-220.16-99.2768-13.4656-216.7296 52.736-278.6816 93.7472-20.3776-2.7648-41.6256-4.6592-64.3072-4.9152-160.9216-2.0992-265.472 52.736-353.8944 168.4992-32.6144 42.5472-61.6448 114.8928-70.7072 196.8128 45.7216-77.7216 182.272-217.9072 328.96-275.5584 0 0-220.2112 157.2352-328.0896 381.952l-0.256 0.256c0.1536 1.1264 0.1536 2.1504 0.256 3.328-4.608 10.24-9.472 20.3776-13.7216 31.1296-107.4688 264.0896-19.5584 378.368 60.8256 399.5648 74.0864 19.456 178.3808-16.7424 261.2736-105.7792 141.9776 33.3824 281.2928-4.1472 334.336-33.1776 99.7376-54.6304 167.3216-151.1424 183.3984-250.4192h-271.9744s-11.4176 87.8592-157.2352 87.8592c-134.4 0-140.1344-155.5968-140.1344-155.5968H940.544s10.9056-167.6288-71.9872-279.3984c-45.9776-62.0544-108.8512-116.6848-196.4544-146.2272 26.88-19.712 73.0112-50.4832 112.0256-60.6208 73.9328-19.3024 124.6208-7.8848 156.416 45.9776 43.1616 73.216-23.808 244.48-23.808 244.48s32.6656-49.664 53.4528-121.7536zM373.248 889.9584c-114.3296 93.1328-209.152 82.8928-245.7088 26.7776-31.6416-48.8448-37.376-136.7552-0.2048-256.5632 17.408 46.4384 44.5952 91.3408 85.0944 131.2768 49.3056 48.6912 104.6016 79.616 160.8192 98.5088z m-4.1472-483.3792s5.7856-110.2336 125.5936-120.32c104.6528-8.6528 158.72 37.0176 174.848 125.7472l-300.4416-5.4272z m0 0\" p-id=\"10728\"></path></svg>";
        FloatButton = new AntdUI.FloatButton.Config(this,
        [
            new AntdUI.FloatButton.ConfigBtn("id1", svgStr, true)
            {
                Tooltip = "在默认浏览器中打开", Type = AntdUI.TTypeMini.Primary
            }
        ], btn =>
        {
            void OpenUrl(string url)
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }

            OpenUrl(httpApiUrl);
            Notification.error(this, "已在默认浏览器中打开，请查看", "", TAlignFrom.Top, Font);
        }).open();
    }

    private void btnSettingEdit_Click(object sender, EventArgs e)
    {
        string settingStr;
        using (var fs = new FileStream(GlobalValue.ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            var s = new StreamReader(fs, Encoding.Default);
            settingStr = s.ReadToEnd();
        }
        AddControl(new SettingEditControl("配置文件", settingStr));
    }

    private void labelVer_Click(object sender, EventArgs e)
    {
        AddControl(ShowUpdateInfoControl.Instance);
    }

    private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            const string svgShare =
                "<svg viewBox=\"0 0 1024 1024\"><path d=\"M752 664c-28.5 0-54.8 10-75.4 26.7L469.4 540.8c1.7-9.3 2.6-19 2.6-28.8s-0.9-19.4-2.6-28.8l207.2-149.9C697.2 350 723.5 360 752 360c66.2 0 120-53.8 120-120s-53.8-120-120-120-120 53.8-120 120c0 11.6 1.6 22.7 4.7 33.3L439.9 415.8C410.7 377.1 364.3 352 312 352c-88.4 0-160 71.6-160 160s71.6 160 160 160c52.3 0 98.7-25.1 127.9-63.8l196.8 142.5c-3.1 10.6-4.7 21.8-4.7 33.3 0 66.2 53.8 120 120 120s120-53.8 120-120-53.8-120-120-120z m0-476c28.7 0 52 23.3 52 52s-23.3 52-52 52-52-23.3-52-52 23.3-52 52-52zM312 600c-48.5 0-88-39.5-88-88s39.5-88 88-88 88 39.5 88 88-39.5 88-88 88z m440 236c-28.7 0-52-23.3-52-52s23.3-52 52-52 52 23.3 52 52-23.3 52-52 52z\"></path></svg>";
            const string svgAbout =
                "<svg viewBox=\"0 0 1024 1024\"><path d=\"M716.3 313.8c19-18.9 19-49.7 0-68.6l-69.9-69.9 0.1 0.1c-18.5-18.5-50.3-50.3-95.3-95.2-21.2-20.7-55.5-20.5-76.5 0.5L80.9 474.2c-21.2 21.1-21.2 55.3 0 76.4L474.6 944c21.2 21.1 55.4 21.1 76.5 0l165.1-165c19-18.9 19-49.7 0-68.6-19-18.9-49.7-18.9-68.7 0l-125 125.2c-5.2 5.2-13.3 5.2-18.5 0L189.5 521.4c-5.2-5.2-5.2-13.3 0-18.5l314.4-314.2c0.4-0.4 0.9-0.7 1.3-1.1 5.2-4.1 12.4-3.7 17.2 1.1l125.2 125.1c19 19 49.8 19 68.7 0z\"></path><path d=\"M408.6 514.4a106.3 106.2 0 1 0 212.6 0 106.3 106.2 0 1 0-212.6 0Z\"></path><path d=\"M944.8 475.8L821.9 353.5c-19-18.9-49.8-18.9-68.7 0.1-19 18.9-19 49.7 0 68.6l83 82.9c5.2 5.2 5.2 13.3 0 18.5l-81.8 81.7c-19 18.9-19 49.7 0 68.6 19 18.9 49.7 18.9 68.7 0l121.8-121.7c21.1-21.1 21.1-55.2-0.1-76.4z\"></path></svg>";

            var menuList = new AntdUI.IContextMenuStripItem[]
            {
                new AntdUI.ContextMenuStripItem("显示主窗口") { IconSvg = svgShare },
                new AntdUI.ContextMenuStripItemDivider(),
                new AntdUI.ContextMenuStripItem("退出") { IconSvg = svgAbout },
            };
            new AntdUI.ContextMenuStrip.Config(this, it =>
            {
                switch (it.Text)
                {
                    case "退出":
                        {
                            ExitApp();
                            break;
                        }
                    case "显示主窗口":
                        this.notifyIcon1.Visible = false;
                        this.Show();
                        break;
                }
            }, menuList, 0)
            { TopMost = true, Align = TAlign.TR }.open();
        }
    }

    private async void ExitApp()
    {
        await GlobalValue.WcfClient.DisableRecvMsg();
        Sdk.WxDestroySdk();
        LoginForm.ExitApp();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        new Modal.Config(this, "是否要退出WeChatFerry", "", icon: TType.Warn)
        {
            OkText = "是",
            Btns =
            [
                new Modal.Btn("Minimize2tray", "到托盘", TTypeMini.Default),
                new Modal.Btn("CancelExit", "否", TTypeMini.Default),
            ],
            OnBtns = btn =>
            {
                switch (btn.Name)
                {
                    case "Minimize2tray":
                        {
                            this.notifyIcon1.Visible = true;
                            this.Hide();
                            e.Cancel = true;
                            break;
                        }
                    case "CancelExit":
                        {
                            e.Cancel = true;
                            break;
                        }
                }
            },
            OnOk = _ =>
            {
                new Modal.Config(this, "是否要同时退出微信", "", icon: TType.Warn)
                {
                    OkText = "是",
                    CancelText = "否",
                    OnOk = _ =>
                    {
                        ExitProcess("wechat");
                        return true;
                    }
                }.open();
                return true;
            },
            CancelText = null
        }.open();
    }

    private void ExitProcess(string processName)
    {
        foreach (var process in Process.GetProcessesByName(processName))
        {
            try
            {
                process.Kill(); // 结束进程
                process.WaitForExit(); // 等待进程真正退出
            }
            catch (Exception ex)
            {
                Console.WriteLine($"无法关闭 {processName}: {ex.Message}");
            }
        }
    }

    private void avatar_Click(object sender, EventArgs e)
    {
        AntdUI.Popover.open(avatar, new UserInfoControl { Size = new Size(370, 200) }, TAlign.RT);
    }



    private async void MainForm_Shown(object sender, EventArgs e)
    {
        var avatarBase64Str = await GlobalValue.WcfClient.GetContactHeadImgByWxid(await GlobalValue.WcfClient.GetSelfWxid());

        if (string.IsNullOrWhiteSpace(avatarBase64Str))
        {
            return;
        }
        avatar.Invoke(() => { avatar.Image = ConvertFromBase64ToImage(avatarBase64Str); });

        return;

        Bitmap ConvertFromBase64ToImage(string base64String)
        {
            // 将base64字符串转换为字节数组
            var imageBytes = Convert.FromBase64String(base64String);
            using MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            // 使用Bitmap类从字节数组创建图片
            Bitmap bmp = new Bitmap(ms);
            return bmp;
        }
    }
}