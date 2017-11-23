using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Text {
    public class TextFieldModifier : IContentFieldModifier {
        public ContentModifierForm BuildModifierForm(ContentField field, ContentModifyOperation operation,
            ProtoContent content,
            ContentFieldDefinition fieldDefinition) {
            var cf = field.DirectCastTo<TextField>();
            var cfg = fieldDefinition.Config.DirectCastTo<TextFieldConfiguration>();
            var val = cf.Val;
            if (operation.IsCreateOperation()) val = cfg.InitialValue;
            var form = new TextFieldModifierForm {Val = val};
            return form;
        }

        public void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var f = modifierForm.DirectCastTo<TextFieldModifierForm>();
            var cfg = fieldDefinition.Config.DirectCastTo<TextFieldConfiguration>();
            var fn = $"{fieldDefinition.FieldName}.{nameof(f.Val)}";
            var cf = content.ContentFields.FirstOrDefault(x => x.FieldName == fn);
            if (cf == null) {
                cf = new ProtoField {
                    ContentId = content.Id,
                    FieldName = fn
                };
                content.ContentFields.Add(cf);
            }
            cf.FieldClassTypeName = typeof(TextField).FullName;
            cf.StringValue = string.IsNullOrWhiteSpace(f.Val) ? cfg.DefaultValue?.Trim() : f.Val;
        }

        public VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var cfg = fieldDefinition.Config.DirectCastTo<TextFieldConfiguration>();
            var editorVue = new VueComponentDefinition {
                Name = "cms-form-field-text",
                Props = new {
                    label = cfg.Label,
                    helpText = cfg.HelpText,
                    valuePath = nameof(TextFieldModifierForm.Val)
                }
            };
            switch (cfg.EditorType) {
                case TextFieldEditorType.TextArea:
                    editorVue = new VueComponentDefinition {
                        Name = "cms-form-field-textarea",
                        Props = new {
                            label = cfg.Label,
                            helpText = cfg.HelpText,
                            valuePath = nameof(TextFieldModifierForm.Val)
                        }
                    };
                    break;
                case TextFieldEditorType.RichHtml:
                    editorVue = new VueComponentDefinition {
                        Name = "cms-form-field-rich-html",
                        Props = new {
                            label = cfg.Label,
                            helpText = cfg.HelpText,
                            valuePath = nameof(TextFieldModifierForm.Val)
                        }
                    };
                    break;
            }
            return new[] {
                editorVue
            };
        }
    }
}