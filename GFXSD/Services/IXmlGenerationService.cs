using GFXSD.Models;

namespace GFXSD.Services
{
    /// <summary>
    /// Interface implemented by classes capable of generating sample XML for a given schema.
    /// </summary>
    public interface IXmlGenerationService 
    {
        /// <summary>
        /// Generates sample XML for the given <paramref name="schema"/>.
        /// </summary>
        /// <param name="schema">The schema for which the sample XML should be generated.</param>
        /// <param name="root">The name of the root element.</param>
        /// <returns>Sample XML for the given <paramref name="schema"/>.</returns>
        XmlGenerationResult Generate(string schema, string root);
    }
}
