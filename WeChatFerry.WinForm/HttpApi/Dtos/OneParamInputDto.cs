using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatFerry.WinForm.HttpApi.Dtos
{
    public class OneParamInputDto<T>
    {
        public required T Param { get; set; }
    }
}
