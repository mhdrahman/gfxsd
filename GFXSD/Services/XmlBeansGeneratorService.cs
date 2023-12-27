using GFXSD.Extensions;
using GFXSD.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml;
using System.Xml.Linq;

namespace GFXSD.Services
{
    /// <summary>
    /// Implementation of <see cref="IXmlGenerationService"/> which uses the
    /// XmlBeans Java tool to generate the sample XML.
    /// </summary>
    public class XmlBeansGeneratorService : IXmlGenerationService
    {
        /// <inheritdoc/>
        public XmlGenerationResult Generate(string schema)
        {
            try
            {
                // Save the schema to file
                var schemaXDoc = XDocument.Parse(schema);
                schemaXDoc.UpdateMinAndMaxOccurs();

                var fileName = Guid.NewGuid().ToString();
                var inputFilePath = Path.Combine(Configuration.DataDirectory, $"{fileName}.xsd");
                schemaXDoc.Save(inputFilePath);

                // TODO: This doeesn't feel very robust - should probably be something you can pass in
                // and if not passed in - default to trying to find it like so
                var name = schemaXDoc.Root
                                     .Elements()
                                     .FirstOrDefault(_ => _.Name.LocalName.ToLower().Contains("element"))
                                     .Attribute("name").Value;

                // Use the xsd2inst to generate sample XML from the schema
                var xsd2InstProcessStartInfo = new ProcessStartInfo
                {
                    FileName = Configuration.Terminal,
                    Arguments = $"{Configuration.Xsd2InstToolPath} {inputFilePath} -name {name}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                using var xsd2InstProcess = Process.Start(xsd2InstProcessStartInfo);

                // TODO: There has to be a better way to do this
                var output = xsd2InstProcess.StandardOutput.ReadToEnd().Replace("XMLBEANS_LIB=/home/xmlbeans/xmlbeans-5.2.0/bin/../lib", string.Empty);
                var error = xsd2InstProcess.StandardError.ReadToEnd();
                xsd2InstProcess.WaitForExit();

                return new XmlGenerationResult
                {
                    Xml = output.RemoveComments(),
                    Error = error.IsLog4j2Error() ? null : error,
                };
            }
            catch (XmlException)
            {
                return new XmlGenerationResult
                {
                    Error = "An error occured while processing the XML. Please check that you have provided a well formed XML schema.",
                };
            }
        }
    }
}
