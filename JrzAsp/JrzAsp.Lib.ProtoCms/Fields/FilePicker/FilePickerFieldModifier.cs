using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.FilePicker {
    public class FilePickerFieldModifier : IContentFieldModifier {
        private readonly IProtoCmsDbContext _dbContext;
        private readonly IProtoCmsMainUrlsProvider _urlProv;

        public FilePickerFieldModifier(IProtoCmsDbContext dbContext, IProtoCmsMainUrlsProvider urlProv) {
            _dbContext = dbContext;
            _urlProv = urlProv;
        }

        public ContentModifierForm BuildModifierForm(ContentField field, ContentModifyOperation operation,
            ProtoContent content,
            ContentFieldDefinition fieldDefinition) {
            var fi = field.DirectCastTo<FilePickerField>();
            var fm = new FilePickerFieldModifierForm {
                Val = fi.Val
            };
            return fm;
        }

        public void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var fm = modifierForm.DirectCastTo<FilePickerFieldModifierForm>();
            var fnPrefix = $"{fieldDefinition.FieldName}.{nameof(FilePickerField.Val)}.";
            var existing = content.ContentFields.Where(x => x.FieldName.StartsWith(fnPrefix)).ToArray();
            foreach (var cfe in existing) {
                content.ContentFields.Remove(cfe);
                _dbContext.ProtoFields.Remove(cfe);
            }
            var idx = 0;
            foreach (var path in fm.Val) {
                var cf = new ProtoField {
                    ContentId = content.Id,
                    StringValue = path,
                    FieldName = $"{fnPrefix}{idx}",
                    FieldClassTypeName = typeof(FilePickerField).FullName
                };
                content.ContentFields.Add(cf);
                idx++;
            }
        }

        public VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var fcfg = fieldDefinition.Config.DirectCastTo<FilePickerFieldConfiguration>();
            return new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-file-picker",
                    Props = new {
                        label = fcfg.Label ?? fieldDefinition.FieldName,
                        valuePath = nameof(FilePickerFieldModifierForm.Val),
                        helpText = fcfg.HelpText,
                        isMultiSelect = fcfg.IsMultiSelect,
                        fileExplorerPageUrl = _urlProv.GenerateManageFileExplorerUrl()
                    }
                }
            };
        }
    }
}