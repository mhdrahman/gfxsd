using GFXSD.Models;
using Newtonsoft.Json;
using System;

namespace GFXSD.Services
{
    public static class ExceptionHandler
    {
        public static XmlGenerationResult Handle(Exception exception)
            => new XmlGenerationResult { Error = JsonConvert.SerializeObject(exception) };
    }
}
