using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.SimpleField;

namespace JrzAsp.Lib.ProtoCms.Fields.Boolean {
    public class BooleanField : SimpleContentField<bool?> {
        public override ContentFieldSpec __FieldSpec =>
            new ContentFieldSpec(
                typeof(BooleanFieldFinder),
                typeof(BooleanFieldModifier),
                typeof(BooleanFieldConfiguration)
            );
    }
}