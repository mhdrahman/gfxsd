using GFXSD.Models;
using Microsoft.Xml.XMLGen;
using System;
using System.IO;
using System.Xml;

namespace GFXSD.Services
{
    public class XmlSampleGeneratorService : IXmlGenerationService
    {
        public XmlGenerationResult Generate(string schema)
        {
            // Save the schema to file
            var fileName = Guid.NewGuid().ToString();
            var inputFilePath = Path.Combine(Configuration.DataDirectory, $"{fileName}.xsd");
            File.WriteAllText(inputFilePath, schema);

            // Use the XmlSampleGenerator from Microsoft to generate the dummy XML on file
            var outputFilePath = Path.Combine(Configuration.DataDirectory, $"{fileName}.xml");
            using (var textWriter = new XmlTextWriter(outputFilePath, null))
            {
                textWriter.Formatting = Formatting.Indented;
                var xmlQualifiedName = new XmlQualifiedName("Root", "http://tempuri.org");

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
    }
}
