using GFXSD.Extensions;
using GFXSD.Models;
using GFXSD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System;

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
        public ActionResult GenerateXmlFromSchema([FromBody] Schema schema)
        {
            try
            {
                return Ok(_xmlGenerationService.Generate(schema.Content));
            }
            catch (Exception e)
            {
                return Ok(new XmlGenerationResult { Xml = JsonConvert.SerializeObject(e) });
            }
        }

        [HttpPost]
        public ActionResult RemoveNodes([FromBody] RemoveNodesCommand command)
            => Ok(command.Xml.RemoveNodes(command.NodeName));
    }
}
