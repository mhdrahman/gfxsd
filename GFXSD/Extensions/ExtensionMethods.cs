using Newtonsoft.Json;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GFXSD.Extensions
{
    public static class ExtensionMethods
    {
        public static string Serialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var xmlserializer = new XmlSerializer(value.GetType());
            var xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                OmitXmlDeclaration = true,
            };

            var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter, xmlSettings))
            {
                xmlserializer.Serialize(writer, value);
                return stringWriter.ToString();
            }
        }
    }
}
