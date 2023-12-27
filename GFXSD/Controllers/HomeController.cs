using GFXSD.Commands;
using GFXSD.Models;
using GFXSD.Services;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult GenerateXmlFromSchema([FromBody] XmlSchema schema)
        {
            try
            {
                return Ok(_xmlGenerationService.Generate(schema.Content));
            }
            // TODO Handle the exceptions inside the method, probably want XmlGenerationResult to have multiple
            // types, each defining how to create it's Http response
            catch (Exception e)
            {
                return Ok(new XmlGenerationResult { Error = JsonConvert.SerializeObject(e) });
            }
        }

        [HttpPost]
        public ActionResult RemoveNodes([FromBody] RemoveNodesCommand command)
            => Ok(command.Execute());
    }
}
