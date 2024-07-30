namespace WeChatFerry.WinForm.Controls
{
    partial class ShowUpdateInfoControl
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
            timeline1 = new AntdUI.Timeline();
            SuspendLayout();
            // 
            // timeline1
            // 
            timeline1.Dock = DockStyle.Fill;
            timeline1.Location = new Point(50, 5);
            timeline1.Name = "timeline1";
            timeline1.Size = new Size(342, 384);
            timeline1.TabIndex = 0;
            timeline1.Text = "timeline1";
            // 
            // ShowUpdateInfoControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            Controls.Add(timeline1);
            Font = new Font("Microsoft YaHei UI", 12F);
            Name = "ShowUpdateInfoControl";
            Padding = new Padding(50, 5, 5, 5);
            Size = new Size(397, 394);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Timeline timeline1;
    }
}
