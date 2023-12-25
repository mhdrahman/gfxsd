using GFXSD.Models;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
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

            using var stringWriter = new StringWriter();
            using var writer = XmlWriter.Create(stringWriter, xmlSettings);
            xmlserializer.Serialize(writer, value);

            return stringWriter.ToString();

        }

        public static CommandResult RemoveNodes(this string xml, string nodeName)
        {
            var xElement = XElement.Parse(xml);
            xElement.Descendants().Where(_ => _.Name == nodeName).Remove();
            xElement.DescendantNodes().OfType<XComment>().Remove();

            return new CommandResult
            {
                Result = xElement.ToString(),
            };
        }
    }
}
