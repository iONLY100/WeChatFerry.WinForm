#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
namespace WeChatFerry.WinForm.Controls
{
    public partial class WebView2Control : UserControl
    {
        public sealed override DockStyle Dock
        {
            get => base.Dock;
            set => base.Dock = value;
        }
        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }
        public string? SourceUrl { get; set; }
        public WebView2Control()
        {
            InitializeComponent();
            this.Dock= DockStyle.Fill;
        }

        public WebView2Control(string url,string title)
        {
            SourceUrl= url;
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.Text = title;
            webView21.Source = new Uri(SourceUrl);
        }


    }
}
