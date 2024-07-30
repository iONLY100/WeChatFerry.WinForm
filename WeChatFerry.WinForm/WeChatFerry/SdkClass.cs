using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using WeChatFerry.WinForm.Helper;

namespace WeChatFerry.WinForm.WeChatFerry
{
    public class SdkClass
    {
        private delegate int WxInitSdkDelegate(bool debug, int port);
        private delegate int WxDestroySdkDelegate();

        private readonly WxDestroySdkDelegate _wxDestroySdk;
        private readonly WxInitSdkDelegate _wxInitSdk;
        private readonly ILogger<SdkClass> _logger;
        public SdkClass(string dllPath = "")
        {
            var path = Path.GetFullPath(dllPath);

            _logger = NLogHelper.CreateLogger<SdkClass>();
            if (string.IsNullOrWhiteSpace(dllPath))
            {
                dllPath = Environment.CurrentDirectory + "\\spy\\v39.2.4\\sdk.dll";
            }

            var pDll = NativeMethods.LoadLibrary(dllPath);
            if (pDll == nint.Zero)
            {
                _logger.LogError("Failed to load DLL.");
                throw new FileLoadException("Failed to load DLL.");
            }

            // 获取函数指针
            nint pInitSdk = NativeMethods.GetProcAddress(pDll, "WxInitSDK");
            nint pDestroySdk = NativeMethods.GetProcAddress(pDll, "WxDestroySDK");

            // 将函数指针转换为委托
            _wxInitSdk = Marshal.GetDelegateForFunctionPointer<WxInitSdkDelegate>(pInitSdk);
            _wxDestroySdk = Marshal.GetDelegateForFunctionPointer<WxDestroySdkDelegate>(pDestroySdk);
        }

        public int WxInitSdk(bool debug = true, int port = 10086)
        {
            var init = _wxInitSdk(debug, port);
            _logger.LogInformation($"WxInitSdk, return:{init}");
            return init;
        }

        public int WxDestroySdk()
        {
            var destroy = _wxDestroySdk();
            _logger.LogInformation($"WxDestroySdk, return:{destroy}");
            return destroy;
        }
    }
}
