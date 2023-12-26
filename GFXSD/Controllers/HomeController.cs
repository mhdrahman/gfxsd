using GFXSD.Extensions;
using GFXSD.Models;
using GFXSD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace GFXSD.Controllers
{
    public class HomeController : Controller
    {
        private readonly XmlGenerationService _xmlGenerationService;

        public HomeController(XmlGenerationService xmlGenerationService)
            => _xmlGenerationService = xmlGenerationService;

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpPost]
        public ActionResult GenerateXmlFromSchema([FromBody] Schema schema, [FromQuery] XmlGenerationMode xmlGenerationMode)
            => Ok(_xmlGenerationService.GenerateXmlFromSchema(schema.Content, xmlGenerationMode));

        [HttpPost]
        public ActionResult RemoveNodes([FromBody] RemoveNodesCommand command)
            => Ok(command.Xml.RemoveNodes(command.NodeName));
    }
}
