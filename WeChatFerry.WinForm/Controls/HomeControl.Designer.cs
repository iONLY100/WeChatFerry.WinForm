namespace WeChatFerry.WinForm.Controls
{
    partial class HomeControl
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
            tabPage4 = new AntdUI.TabPage();
            tabPage5 = new AntdUI.TabPage();
            tabPage6 = new AntdUI.TabPage();
            tabPage2 = new AntdUI.TabPage();
            tabPage3 = new AntdUI.TabPage();
            panel5 = new AntdUI.Panel();
            switchListenPyq = new AntdUI.Switch();
            label3 = new AntdUI.Label();
            inputCallBackAddress = new AntdUI.Input();
            divider1 = new AntdUI.Divider();
            label6 = new AntdUI.Label();
            switchCallBack = new AntdUI.Switch();
            label7 = new AntdUI.Label();
            label8 = new AntdUI.Label();
            panel4 = new AntdUI.Panel();
            inputHttpApiPort = new AntdUI.InputNumber();
            divider2 = new AntdUI.Divider();
            label5 = new AntdUI.Label();
            switchHttpApi = new AntdUI.Switch();
            label2 = new AntdUI.Label();
            label1 = new AntdUI.Label();
            panel5.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // tabPage4
            // 
            tabPage4.Dock = DockStyle.Fill;
            tabPage4.Location = new Point(0, 0);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(0, 0);
            tabPage4.TabIndex = 0;
            tabPage4.Text = "tabPage4";
            // 
            // tabPage5
            // 
            tabPage5.Dock = DockStyle.Fill;
            tabPage5.Location = new Point(0, 0);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(0, 0);
            tabPage5.TabIndex = 0;
            tabPage5.Text = "tabPage5";
            // 
            // tabPage6
            // 
            tabPage6.Dock = DockStyle.Fill;
            tabPage6.Location = new Point(0, 0);
            tabPage6.Name = "tabPage6";
            tabPage6.Size = new Size(0, 0);
            tabPage6.TabIndex = 0;
            tabPage6.Text = "tabPage6";
            // 
            // tabPage2
            // 
            tabPage2.Dock = DockStyle.Fill;
            tabPage2.Location = new Point(0, 0);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(0, 0);
            tabPage2.TabIndex = 0;
            tabPage2.Text = "tabPage2";
            // 
            // tabPage3
            // 
            tabPage3.Dock = DockStyle.Fill;
            tabPage3.Location = new Point(0, 0);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(0, 0);
            tabPage3.TabIndex = 0;
            tabPage3.Text = "tabPage3";
            // 
            // panel5
            // 
            panel5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel5.Controls.Add(switchListenPyq);
            panel5.Controls.Add(label3);
            panel5.Controls.Add(inputCallBackAddress);
            panel5.Controls.Add(divider1);
            panel5.Controls.Add(label6);
            panel5.Controls.Add(switchCallBack);
            panel5.Controls.Add(label7);
            panel5.Controls.Add(label8);
            panel5.Location = new Point(35, 150);
            panel5.Name = "panel5";
            panel5.Shadow = 10;
            panel5.ShadowColor = Color.Black;
            panel5.ShadowOpacity = 0.6F;
            panel5.Size = new Size(730, 290);
            panel5.TabIndex = 18;
            panel5.Text = "panel5";
            // 
            // switchListenPyq
            // 
            switchListenPyq.AutoCheck = true;
            switchListenPyq.Location = new Point(298, 60);
            switchListenPyq.Name = "switchListenPyq";
            switchListenPyq.Size = new Size(45, 28);
            switchListenPyq.TabIndex = 15;
            switchListenPyq.Text = "switch1";
            // 
            // label3
            // 
            label3.AutoSizeMode = AntdUI.TAutoSize.Auto;
            label3.Font = new Font("Microsoft YaHei UI", 12F);
            label3.Location = new Point(207, 63);
            label3.Name = "label3";
            label3.Size = new Size(88, 23);
            label3.TabIndex = 16;
            label3.Text = "监听朋友圈";
            // 
            // inputCallBackAddress
            // 
            inputCallBackAddress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputCallBackAddress.AutoScroll = true;
            inputCallBackAddress.Location = new Point(92, 93);
            inputCallBackAddress.Multiline = true;
            inputCallBackAddress.Name = "inputCallBackAddress";
            inputCallBackAddress.Size = new Size(599, 165);
            inputCallBackAddress.TabIndex = 14;
            // 
            // divider1
            // 
            divider1.Dock = DockStyle.Top;
            divider1.Location = new Point(10, 46);
            divider1.Name = "divider1";
            divider1.Size = new Size(710, 5);
            divider1.TabIndex = 0;
            // 
            // label6
            // 
            label6.BackColor = Color.Transparent;
            label6.Dock = DockStyle.Top;
            label6.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label6.Location = new Point(10, 10);
            label6.Name = "label6";
            label6.Size = new Size(710, 36);
            label6.TabIndex = 1;
            label6.Text = "HTTP回调";
            // 
            // switchCallBack
            // 
            switchCallBack.AutoCheck = true;
            switchCallBack.Location = new Point(92, 60);
            switchCallBack.Name = "switchCallBack";
            switchCallBack.Size = new Size(45, 28);
            switchCallBack.TabIndex = 11;
            switchCallBack.Text = "switch1";
            switchCallBack.CheckedChanged += switchCallBack_CheckedChanged;
            // 
            // label7
            // 
            label7.AutoSizeMode = AntdUI.TAutoSize.Auto;
            label7.Font = new Font("Microsoft YaHei UI", 12F);
            label7.Location = new Point(47, 105);
            label7.Name = "label7";
            label7.Size = new Size(39, 23);
            label7.TabIndex = 13;
            label7.Text = "地址";
            // 
            // label8
            // 
            label8.AutoSizeMode = AntdUI.TAutoSize.Auto;
            label8.Font = new Font("Microsoft YaHei UI", 12F);
            label8.Location = new Point(47, 63);
            label8.Name = "label8";
            label8.Size = new Size(39, 23);
            label8.TabIndex = 12;
            label8.Text = "启用";
            // 
            // panel4
            // 
            panel4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel4.Controls.Add(inputHttpApiPort);
            panel4.Controls.Add(divider2);
            panel4.Controls.Add(label5);
            panel4.Controls.Add(switchHttpApi);
            panel4.Controls.Add(label2);
            panel4.Controls.Add(label1);
            panel4.Location = new Point(35, 20);
            panel4.Name = "panel4";
            panel4.Shadow = 10;
            panel4.ShadowColor = Color.Black;
            panel4.ShadowOpacity = 0.6F;
            panel4.Size = new Size(730, 130);
            panel4.TabIndex = 17;
            panel4.Text = "panel4";
            // 
            // inputHttpApiPort
            // 
            inputHttpApiPort.Font = new Font("Microsoft YaHei UI", 12F);
            inputHttpApiPort.Location = new Point(252, 62);
            inputHttpApiPort.Maximum = new decimal(new int[] { 49151, 0, 0, 0 });
            inputHttpApiPort.Minimum = new decimal(new int[] { 1024, 0, 0, 0 });
            inputHttpApiPort.Name = "inputHttpApiPort";
            inputHttpApiPort.ShowControl = false;
            inputHttpApiPort.Size = new Size(80, 35);
            inputHttpApiPort.TabIndex = 10;
            inputHttpApiPort.Text = "10089";
            inputHttpApiPort.Value = new decimal(new int[] { 10089, 0, 0, 0 });
            // 
            // divider2
            // 
            divider2.Dock = DockStyle.Top;
            divider2.Location = new Point(10, 46);
            divider2.Name = "divider2";
            divider2.Size = new Size(710, 5);
            divider2.TabIndex = 0;
            // 
            // label5
            // 
            label5.BackColor = Color.Transparent;
            label5.Dock = DockStyle.Top;
            label5.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label5.Location = new Point(10, 10);
            label5.Name = "label5";
            label5.Size = new Size(710, 36);
            label5.TabIndex = 1;
            label5.Text = "HTTPAPI";
            // 
            // switchHttpApi
            // 
            switchHttpApi.AutoCheck = true;
            switchHttpApi.Location = new Point(92, 65);
            switchHttpApi.Name = "switchHttpApi";
            switchHttpApi.Size = new Size(45, 28);
            switchHttpApi.TabIndex = 11;
            switchHttpApi.Text = "switch1";
            switchHttpApi.CheckedChanged += switchHttpApi_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSizeMode = AntdUI.TAutoSize.Auto;
            label2.Font = new Font("Microsoft YaHei UI", 12F);
            label2.Location = new Point(207, 68);
            label2.Name = "label2";
            label2.Size = new Size(39, 23);
            label2.TabIndex = 13;
            label2.Text = "端口";
            // 
            // label1
            // 
            label1.AutoSizeMode = AntdUI.TAutoSize.Auto;
            label1.Font = new Font("Microsoft YaHei UI", 12F);
            label1.Location = new Point(47, 68);
            label1.Name = "label1";
            label1.Size = new Size(39, 23);
            label1.TabIndex = 12;
            label1.Text = "启用";
            // 
            // HomeControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            Controls.Add(panel5);
            Controls.Add(panel4);
            Font = new Font("Microsoft YaHei UI", 12F);
            Name = "HomeControl";
            Size = new Size(800, 580);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private AntdUI.TabPage tabPage2;
        private AntdUI.TabPage tabPage3;
        private AntdUI.TabPage tabPage4;
        private AntdUI.TabPage tabPage5;
        private AntdUI.TabPage tabPage6;
        private AntdUI.Panel panel5;
        private AntdUI.Input inputCallBackAddress;
        private AntdUI.Divider divider1;
        private AntdUI.Label label6;
        private AntdUI.Switch switchCallBack;
        private AntdUI.Label label7;
        private AntdUI.Label label8;
        private AntdUI.Panel panel4;
        private AntdUI.InputNumber inputHttpApiPort;
        private AntdUI.Divider divider2;
        private AntdUI.Label label5;
        private AntdUI.Switch switchHttpApi;
        private AntdUI.Label label2;
        private AntdUI.Label label1;
        private AntdUI.Switch switchListenPyq;
        private AntdUI.Label label3;
    }
}
