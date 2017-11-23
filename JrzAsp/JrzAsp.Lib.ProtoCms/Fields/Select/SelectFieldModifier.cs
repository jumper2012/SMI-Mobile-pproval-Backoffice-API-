using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Select {
    public class SelectFieldModifier : IContentFieldModifier {
        private readonly IProtoCmsDbContext _dbContext;

        public SelectFieldModifier(IProtoCmsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public ContentModifierForm BuildModifierForm(ContentField field, ContentModifyOperation operation,
            ProtoContent content,
            ContentFieldDefinition fieldDefinition) {
            var cf = field.DirectCastTo<SelectField>();
            var val = cf.Val;
            var form = new SelectFieldModifierForm {Val = val};
            return form;
        }

        public void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var f = modifierForm.DirectCastTo<SelectFieldModifierForm>();
            var fnPrefix = $"{fieldDefinition.FieldName}.{nameof(SelectField.Val)}.";
            var existingCfs = content.ContentFields.Where(x => x.FieldName.StartsWith(fnPrefix))
                .ToArray();
            foreach (var excf in existingCfs) {
                content.ContentFields.Remove(excf);
                _dbContext.ProtoFields.Remove(excf);
            }
            var idx = 0;
            foreach (var fv in f.Val) {
                if (fv == null) continue;
                var cf = new ProtoField {
                    ContentId = content.Id,
                    FieldName = $"{fnPrefix}{idx}"
                };
                content.ContentFields.Add(cf);
                cf.FieldClassTypeName = typeof(SelectField).FullName;
                cf.StringValue = fv;
                idx++;
            }
        }

        public VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var cfg = fieldDefinition.Config.DirectCastTo<SelectFieldConfiguration>();
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-select",
                    Props = new {
                        label = fieldDefinition.Config.Label ?? fieldDefinition.FieldName,
                        valuePath = $"{nameof(SelectField.Val)}",
                        isMultiSelect = cfg.IsMultiSelect,
                        optionsHandlerId = cfg.OptionsHandlerId,
                        optionsHandlerParam = cfg.OptionsHandlerParam
                    }
                },
            };
        }
    }
}