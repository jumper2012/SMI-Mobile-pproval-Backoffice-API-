using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug {
    public interface IFauxUrlSlugGenerator {
        string GenerateFauxUrlSlugPart(string param, ContentFieldDefinition fieldDefinition);
    }
}