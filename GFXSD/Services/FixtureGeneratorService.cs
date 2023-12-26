﻿using AutoFixture;
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
    public class FixtureGeneratorService : IXmlGenerationService
    {
        public XmlGenerationResult Generate(string schema)
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
            var output = proc.StandardOutput.ReadToEnd();
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
    }
}
