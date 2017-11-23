using JrzAsp.Lib.TextUtilities;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug;
using JrzAsp.Lib.ProtoCms.Fields.SimpleField;

namespace JrzAsp.Lib.ProtoCms.Fields.Text {
    public class TextField : SimpleContentField<string>, IFauxUrlSlugGenerator {
        public override ContentFieldSpec __FieldSpec =>
            new ContentFieldSpec(typeof(TextFieldFinder), typeof(TextFieldModifier), typeof(TextFieldConfiguration));

        public string GenerateFauxUrlSlugPart(string param, ContentFieldDefinition fieldDefinition)  {
            if (string.IsNullOrWhiteSpace(Val)) return null;
            return param == "lowercase" ? Val.ToUrlSlug().ToLowerInvariant() : Val.ToUrlSlug();
        }
    }
}