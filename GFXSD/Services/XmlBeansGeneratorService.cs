using GFXSD.Extensions;
using GFXSD.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GFXSD.Services
{
    public class XmlBeansGeneratorService : IXmlGenerationService
    {
        public XmlGenerationResult Generate(string schema)
        {
            // Save the schema to file
            var schemaXDoc = XDocument.Parse(schema);
            schemaXDoc.Descendants()
                      .SelectMany(descendant => descendant.Attributes().Where(attribute => attribute.Name.LocalName.ToLower() == "minoccurs" || attribute.Name.LocalName.ToLower() == "maxoccurs"))
                      .ToList()
                      .ForEach(Extensions.XmlUtils.UpdateMinAndMaxOccurs);

            var fileName = Guid.NewGuid().ToString();
            var inputFilePath = Path.Combine(Configuration.DataDirectory, $"{fileName}.xsd");
            schemaXDoc.Save(inputFilePath);

            var name = schemaXDoc.Root
                                 .Elements()
                                 .FirstOrDefault(_ => _.Name.LocalName.ToLower().Contains("element"))
                                 .Attribute("name").Value;

            // Use the xsd2inst to generate sample XML from the schema
            ProcessStartInfo procStartInfo;
            if (Configuration.IsLinux)
            {
                procStartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"{Configuration.Xsd2InstToolPath} {inputFilePath} -name {name}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
            }
            else
            {
                procStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {Configuration.Xsd2InstToolPath} {inputFilePath} -name {name}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
            }

            using var proc = Process.Start(procStartInfo);
            var output = proc.StandardOutput.ReadToEnd().Replace("XMLBEANS_LIB=/home/xmlbeans/xmlbeans-5.2.0/bin/../lib", string.Empty);
            var error = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            return new XmlGenerationResult
            {
                Xml = output.RemoveComments(),
                CSharp = null,
            };
        }
    }
}
