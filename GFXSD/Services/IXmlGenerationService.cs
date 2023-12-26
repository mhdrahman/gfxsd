using GFXSD.Models;

namespace GFXSD.Services
{
    public interface IXmlGenerationService 
    {
        XmlGenerationResult Generate(string schema);
    }
}
