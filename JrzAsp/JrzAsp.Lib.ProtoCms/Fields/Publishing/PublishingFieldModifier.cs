using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Publishing {
    public class PublishingFieldModifier : IContentFieldModifier {

        public ContentModifierForm BuildModifierForm(ContentField field,
            ContentModifyOperation operation, ProtoContent content,
            ContentFieldDefinition fieldDefinition) {
            var f = field != null ? field.DirectCastTo<PublishingField>() : new PublishingField();
            var form = new PublishingFieldModifierForm {
                PublishedAt = f.PublishedUtc?.ToLocalTime(),
                UnpublishedAt = f.UnpublishedUtc?.ToLocalTime()
            };
            return form;
        }

        public void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var pfn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.PublishedUtc)}";
            var ufn = $"{fieldDefinition.FieldName}.{nameof(PublishingField.UnpublishedUtc)}";

            var fp = modifierForm.DirectCastTo<PublishingFieldModifierForm>();

            var publishedUtc = fp?.PublishedAt?.ToUniversalTime();
            var pcf = content.ContentFields.FirstOrDefault(x => x.FieldName == pfn);
            if (pcf == null) {
                pcf = new ProtoField {
                    FieldName = pfn,
                    ContentId = content.Id
                };
                content.ContentFields.Add(pcf);
            }
            pcf.FieldClassTypeName = typeof(PublishingField).FullName;
            pcf.DateTimeValue = publishedUtc;

            var unpublishedUtc = fp?.UnpublishedAt?.ToUniversalTime();
            var ucf = content.ContentFields.FirstOrDefault(x => x.FieldName == ufn);
            if (ucf == null) {
                ucf = new ProtoField {
                    FieldName = ufn,
                    ContentId = content.Id
                };
                content.ContentFields.Add(ucf);
            }
            ucf.FieldClassTypeName = typeof(PublishingField).FullName;
            ucf.DateTimeValue = unpublishedUtc;
        }

        public VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm,
            ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var vues = new List<VueComponentDefinition> {
                new VueComponentDefinition {
                    Name = "cms-form-field-datetime",
                    Props = new {
                        label = "Published At",
                        valuePath = nameof(PublishingFieldModifierForm.PublishedAt)
                    }
                },
                new VueComponentDefinition {
                    Name = "cms-form-field-datetime",
                    Props = new {
                        label = "Unpublished At",
                        valuePath = nameof(PublishingFieldModifierForm.UnpublishedAt)
                    }
                }
            };
            return vues.ToArray();
        }
    }
}