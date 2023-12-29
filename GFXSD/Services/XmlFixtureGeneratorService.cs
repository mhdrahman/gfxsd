using AutoFixture;
using AutoFixture.Kernel;
using GFXSD.Extensions;
using GFXSD.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GFXSD.Services
{
    /// <summary>
    /// Implementation of <see cref="IXmlGenerationService"/> which uses the
    /// the xsd tool to generated a dll containing a C# class representation of the schema,
    /// fixture is used to populate the fields of the instance.
    /// </summary>
    public class XmlFixtureGeneratorService : IXmlGenerationService
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

                // Use the xsd.exe to generate the C# class from the schema
                var procStartInfo = new ProcessStartInfo
                {
                    FileName = Configuration.XsdToolPath,
                    Arguments = $"{inputFilePath} /c /o:{Configuration.DataDirectory}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                using var proc = Process.Start(procStartInfo);
                var err = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                if (!string.IsNullOrEmpty(err))
                {
                    throw new InvalidOperationException($"Error occured while generating the C# from the XSD file: {err}");
                }

                var generatedCSharp = File.ReadAllText(Path.Combine(Configuration.DataDirectory, $"{fileName}.cs"));

                // Compile the generated code and save the assembly to file
                var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
                var syntaxTree = CSharpSyntaxTree.ParseText(generatedCSharp);
                var references = AppDomain.CurrentDomain.GetAssemblies()
                                                        .Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                                                        .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

                var compilation = CSharpCompilation .Create(fileName)
                                                    .WithOptions(compilationOptions)
                                                    .AddReferences(references)
                                                    .AddSyntaxTrees(syntaxTree);

                var outputDllPath = Path.Combine(Configuration.DataDirectory, $"{fileName}.dll");
                var compilationResult = compilation.Emit(outputDllPath);

                if (!compilationResult.Success)
                {
                    throw new InvalidOperationException($"An error occured while compiling the generated C#: {JsonConvert.SerializeObject(compilationResult.Diagnostics)}");
                }

                // Load the assembly and create an instance of the generated class
                var matchingTypeName = Regex.Match(generatedCSharp, @"public\s+partial\s+class\s+(\w+)|public\s+class\s+(\w+)");
    #pragma warning disable S3885 // "Assembly.Load" should be used - need to load it from file.
                var loadedAssembly = Assembly.LoadFile(outputDllPath);
    #pragma warning restore S3885 // "Assembly.Load" should be used
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
            catch (Exception exception)
            {
                return ExceptionUtils.HandleGenerateException(exception);
            }
        }
    }
}
