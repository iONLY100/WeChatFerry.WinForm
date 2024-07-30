using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatFerry.WinForm.HttpApi.Dtos
{
    public class TwoParamInputDto<T1,T2>
    {
        public required T1 Param1 { get; set; }
        public T2? Param2 { get; set; }
    }
}
