using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatFerry.WinForm.HttpApi.Dtos
{
    public class DownloadImageInputDto
    {
        public ulong Id { get; set; }
        public string Extra { get; set; } = "";
        public int Timeout { get; set; } = 30;
    }
}
