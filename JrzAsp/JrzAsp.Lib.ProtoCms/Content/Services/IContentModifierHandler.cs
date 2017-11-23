using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentModifierHandler : IContentHandler {
        IDictionary<string, ContentModifierForm> BuildModifierForm(ProtoContent content,
            ContentModifyOperation operation, ContentType contentType);

        IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(IDictionary<string, ContentModifierForm> modifierForm,
            ProtoContent content, ContentModifyOperation operation, ContentType contentType);

        FurtherValidationResult ValidateModifierForm(IDictionary<string, ContentModifierForm> modifierForm,
            ProtoContent content, ContentModifyOperation operation, ContentType contentType);

        void PerformModification(IDictionary<string, ContentModifierForm> modifierForm, ProtoContent content,
            ContentModifyOperation operation, ContentType contentType);
    }
}