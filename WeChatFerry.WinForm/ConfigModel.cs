using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatFerry.WinForm
{
    public class ConfigModel
    {
        public int DebugModel { get; set; } = 1;
        public int WcfPort { get; set; } = 10086;
        public int HttpApiPort { get; set; } = 10089;
        public string SpyFilePath { get; set; } = "";
        public List<string> CallBackUrlList { get; set; }= new List<string>();
    }
}
