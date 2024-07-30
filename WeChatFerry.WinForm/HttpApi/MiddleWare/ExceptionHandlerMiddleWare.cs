using Microsoft.AspNetCore.Http;
using System.Net;

// ReSharper disable ReplaceWithPrimaryConstructorParameter UnusedMember.Global

namespace WeChatFerry.WinForm.HttpApi.MiddleWare;

public class ExceptionHandlerMiddleWare(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        await WriteExceptionAsync(context, exception).ConfigureAwait(false);
    }

    private static async Task WriteExceptionAsync(HttpContext context, Exception exception)
    {
        //返回友好的提示
        var response = context.Response;

        //状态码
        if (exception is UnauthorizedAccessException)
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
        else
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

        response.ContentType = "application/json";
        await response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            code = 500, msg = exception.GetBaseException().Message
        }));
    }
}