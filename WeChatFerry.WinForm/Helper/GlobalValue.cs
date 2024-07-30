using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatFerry.WinForm.Helper
{
    public static class GlobalValue
    {
        public static string ConfigFilePath = Path.GetFullPath("config.yaml");
        public static ConfigModel Config { get; set; }= new ConfigModel();

        public static string IniDefaultStr = @"# 调试模式
debugModel : 1
# Wcf使用的端口号,监听消息的端口号为此端口号+1
wcfPort : 10086
# 启用HttpApi使用的端口号
httpApiPort : 10089
# Spy文件路径
spyFilePath : \\spy\\v39.2.4\\sdk.dll
# 消息回调调用的地址
callBackUrlList :
    - http://127.0.0.1:19001/api/app/testWeChat/weChatCallBackWxHelper
    - https://www.baidu.com";
    }
}
