using GFXSD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GFXSD.Extensions
{
    public static class XmlUtils
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

            return new CommandResult
            {
                Result = xElement.ToString(),
            };
        }

        public static string RemoveComments(this string xml)
        {
            try
            {
                var xElement = XElement.Parse(xml);
                xElement.DescendantNodes().OfType<XComment>().Remove();

                return xElement.ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }

        public static void UpdateMinAndMaxOccurs(this XDocument document)
        {
            var attributes = document.Descendants().SelectMany(el => el.Attributes().Where(attr => attr.Name.LocalName.ToLower() == "minoccurs" || attr.Name.LocalName.ToLower() == "maxoccurs"));

            foreach (var attribute in attributes)
            {
                if (attribute.Name.LocalName.ToLower() == "minoccurs")
                {
                    attribute.Value = "1";
                }

                if (attribute.Name.LocalName.ToLower() == "maxoccurs" && int.TryParse(attribute.Value, out var max) && max > 1)
                {
                    var minAttribute = attribute.Parent.Attributes().FirstOrDefault(_ => _.Name.LocalName.ToLower() == "minoccurs");
                    if (minAttribute != null)
                    {
                        minAttribute.Value = "3";
                    }
                }
            }
        }
    }
}
