using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeChatFerry.WinForm.HttpApi
{
    public class UnifiedResponse<T> : IActionResult
    {
        public UnifiedResponse(HttpStatusCode code, string message = "", T? data = default)
        {
            Code = (int)code;
            if (string.IsNullOrEmpty(message))
                message = code.ToString();
            Message = message;
            Data = data;
        }
        public int Code { get; set; } = 0;
        public string? Message { get; set; } = "success";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = Code;
            var json = System.Text.Json.JsonSerializer.Serialize(this);
            return response.WriteAsync(json);
        }
    }
}
