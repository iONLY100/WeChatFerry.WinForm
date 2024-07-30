namespace WeChatFerry.WinForm.Controls
{
    partial class UserInfoControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserInfoControl));
            label3 = new AntdUI.Label();
            label4 = new AntdUI.Label();
            inputWxName = new AntdUI.Input();
            inputWxWxid = new AntdUI.Input();
            inputWxMobile = new AntdUI.Input();
            inputWxHome = new AntdUI.Input();
            avatar1 = new AntdUI.Avatar();
            SuspendLayout();
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.None;
            label3.AutoSizeMode = AntdUI.TAutoSize.Auto;
            label3.Location = new Point(16, 107);
            label3.Name = "label3";
            label3.Size = new Size(72, 23);
            label3.TabIndex = 2;
            label3.Text = "手机号：";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.None;
            label4.AutoSizeMode = AntdUI.TAutoSize.Auto;
            label4.Location = new Point(16, 147);
            label4.Name = "label4";
            label4.Size = new Size(88, 23);
            label4.TabIndex = 3;
            label4.Text = "微信路径：";
            // 
            // inputWxName
            // 
            inputWxName.Anchor = AnchorStyles.None;
            inputWxName.BackColor = Color.Transparent;
            inputWxName.BorderActive = Color.Transparent;
            inputWxName.BorderColor = Color.Transparent;
            inputWxName.BorderHover = Color.Transparent;
            inputWxName.BorderWidth = 0F;
            inputWxName.Location = new Point(97, 27);
            inputWxName.Name = "inputWxName";
            inputWxName.ReadOnly = true;
            inputWxName.Size = new Size(260, 23);
            inputWxName.SuffixSvg = "";
            inputWxName.TabIndex = 4;
            inputWxName.Text = "99999999999999";
            // 
            // inputWxWxid
            // 
            inputWxWxid.Anchor = AnchorStyles.None;
            inputWxWxid.BackColor = Color.Transparent;
            inputWxWxid.BorderActive = Color.Transparent;
            inputWxWxid.BorderColor = Color.Transparent;
            inputWxWxid.BorderHover = Color.Transparent;
            inputWxWxid.BorderWidth = 0F;
            inputWxWxid.Location = new Point(97, 67);
            inputWxWxid.Name = "inputWxWxid";
            inputWxWxid.ReadOnly = true;
            inputWxWxid.Size = new Size(260, 23);
            inputWxWxid.TabIndex = 5;
            // 
            // inputWxMobile
            // 
            inputWxMobile.Anchor = AnchorStyles.None;
            inputWxMobile.BackColor = Color.Transparent;
            inputWxMobile.BorderActive = Color.Transparent;
            inputWxMobile.BorderColor = Color.Transparent;
            inputWxMobile.BorderHover = Color.Transparent;
            inputWxMobile.BorderWidth = 0F;
            inputWxMobile.Location = new Point(97, 107);
            inputWxMobile.Name = "inputWxMobile";
            inputWxMobile.ReadOnly = true;
            inputWxMobile.Size = new Size(260, 23);
            inputWxMobile.TabIndex = 6;
            // 
            // inputWxHome
            // 
            inputWxHome.Anchor = AnchorStyles.None;
            inputWxHome.AutoScroll = true;
            inputWxHome.BackColor = Color.Transparent;
            inputWxHome.BorderActive = Color.Transparent;
            inputWxHome.BorderColor = Color.Transparent;
            inputWxHome.BorderHover = Color.Transparent;
            inputWxHome.BorderWidth = 0F;
            inputWxHome.Location = new Point(97, 147);
            inputWxHome.Name = "inputWxHome";
            inputWxHome.ReadOnly = true;
            inputWxHome.Size = new Size(260, 23);
            inputWxHome.TabIndex = 7;
            // 
            // avatar1
            // 
            avatar1.Anchor = AnchorStyles.None;
            avatar1.BorderColor = Color.Black;
            avatar1.BorderWidth = 1F;
            avatar1.Image = (Image)resources.GetObject("avatar1.Image");
            avatar1.ImageFit = AntdUI.TFit.Contain;
            avatar1.Location = new Point(16, 27);
            avatar1.Name = "avatar1";
            avatar1.Radius = 16;
            avatar1.Size = new Size(64, 64);
            avatar1.TabIndex = 8;
            avatar1.Text = "a";
            // 
            // UserInfoControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            Controls.Add(avatar1);
            Controls.Add(inputWxHome);
            Controls.Add(label3);
            Controls.Add(inputWxMobile);
            Controls.Add(label4);
            Controls.Add(inputWxWxid);
            Controls.Add(inputWxName);
            Font = new Font("Microsoft YaHei UI", 12F);
            Name = "UserInfoControl";
            Size = new Size(373, 197);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private AntdUI.Label label3;
        private AntdUI.Label label4;
        private AntdUI.Input inputWxName;
        private AntdUI.Input inputWxWxid;
        private AntdUI.Input inputWxMobile;
        private AntdUI.Input inputWxHome;
        private AntdUI.Avatar avatar1;
    }
}
