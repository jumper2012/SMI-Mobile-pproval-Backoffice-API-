using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentFieldModifier : IPerRequestDependency {
        ContentModifierForm BuildModifierForm(ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition);

        void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation, ProtoContent content, ContentFieldDefinition fieldDefinition);

        VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm,
            ContentField field, ContentModifyOperation operation, ProtoContent content,
            ContentFieldDefinition fieldDefinition);
    }
}