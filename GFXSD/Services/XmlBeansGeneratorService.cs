﻿using GFXSD.Extensions;
using GFXSD.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public XmlGenerationResult Generate(string schema, string root)
        {
            try
            {
                // Save the schema to file
                var schemaXDoc = XDocument.Parse(schema);
                schemaXDoc.UpdateMinAndMaxOccurs();

                var fileName = Guid.NewGuid().ToString();
                var inputFilePath = Path.Combine(Configuration.DataDirectory, $"{fileName}.xsd");
                schemaXDoc.Save(inputFilePath);

                var name = string.IsNullOrEmpty(root)
                    ? schemaXDoc.Root.Elements().FirstOrDefault(_ => _.Name.LocalName.ToLower().Contains("element"))?.Attribute("name")?.Value
                    : root; 
                
                // Use the xsd2inst to generate sample XML from the schema
                var xsd2InstProcessStartInfo = new ProcessStartInfo
                {
                    FileName = Configuration.Terminal,
                    Arguments = $"{Configuration.Xsd2InstCommand} {inputFilePath} -name {name}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                using var xsd2InstProcess = Process.Start(xsd2InstProcessStartInfo);

                var output = xsd2InstProcess.StandardOutput.ReadToEnd().Replace("XMLBEANS_LIB=/home/xmlbeans/xmlbeans-5.2.0/bin/../lib", string.Empty);
                var error = xsd2InstProcess.StandardError.ReadToEnd();
                xsd2InstProcess.WaitForExit();

                return new XmlGenerationResult
                {
                    Xml = string.IsNullOrEmpty(output) ? null : output.RemoveComments(),
                    Error = error.IsLog4j2Error() ? null : error,
                    Root = name,
                };
            }
            catch (Exception exception)
            {
                return ExceptionUtils.HandleGenerateException(exception);
            }
        }
    }
}
