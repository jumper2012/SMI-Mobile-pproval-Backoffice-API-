using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.SimpleField;

namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberField : SimpleContentField<decimal?> {

        public override ContentFieldSpec __FieldSpec =>
            new ContentFieldSpec(
                typeof(NumberFieldFinder),
                typeof(NumberFieldModifier),
                typeof(NumberFieldConfiguration)
            );
    }
}