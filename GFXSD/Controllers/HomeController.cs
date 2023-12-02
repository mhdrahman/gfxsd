using AutoFixture;
using AutoFixture.Kernel;
using GFXSD.Extensions;
using GFXSD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xml.XMLGen;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GFXSD.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public  ActionResult GenerateXmlFromSchema([FromBody] Schema schema)
        {
            // TODO move all the logic into XmlGenerationService
            const string directory = "C:/ProgramData/GFXSD";
            const string xsdToolPath = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\xsd.exe";

            var fileName = Guid.NewGuid().ToString();
            var inputFilePath = Path.Combine(directory, $"{fileName}.xsd");
            Directory.CreateDirectory(directory);
            System.IO.File.WriteAllText(inputFilePath, schema.Content);

            var procStartInfo = new ProcessStartInfo
            {
                FileName = xsdToolPath,
                Arguments = $"{inputFilePath} /c /o:{directory}",
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
                var errorResult = new XmlGenerationResult
                {
                    Error = err,
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResult);
            }

            var generatedCSharp = System.IO.File.ReadAllText(Path.Combine(directory, $"{fileName}.cs"));

            // Compile the code and save the assembly to a file
            var compilation = CSharpCompilation
                .Create(fileName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(AppDomain.CurrentDomain.GetAssemblies()
                                                      .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                                      .Select(a => MetadataReference.CreateFromFile(a.Location)))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(generatedCSharp));

            var dllPath = Path.Combine(directory, $"{fileName}.dll");
            var result = compilation.Emit(dllPath);
            if (!result.Success)
            {
                var errorResult = new XmlGenerationResult
                {
                    Error = "An error occured while compiling the generated C#",
                    CSharp = generatedCSharp,
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResult);
            }

            // TODO Check if this reliably creates the correct root element
            var typeNamePattern = @"public\s+partial\s+class\s+(\w+)|public\s+class\s+(\w+)";
            var match = Regex.Match(generatedCSharp, typeNamePattern);

            var loadedAssembly = Assembly.LoadFile(dllPath);
            var instance = Activator.CreateInstance(loadedAssembly.GetType(match.Groups[1].Value));

            var fixture = new Fixture();
            new AutoPropertiesCommand().Execute(instance, new SpecimenContext(fixture));

            var sucessResult = new XmlGenerationResult
            {
                CSharp = generatedCSharp,
                Xml = instance.Serialize(),
            };

            return Ok(sucessResult);
        }

        [HttpPost]
        public ActionResult GenerateXmlFromSchemaV2([FromBody] Schema schema)
        {
            // TODO move all the logic into XmlGenerationService
            const string directory = "C:/ProgramData/GFXSD";

            var fileName = Guid.NewGuid().ToString();
            var inputFilePath = Path.Combine(directory, $"{fileName}.xsd");
            Directory.CreateDirectory(directory);
            System.IO.File.WriteAllText(inputFilePath, schema.Content);

            var outputFilePath = Path.Combine(directory, $"{fileName}.xml");
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

            return Ok(new XmlGenerationResult { Xml = System.IO.File.ReadAllText(outputFilePath) });
        }

        [HttpPost]
        public ActionResult RemoveNodes([FromBody] RemoveNodesCommand command)
        {
            var xElement = XElement.Parse(command.Xml);
            xElement.Descendants().Where(_ => _.Name == command.NodeName).Remove();

            return Ok(new RemoveNodesCommand { Xml = xElement.ToString() });
        }
    }
}
