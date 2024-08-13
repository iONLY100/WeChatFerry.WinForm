namespace WeChatFerry.WinForm
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            windowBar1 = new AntdUI.WindowBar();
            divider1 = new AntdUI.Divider();
            labelTitle = new AntdUI.Label();
            panel1 = new AntdUI.Panel();
            panelLeft = new AntdUI.Panel();
            btnSettingEdit = new AntdUI.Button();
            avatarLogo = new AntdUI.Avatar();
            btnApiDoc = new AntdUI.Button();
            btnHome = new AntdUI.Button();
            avatar = new AntdUI.Avatar();
            labelVer = new AntdUI.Label();
            panel2 = new AntdUI.Panel();
            labelRunTotalTime = new AntdUI.Label();
            notifyIcon1 = new NotifyIcon(components);
            windowBar1.SuspendLayout();
            panelLeft.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // windowBar1
            // 
            windowBar1.BackColor = Color.Transparent;
            windowBar1.Controls.Add(divider1);
            windowBar1.Controls.Add(labelTitle);
            windowBar1.Dock = DockStyle.Top;
            windowBar1.Location = new Point(60, 0);
            windowBar1.MaximizeBox = false;
            windowBar1.Name = "windowBar1";
            windowBar1.ShowIcon = false;
            windowBar1.Size = new Size(740, 40);
            windowBar1.TabIndex = 0;
            windowBar1.Text = " ";
            // 
            // divider1
            // 
            divider1.Dock = DockStyle.Bottom;
            divider1.Location = new Point(0, 39);
            divider1.Name = "divider1";
            divider1.Size = new Size(644, 1);
            divider1.TabIndex = 0;
            divider1.Thickness = 0.9F;
            // 
            // labelTitle
            // 
            labelTitle.Dock = DockStyle.Fill;
            labelTitle.Font = new Font("Microsoft YaHei UI", 14F);
            labelTitle.Location = new Point(0, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(644, 40);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Title";
            // 
            // panel1
            // 
            panel1.BorderColor = Color.Black;
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(60, 40);
            panel1.Name = "panel1";
            panel1.Size = new Size(740, 517);
            panel1.TabIndex = 5;
            panel1.Text = "panel1";
            // 
            // panelLeft
            // 
            panelLeft.Back = Color.FromArgb(242, 242, 242);
            panelLeft.Controls.Add(btnSettingEdit);
            panelLeft.Controls.Add(avatarLogo);
            panelLeft.Controls.Add(btnApiDoc);
            panelLeft.Controls.Add(btnHome);
            panelLeft.Controls.Add(avatar);
            panelLeft.Dock = DockStyle.Left;
            panelLeft.Location = new Point(0, 0);
            panelLeft.Margin = new Padding(0);
            panelLeft.Name = "panelLeft";
            panelLeft.Radius = 0;
            panelLeft.Size = new Size(60, 580);
            panelLeft.TabIndex = 2;
            panelLeft.Text = "panel3";
            // 
            // btnSettingEdit
            // 
            btnSettingEdit.Anchor = AnchorStyles.Bottom;
            btnSettingEdit.BackActive = Color.FromArgb(233, 233, 233);
            btnSettingEdit.BackColor = Color.FromArgb(242, 242, 242);
            btnSettingEdit.Font = new Font("Microsoft YaHei UI", 16F);
            btnSettingEdit.ForeColor = Color.Black;
            btnSettingEdit.ImageSvg = resources.GetString("btnSettingEdit.ImageSvg");
            btnSettingEdit.Location = new Point(10, 536);
            btnSettingEdit.Margin = new Padding(0);
            btnSettingEdit.Name = "btnSettingEdit";
            btnSettingEdit.Size = new Size(40, 40);
            btnSettingEdit.TabIndex = 3;
            btnSettingEdit.Type = AntdUI.TTypeMini.Primary;
            btnSettingEdit.Click += btnSettingEdit_Click;
            // 
            // avatarLogo
            // 
            avatarLogo.BackgroundImageLayout = ImageLayout.Zoom;
            avatarLogo.ImageSvg = resources.GetString("avatarLogo.ImageSvg");
            avatarLogo.Location = new Point(10, 8);
            avatarLogo.Name = "avatarLogo";
            avatarLogo.Size = new Size(40, 31);
            avatarLogo.TabIndex = 1;
            avatarLogo.Text = "";
            // 
            // btnApiDoc
            // 
            btnApiDoc.BackActive = Color.FromArgb(233, 233, 233);
            btnApiDoc.BackColor = Color.FromArgb(242, 242, 242);
            btnApiDoc.Font = new Font("Microsoft YaHei UI", 14F);
            btnApiDoc.ForeColor = Color.Black;
            btnApiDoc.ImageSvg = resources.GetString("btnApiDoc.ImageSvg");
            btnApiDoc.Location = new Point(10, 152);
            btnApiDoc.Margin = new Padding(0);
            btnApiDoc.Name = "btnApiDoc";
            btnApiDoc.Size = new Size(40, 40);
            btnApiDoc.TabIndex = 2;
            btnApiDoc.Type = AntdUI.TTypeMini.Primary;
            btnApiDoc.Click += btnApiDoc_Click;
            // 
            // btnHome
            // 
            btnHome.BackActive = Color.FromArgb(233, 233, 233);
            btnHome.BackColor = Color.FromArgb(242, 242, 242);
            btnHome.Font = new Font("Microsoft YaHei UI", 16F);
            btnHome.ForeColor = Color.Black;
            btnHome.ImageSvg = resources.GetString("btnHome.ImageSvg");
            btnHome.Location = new Point(10, 105);
            btnHome.Margin = new Padding(0);
            btnHome.Name = "btnHome";
            btnHome.Size = new Size(40, 40);
            btnHome.TabIndex = 1;
            btnHome.Type = AntdUI.TTypeMini.Primary;
            btnHome.Click += btnHome_Click;
            // 
            // avatar
            // 
            avatar.Anchor = AnchorStyles.Top;
            avatar.Image = (Image)resources.GetObject("avatar.Image");
            avatar.ImageFit = AntdUI.TFit.Contain;
            avatar.Location = new Point(14, 54);
            avatar.Name = "avatar";
            avatar.Round = true;
            avatar.Size = new Size(36, 36);
            avatar.TabIndex = 0;
            avatar.Text = "a";
            avatar.Click += avatar_Click;
            // 
            // labelVer
            // 
            labelVer.AutoSizeMode = AntdUI.TAutoSize.Auto;
            labelVer.BackColor = Color.Transparent;
            labelVer.Dock = DockStyle.Right;
            labelVer.Font = new Font("Microsoft YaHei UI", 12F);
            labelVer.Location = new Point(706, 0);
            labelVer.Name = "labelVer";
            labelVer.Size = new Size(34, 23);
            labelVer.TabIndex = 3;
            labelVer.Text = "ver.";
            labelVer.Click += labelVer_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(labelRunTotalTime);
            panel2.Controls.Add(labelVer);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(60, 557);
            panel2.Name = "panel2";
            panel2.Size = new Size(740, 23);
            panel2.TabIndex = 6;
            panel2.Text = "panel2";
            // 
            // labelRunTotalTime
            // 
            labelRunTotalTime.Dock = DockStyle.Left;
            labelRunTotalTime.Location = new Point(0, 0);
            labelRunTotalTime.Name = "labelRunTotalTime";
            labelRunTotalTime.Size = new Size(200, 23);
            labelRunTotalTime.TabIndex = 4;
            labelRunTotalTime.Text = "已运行：";
            // 
            // notifyIcon1
            // 
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "WeChatFerry";
            notifyIcon1.MouseClick += notifyIcon1_MouseClick;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(800, 580);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Controls.Add(windowBar1);
            Controls.Add(panelLeft);
            Font = new Font("Microsoft YaHei UI", 12F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(510, 520);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WeChatFerry.Winform";
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Shown += MainForm_Shown;
            windowBar1.ResumeLayout(false);
            panelLeft.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.WindowBar windowBar1;
        private AntdUI.Panel panel1;
        private AntdUI.Panel panelLeft;
        private AntdUI.Avatar avatarLogo;
        private AntdUI.Button btnApiDoc;
        private AntdUI.Button btnHome;
        private AntdUI.Avatar avatar;
        private AntdUI.Label labelTitle;
        private AntdUI.Label labelVer;
        private AntdUI.Panel panel2;
        private AntdUI.Label labelRunTotalTime;
        private AntdUI.Button btnSettingEdit;
        private AntdUI.Divider divider1;
        private NotifyIcon notifyIcon1;
    }
}
