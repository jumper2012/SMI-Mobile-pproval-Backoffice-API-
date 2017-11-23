using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberFieldModifier : IContentFieldModifier {
        public ContentModifierForm BuildModifierForm(ContentField field, ContentModifyOperation operation,
            ProtoContent content,
            ContentFieldDefinition fieldDefinition) {
            var cfg = fieldDefinition.Config.DirectCastTo<NumberFieldConfiguration>();
            var cf = field.DirectCastTo<NumberField>();
            var val = cf?.Val;
            if (operation.IsCreateOperation()) val = cfg.InitialValue;
            var form = new NumberFieldModifierForm {
                Val = val
            };
            return form;
        }

        public void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var cfg = fieldDefinition.Config.DirectCastTo<NumberFieldConfiguration>();
            var form = modifierForm.DirectCastTo<NumberFieldModifierForm>();
            var fn = $"{fieldDefinition.FieldName}.{nameof(form.Val)}";
            var cf = content.ContentFields.FirstOrDefault(x => x.FieldName == fn);
            if (cf == null) {
                cf = new ProtoField {
                    ContentId = content.Id,
                    FieldName = fn
                };
                content.ContentFields.Add(cf);
            }
            cf.FieldClassTypeName = typeof(NumberField).FullName;
            cf.NumberValue = form.Val ?? cfg.DefaultValue;
        }

        public VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var cfg = fieldDefinition.Config.DirectCastTo<NumberFieldConfiguration>();
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-number",
                    Props = new {
                        label = cfg.Label ?? fieldDefinition.FieldName,
                        valuePath = nameof(NumberFieldModifierForm.Val),
                        numberKind = cfg.NumberKind.ToString().ToLowerInvariant(),
                        helpText = cfg.HelpText,
                    }
                }
            };
        }
    }
}