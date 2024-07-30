using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace WeChatFerry.WinForm.Helper
{
    public static class NLogHelper
    {
        public static ILogger<T> CreateLogger<T>()
        {
            var logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<T>();
            return logger;
        }
    }
}
