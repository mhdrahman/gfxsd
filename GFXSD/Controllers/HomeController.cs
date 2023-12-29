using GFXSD.Commands;
using GFXSD.Models;
using GFXSD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFXSD.Controllers
{
    public class HomeController : Controller
    {
        private readonly IXmlGenerationService _xmlGenerationService;

        public HomeController(IXmlGenerationService xmlGenerationService)
            => _xmlGenerationService = xmlGenerationService;

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpPost]
        [Authorize]
        public ActionResult GenerateXmlFromSchema([FromBody] XmlSchema schema)
            => Ok(_xmlGenerationService.Generate(schema.Content, schema.Root));

        [HttpPost]
        [Authorize]
        public ActionResult RemoveNodes([FromBody] RemoveNodesCommand command)
            => Ok(command.Execute());
    }
}
