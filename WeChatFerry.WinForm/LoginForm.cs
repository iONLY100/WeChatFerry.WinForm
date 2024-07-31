using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI;
using WeChatFerry.WinForm.Helper;
using WeChatFerry.WinForm.WeChatFerry;

namespace WeChatFerry.WinForm
{
    public partial class LoginForm : Window
    {
        public event EventHandler? LoginClick;
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            DraggableMouseDown();
            base.OnMouseDown(e);
        }

        public LoginForm()
        {
            InitializeComponent();
            this.Load += LoginForm_Load;
            this.inputNumPort.Value = GlobalValue.Config.WcfPort;
            this.switchDebug.Checked = GlobalValue.Config.DebugModel == 1;

            this.Size = new Size(280, 380);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            labelVer.Text = $"ver.{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}";
            avatarLogo.MouseDown += (_, _) => { DraggableMouseDown(); };
            panelLogo.MouseDown += (_, _) => { DraggableMouseDown(); };
            labelVer.MouseDown += (_, _) => { DraggableMouseDown(); };
        }

        private void LoginForm_Load(object? sender, EventArgs e)
        {
            this.Left -= this.Width;
        }

        public sealed override Size MinimumSize
        {
            get => base.MinimumSize;
            set => base.MinimumSize = value;
        }

        public sealed override Size MaximumSize
        {
            get => base.MaximumSize;
            set => base.MaximumSize = value;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            var dialogResult = new Modal.Config(this, "", "是否要退出\r\nWeChatFerry", icon: TType.Warn)
            {
                Width = this.Width - 60
            }.open();
            if (dialogResult == DialogResult.OK)
            {
                Environment.Exit(0);
            }
        }

        public static void ExitApp()
        {
            Environment.Exit(0);
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            AntdUI.Button btn = (AntdUI.Button)sender;
            var btnText = btn.Text;
            var dic = new Dictionary<string, int>
            {
                { "debug", switchDebug.Checked ? 1 : 0 },
                { "port", (int)inputNumPort.Value }
            };
            btn.Loading = true;
            btn.Enabled = false;
            switchDebug.Enabled = false;
            inputNumPort.Enabled = false;
            btn.Text = "等待微信登录";
            await Task.Run(async () =>
            {
                LoginClick?.Invoke(dic, e);
                while (!await GlobalValue.WcfClient.IsLogin())
                {
                    await Task.Delay(1000);
                }
                if (btn.IsDisposed) return;
                btn.Invoke(() =>
                {
                    if (btn.IsDisposed) return;
                    btn.Loading = false;
                    btn.Enabled = true;
                    switchDebug.Enabled = true;
                    inputNumPort.Enabled = true;
                    btn.Text = btnText;
                    this.DialogResult = DialogResult.OK;
                });
            });
        }
    }
}
