using GFXSD.Extensions;
using GFXSD.Models;
using Microsoft.Xml.XMLGen;
using System;
using System.IO;
using System.Xml;

namespace GFXSD.Services
{
    /// <summary>
    /// Implementation of <see cref="IXmlGenerationService"/> which uses the
    /// XmlSampleGenerator dll to generate the sample XML.
    /// </summary>
#pragma warning disable S1133 // Deprecated code should be removed - Potentially may look into licensing for this and begin using it again.
    [Obsolete("Not sure about the licensing for this library. Marked as obsolete until have a concrete answer on it.")]
#pragma warning restore S1133 // Deprecated code should be removed
    public class XmlSampleGeneratorService : IXmlGenerationService
    {
        /// <inheritdoc/>
        public XmlGenerationResult Generate(string schema, string root)
        {
            try
            {
                // Save the schema to file
                var fileName = Guid.NewGuid().ToString();
                var inputFilePath = Path.Combine(Configuration.DataDirectory, $"{fileName}.xsd");
                File.WriteAllText(inputFilePath, schema);

                // Use the XmlSampleGenerator from Microsoft to generate the dummy XML on file
                var outputFilePath = Path.Combine(Configuration.DataDirectory, $"{fileName}.xml");
                using (var textWriter = new XmlTextWriter(outputFilePath, null) { Formatting = Formatting.Indented })
                {
                    var generator = new XmlSampleGenerator(inputFilePath, null)
                    {
                        MaxThreshold = 3,
                        ListLength = 3,
                    };

                    generator.WriteXml(textWriter);
                }

                return new XmlGenerationResult
                {
                    Xml = File.ReadAllText(outputFilePath),
                    CSharp = null,
                };
            }
            catch (Exception exception)
            {
                return ExceptionUtils.HandleGenerateException(exception);
            }
        }
    }
}
