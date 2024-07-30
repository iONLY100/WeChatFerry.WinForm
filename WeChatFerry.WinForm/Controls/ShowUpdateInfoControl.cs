using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI;
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

namespace WeChatFerry.WinForm.Controls
{
    public partial class ShowUpdateInfoControl : UserControl
    {
        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public sealed override DockStyle Dock
        {
            get => base.Dock;
            set => base.Dock = value;
        }
        private static ShowUpdateInfoControl? _instance;
        public static ShowUpdateInfoControl Instance => _instance ??= new ShowUpdateInfoControl();

        public ShowUpdateInfoControl()
        {
            InitializeComponent();
            var dictionary = UpdateLogInfo.LogInfo;
            Dock = DockStyle.Fill;
            Text = "更新日志";
            var enumList = Enum.GetValues(typeof(AntdUI.TTypeMini)).Cast<AntdUI.TTypeMini>().ToList();
            enumList.Remove(TTypeMini.Info);
            var i = 0;
            foreach (var (key,value) in dictionary)
            {
                var timelineItem = new TimelineItem()
                {
                    Text = key,
                    Description = value,
                    Type = enumList[i % enumList.Count]
                };
                timeline1.Items.Add(timelineItem);
                i++;
            }
            
        }

        
    }
}
