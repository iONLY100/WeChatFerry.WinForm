using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WeChatFerry.WinForm.Helper
{
    public static class FileHelper
    {
        public static string GetTempFileDataPath()
        {
            var exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            var savePath = Path.Combine(exeDirectory, "tmpFile");
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            return savePath;
        }

    }
}
