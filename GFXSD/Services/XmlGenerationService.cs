using AutoFixture;
using AutoFixture.Kernel;
using GFXSD.Extensions;
using GFXSD.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xml.XMLGen;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace GFXSD.Services
{
    public class XmlGenerationService
    {
        private const string DataDirectory = @"C:\ProgramData\GFXSD";
        private const string XsdToolPath = @"External\xsd.exe";

        public XmlGenerationService()
        {
            Directory.CreateDirectory(DataDirectory);
        }

        public XmlGenerationResult GenerateXmlFromSchema(string schema, XmlGenerationMode xmlGenerationMode)
        {
            switch (xmlGenerationMode)
            {
                case XmlGenerationMode.Microsoft:
                    return GenerateUsingXmlSampleGenerator(schema);

                default:
                case XmlGenerationMode.AutoFixture:
                    return GenerateUsingCodeGenerator(schema);
            }
        }

        private XmlGenerationResult GenerateUsingXmlSampleGenerator(string schema)
        {
            // Save the schema to file
            var fileName = Guid.NewGuid().ToString();
            var inputFilePath = Path.Combine(DataDirectory, $"{fileName}.xsd");
            File.WriteAllText(inputFilePath, schema);


            // Use the XmlSampleGenerator from Microsoft to generate the dummy XML on file
            var outputFilePath = Path.Combine(DataDirectory, $"{fileName}.xml");
            using (var textWriter = new XmlTextWriter(outputFilePath, null))
            {
                textWriter.Formatting = System.Xml.Formatting.Indented;
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

        public XmlGenerationResult GenerateUsingCodeGenerator(string schema)
        {
            // Save the schema to file
            var fileName = Guid.NewGuid().ToString();
            var inputFilePath = Path.Combine(DataDirectory, $"{fileName}.xsd");
            File.WriteAllText(inputFilePath, schema);

            // Use the xsd.exe to generate the C# class from the schema
            var procStartInfo = new ProcessStartInfo
            {
                FileName = XsdToolPath,
                Arguments = $"{inputFilePath} /c /o:{DataDirectory}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using var proc = Process.Start(procStartInfo);
            var output = proc.StandardOutput.ReadToEnd();
            var err = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (!string.IsNullOrEmpty(err))
            {
                throw new InvalidOperationException($"Error occured while generating the C# from the XSD file: {err}");
            }

            var generatedCSharp = File.ReadAllText(Path.Combine(DataDirectory, $"{fileName}.cs"));

            // Compile the generated code and save the assembly to file
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var syntaxTree = CSharpSyntaxTree.ParseText(generatedCSharp);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                                                    .Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                                                    .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

            var compilation = CSharpCompilation
                .Create(fileName)
                .WithOptions(compilationOptions)
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);

            var outputDllPath = Path.Combine(DataDirectory, $"{fileName}.dll");
            var compilationResult = compilation.Emit(outputDllPath);

            if (!compilationResult.Success)
            {
                throw new InvalidOperationException($"An error occured while compiling the generated C#: { JsonConvert.SerializeObject(compilationResult.Diagnostics)}");
            }

            // Load the assembly and create an instance of the generated class
            var matchingTypeName = Regex.Match(generatedCSharp, @"public\s+partial\s+class\s+(\w+)|public\s+class\s+(\w+)");
            var loadedAssembly = Assembly.LoadFile(outputDllPath);
            var instance = Activator.CreateInstance(loadedAssembly.GetType(matchingTypeName.Groups[1].Value));

            // Populate the instance with dummy data using AutoFixture
            var fixture = new Fixture();
            new AutoPropertiesCommand().Execute(instance, new SpecimenContext(fixture));

            return new XmlGenerationResult
            {
                Xml = instance.Serialize(),
                CSharp = generatedCSharp,
            };
        }

        public enum XmlGenerationMode
        {
            Microsoft,
            AutoFixture,
        }
    }
}
