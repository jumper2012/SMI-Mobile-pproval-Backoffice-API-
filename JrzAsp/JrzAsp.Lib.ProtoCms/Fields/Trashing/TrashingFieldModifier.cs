using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingFieldModifier : IContentFieldModifier {

        public ContentModifierForm BuildModifierForm(ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var f = field.DirectCastTo<TrashingField>();
            var form = new TrashingFieldModifierForm {
                TrashedAt = f.TrashedUtc?.ToLocalTime()
            };
            return form;
        }

        public void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var fn = $"{fieldDefinition.FieldName}.{nameof(TrashingField.TrashedUtc)}";
            var fp = modifierForm.DirectCastTo<TrashingFieldModifierForm>();
            var trashedUtc = fp?.TrashedAt?.ToUniversalTime();
            var cf = content.ContentFields.FirstOrDefault(x => x.FieldName == fn);
            if (cf == null) {
                cf = new ProtoField {
                    FieldName = fn,
                    ContentId = content.Id
                };
                content.ContentFields.Add(cf);
            }
            cf.FieldClassTypeName = typeof(TrashingField).FullName;
            cf.DateTimeValue = trashedUtc;
        }

        public VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm,
            ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition) {
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-datetime",
                    Props = new {
                        label = "Trashed At",
                        valuePath = nameof(TrashingFieldModifierForm.TrashedAt)

                    }
                }
            };
        }
    }
}