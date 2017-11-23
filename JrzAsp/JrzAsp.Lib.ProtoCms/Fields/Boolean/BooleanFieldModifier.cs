using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Boolean {
    public class BooleanFieldModifier : IContentFieldModifier {
        public ContentModifierForm BuildModifierForm(ContentField field, ContentModifyOperation operation,
            ProtoContent content,
            ContentFieldDefinition fieldDefinition) {
            var cfg = fieldDefinition.Config.DirectCastTo<BooleanFieldConfiguration>();
            var f = field.DirectCastTo<BooleanField>();
            var val = f?.Val;
            if (operation.IsCreateOperation()) val = cfg.InitialValue;
            var form = new BooleanFieldModifierForm {
                Val = val
            };
            return form;
        }

        public void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var cfg = fieldDefinition.Config.DirectCastTo<BooleanFieldConfiguration>();
            var form = modifierForm.DirectCastTo<BooleanFieldModifierForm>();
            var fn = $"{fieldDefinition.FieldName}.{nameof(BooleanField.Val)}";
            var cf = content.ContentFields.FirstOrDefault(x => x.FieldName == fn);
            if (cf == null) {
                cf = new ProtoField {
                    ContentId = content.Id,
                    FieldName = fn
                };
                content.ContentFields.Add(cf);
            }
            cf.FieldClassTypeName = typeof(BooleanField).FullName;
            cf.BooleanValue = form.Val ?? cfg.DefaultValue;
        }

        public VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var fcfg = fieldDefinition.Config.DirectCastTo<BooleanFieldConfiguration>();
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-checkbox",
                    Props = new {
                        label = fieldDefinition.Config.Label ?? fieldDefinition.FieldName,
                        valuePath = nameof(BooleanFieldModifierForm.Val),
                        yesLabel = fcfg.TrueBoolLabel,
                        noLabel = fcfg.FalseBoolLabel,
                        helpText = fcfg.HelpText,
                    }
                }
            };
        }
    }
}