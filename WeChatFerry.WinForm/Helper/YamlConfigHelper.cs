using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace WeChatFerry.WinForm.Helper
{
    public class YamlConfigHelper
    {
        public static T GetConfigByFilePath<T>(string filePath)
        {
            var text = File.ReadAllText(filePath);
            return GetConfigByStr<T>(text);
        }
        public static T GetConfigByStr<T>(string text)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<T>(text);
        }
    }
}
