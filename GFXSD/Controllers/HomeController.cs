using GFXSD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;

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
        public ActionResult GenerateXmlFromSchema([FromBody] Schema schema)
        {
            // TODO move all the logic into XmlGenerationService
            const string directory = "C:/ProgramData/GFXSD/Schemas";
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
                var result = new XmlGenerationResult
                {
                    Error = err,
                };

                return StatusCode(StatusCodes.Status500InternalServerError, err);
            }

            var generatedCSharp = System.IO.File.ReadAllText(Path.Combine(directory, $"{fileName}.cs"));

            var sucessResult = new XmlGenerationResult
            {
                CSharp = generatedCSharp,
                Xml = null,
            };

            return Ok(sucessResult);
        }
    }
}
