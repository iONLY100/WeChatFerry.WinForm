namespace WeChatFerry.WinForm
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            panelLogo = new AntdUI.Panel();
            btnStart = new AntdUI.Button();
            label2 = new AntdUI.Label();
            label1 = new AntdUI.Label();
            switchDebug = new AntdUI.Switch();
            inputNumPort = new AntdUI.InputNumber();
            divider1 = new AntdUI.Divider();
            btnClose = new AntdUI.Button();
            tooltipClose = new AntdUI.TooltipComponent();
            labelVer = new AntdUI.Label();
            avatarLogo = new AntdUI.Avatar();
            SuspendLayout();
            // 
            // panelLogo
            // 
            panelLogo.BackgroundImage = (Image)resources.GetObject("panelLogo.BackgroundImage");
            panelLogo.BackgroundImageLayout = AntdUI.TFit.Contain;
            panelLogo.Location = new Point(47, 121);
            panelLogo.Name = "panelLogo";
            panelLogo.Size = new Size(186, 37);
            panelLogo.TabIndex = 0;
            panelLogo.Text = "panel1";
            // 
            // btnStart
            // 
            btnStart.ImageHoverSvg = "";
            btnStart.ImageSvg = resources.GetString("btnStart.ImageSvg");
            btnStart.Location = new Point(50, 203);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(180, 44);
            btnStart.TabIndex = 1;
            btnStart.Type = AntdUI.TTypeMini.Success;
            btnStart.Click += btnStart_Click;
            // 
            // label2
            // 
            label2.AutoSizeMode = AntdUI.TAutoSize.Auto;
            label2.Font = new Font("Microsoft YaHei UI", 12F);
            label2.Location = new Point(148, 313);
            label2.Name = "label2";
            label2.Size = new Size(39, 23);
            label2.TabIndex = 8;
            label2.Text = "端口";
            // 
            // label1
            // 
            label1.AutoSizeMode = AntdUI.TAutoSize.Auto;
            label1.Font = new Font("Microsoft YaHei UI", 12F);
            label1.Location = new Point(20, 313);
            label1.Name = "label1";
            label1.Size = new Size(72, 23);
            label1.TabIndex = 7;
            label1.Text = "调试模式";
            // 
            // switchDebug
            // 
            switchDebug.AutoCheck = true;
            switchDebug.Checked = true;
            switchDebug.Location = new Point(86, 310);
            switchDebug.Name = "switchDebug";
            switchDebug.Size = new Size(45, 28);
            switchDebug.TabIndex = 6;
            switchDebug.Text = "switch1";
            // 
            // inputNumPort
            // 
            inputNumPort.Font = new Font("Microsoft YaHei UI", 12F);
            inputNumPort.Location = new Point(181, 307);
            inputNumPort.Maximum = new decimal(new int[] { 49151, 0, 0, 0 });
            inputNumPort.Minimum = new decimal(new int[] { 1024, 0, 0, 0 });
            inputNumPort.Name = "inputNumPort";
            inputNumPort.ShowControl = false;
            inputNumPort.Size = new Size(80, 35);
            inputNumPort.TabIndex = 5;
            inputNumPort.Text = "10086";
            inputNumPort.Value = new decimal(new int[] { 10086, 0, 0, 0 });
            // 
            // divider1
            // 
            divider1.ColorSplit = SystemColors.ControlDark;
            divider1.Location = new Point(137, 313);
            divider1.Name = "divider1";
            divider1.Orientation = AntdUI.TOrientation.Left;
            divider1.Size = new Size(5, 23);
            divider1.TabIndex = 9;
            divider1.Text = "";
            divider1.Thickness = 1F;
            divider1.Vertical = true;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackHover = Color.FromArgb(251, 115, 115);
            btnClose.ImageHoverSvg = resources.GetString("btnClose.ImageHoverSvg");
            btnClose.ImageSvg = resources.GetString("btnClose.ImageSvg");
            btnClose.Location = new Point(248, 0);
            btnClose.Margin = new Padding(0);
            btnClose.Margins = 0;
            btnClose.Name = "btnClose";
            btnClose.Radius = 0;
            btnClose.Size = new Size(32, 25);
            btnClose.TabIndex = 10;
            tooltipClose.SetTip(btnClose, "关闭系统");
            btnClose.Click += btnClose_Click;
            // 
            // tooltipClose
            // 
            tooltipClose.ArrowAlign = AntdUI.TAlign.BR;
            tooltipClose.Font = new Font("Microsoft YaHei UI", 12F);
            // 
            // labelVer
            // 
            labelVer.Dock = DockStyle.Bottom;
            labelVer.Location = new Point(0, 357);
            labelVer.Name = "labelVer";
            labelVer.Size = new Size(280, 23);
            labelVer.TabIndex = 11;
            labelVer.Text = "ver.";
            labelVer.TextAlign = ContentAlignment.MiddleRight;
            // 
            // avatarLogo
            // 
            avatarLogo.BackgroundImageLayout = ImageLayout.Zoom;
            avatarLogo.ImageSvg = resources.GetString("avatarLogo.ImageSvg");
            avatarLogo.Location = new Point(111, 72);
            avatarLogo.Name = "avatarLogo";
            avatarLogo.Size = new Size(59, 51);
            avatarLogo.TabIndex = 12;
            avatarLogo.Text = "";
            // 
            // LoginForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(280, 380);
            Controls.Add(avatarLogo);
            Controls.Add(labelVer);
            Controls.Add(btnClose);
            Controls.Add(inputNumPort);
            Controls.Add(switchDebug);
            Controls.Add(divider1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnStart);
            Controls.Add(panelLogo);
            Font = new Font("Microsoft YaHei UI", 12F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "LoginForm";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoginForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private AntdUI.Panel panelLogo;
        private AntdUI.Button btnStart;
        private AntdUI.Label label2;
        private AntdUI.Label label1;
        private AntdUI.Switch switchDebug;
        private AntdUI.InputNumber inputNumPort;
        private AntdUI.Divider divider1;
        private AntdUI.Button btnClose;
        private AntdUI.TooltipComponent tooltipClose;
        private AntdUI.Label labelVer;
        private AntdUI.Avatar avatarLogo;
    }
}