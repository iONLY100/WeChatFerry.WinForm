using AntdUI;
using nng;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeChatFerry.WinForm.Helper;
using static System.Net.Mime.MediaTypeNames;

namespace WeChatFerry.WinForm.Controls
{
    public partial class SettingEditControl : UserControl
    {
        public SettingEditControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }
        public SettingEditControl(string title, string settingStr)
        {
            InitializeComponent();
            this.Text = title;
            this.Dock = DockStyle.Fill;
            input1.Text = settingStr;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                YamlConfigHelper.GetConfigByStr<ConfigModel>(this.input1.Text);

                using (StreamWriter writer = new StreamWriter(GlobalValue.ConfigFilePath, false)) // false 表示不追加
                {
                    writer.Write(this.input1.Text); // 写入文本
                }

                new Modal.Config(this.ParentForm, "成功", "保存成功,点击确定立即重启", icon: TType.Success)
                {
                    OnOk = _ =>
                    {
                        MainForm.Instance.Sdk.WxDestroySdk();
                        System.Windows.Forms.Application.ExitThread();
                        Restart();
                        return true;
                    }
                }.open();
            }
            catch (Exception ex)
            {
                new Modal.Config(this.ParentForm, "校验未通过", ex.Message, icon: TType.Error)
                {
                    CancelText = null
                }.open();
            }
        }
        private void Restart()
        {
            Thread thread = new Thread(obj =>
            {
                Process ps = new Process();
                ps.StartInfo.FileName = obj.ToString();
                ps.Start();
            });
            object appName = System.Windows.Forms.Application.ExecutablePath;
            Thread.Sleep(1000);
            thread.Start(appName);
            LoginForm.ExitApp();
        }
        

        private void btnSetDefault_Click(object sender, EventArgs e)
        {
            input1.Text = GlobalValue.IniDefaultStr;
        }
    }
}
