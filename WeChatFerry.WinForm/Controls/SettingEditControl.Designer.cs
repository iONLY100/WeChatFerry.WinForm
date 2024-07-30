namespace WeChatFerry.WinForm.Controls
{
    partial class SettingEditControl
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
            input1 = new AntdUI.Input();
            btnSave = new AntdUI.Button();
            btnSetDefault = new AntdUI.Button();
            SuspendLayout();
            // 
            // input1
            // 
            input1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            input1.Location = new Point(3, 3);
            input1.Multiline = true;
            input1.Name = "input1";
            input1.Size = new Size(552, 409);
            input1.TabIndex = 0;
            input1.Text = "input1";
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(446, 421);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(89, 44);
            btnSave.TabIndex = 1;
            btnSave.Text = "保存";
            btnSave.Type = AntdUI.TTypeMini.Success;
            btnSave.Click += btnSave_Click;
            // 
            // btnSetDefault
            // 
            btnSetDefault.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSetDefault.Ghost = true;
            btnSetDefault.Location = new Point(3, 421);
            btnSetDefault.Name = "btnSetDefault";
            btnSetDefault.Size = new Size(138, 44);
            btnSetDefault.TabIndex = 2;
            btnSetDefault.Text = "恢复默认设置";
            btnSetDefault.Click += btnSetDefault_Click;
            // 
            // SettingEditControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            Controls.Add(btnSetDefault);
            Controls.Add(btnSave);
            Controls.Add(input1);
            Font = new Font("Microsoft YaHei UI", 12F);
            Name = "SettingEditControl";
            Size = new Size(558, 476);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Input input1;
        private AntdUI.Button btnSave;
        private AntdUI.Button btnSetDefault;
    }
}
