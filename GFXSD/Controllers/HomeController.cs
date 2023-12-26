using GFXSD.Extensions;
using GFXSD.Models;
using GFXSD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using static GFXSD.Services.XmlGenerationService;

namespace GFXSD.Controllers
{
    public class HomeController : Controller
    {
        private readonly XmlGenerationService _xmlGenerationService;

        public HomeController(XmlGenerationService xmlGenerationService)
        {
            _xmlGenerationService = xmlGenerationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GenerateXmlFromSchema([FromBody] Schema schema, [FromQuery] bool useCodeGen = false)
        {
            // TODO: REVERT THIS
            return Ok("{\"xml\": \"Hello, World\"}");
            return Ok(_xmlGenerationService.GenerateXmlFromSchema(schema.Content, useCodeGen ? XmlGenerationMode.AutoFixture : XmlGenerationMode.XmlBeans));
        }

        [HttpPost]
        public ActionResult RemoveNodes([FromBody] RemoveNodesCommand command)
        {
            return Ok(command.Xml.RemoveNodes(command.NodeName));
        }
    }
}
