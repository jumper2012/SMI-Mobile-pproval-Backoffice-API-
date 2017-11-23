using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoFieldModifier : IContentFieldModifier {

        public ContentModifierForm BuildModifierForm(ContentField field,
            ContentModifyOperation operation, ProtoContent content,
            ContentFieldDefinition fieldDefinition) {
            var cf = field.DirectCastTo<ChronoField>();
            var cfg = fieldDefinition.Config.DirectCastTo<ChronoFieldConfiguration>();
            var val = cf.Val;
            if (operation.IsCreateOperation()) val = cfg.InitialValue;
            var form = new ChronoFieldModifierForm {Val = val};
            return form;
        }

        public void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var f = modifierForm.DirectCastTo<ChronoFieldModifierForm>();
            var cfg = fieldDefinition.Config.DirectCastTo<ChronoFieldConfiguration>();
            var fn = $"{fieldDefinition.FieldName}.{nameof(ChronoField.Val)}";
            var cf = content.ContentFields.FirstOrDefault(x => x.FieldName == fn);
            if (cf == null) {
                cf = new ProtoField {
                    ContentId = content.Id,
                    FieldName = fn
                };
                content.ContentFields.Add(cf);
            }
            cf.FieldClassTypeName = typeof(ChronoField).FullName;
            cf.DateTimeValue = f?.Val ?? cfg.DefaultValue;
        }

        public VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm,
            ContentField field, ContentModifyOperation operation, ProtoContent content,
            ContentFieldDefinition fieldDefinition) {
            var fcfg = fieldDefinition.Config.DirectCastTo<ChronoFieldConfiguration>();
            var pickerKind = fcfg.PickerKind.ToString();
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-datetime",
                    Props = new {
                        label = $"{fcfg.Label}",
                        helpText = fcfg.HelpText,
                        valuePath = nameof(ChronoFieldModifierForm.Val),
                        pickerKind
                    }
                }
            };
        }
    }
}