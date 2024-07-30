using AntdUI;
using Microsoft.Extensions.Logging;
using WeChatFerry.WinForm.Helper;

namespace WeChatFerry.WinForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            GlobalValue.Config=YamlConfigHelper.GetConfigByFilePath<ConfigModel>(GlobalValue.ConfigFilePath);
            ApplicationConfiguration.Initialize();
            ApplicationEventHandlerClass appEvents = new ApplicationEventHandlerClass();
            Application.ThreadException += appEvents.OnThreadException;
            AntdUI.Config.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(MainForm.Instance);
        }
        public class ApplicationEventHandlerClass
        {
            private readonly ILogger<ApplicationEventHandlerClass> _logger = NLogHelper.CreateLogger<ApplicationEventHandlerClass>();
            public void OnThreadException(object sender, ThreadExceptionEventArgs e)
            {
                _logger.LogError( e.Exception, "系统错误");
                new AntdUI.Modal.Config(MainForm.Instance, "系统错误", e.Exception.Message, icon: AntdUI.TType.Error)
                {
                    CancelText = null
                }.open();
            }
        }
    }
}