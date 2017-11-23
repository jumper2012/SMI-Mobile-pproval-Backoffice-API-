using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentModifier : IAlwaysFreshDependency {
        ContentType ContentType { get; }
        IContentModifierHandler[] Handlers { get; }

        void Initialize(ContentType contentType);

        ProtoContent FindProtoContent(string contentId, string operationName);

        ContentModifyOperation FindModifyOperation(string operationName);

        IDictionary<string, ContentModifierForm> BuildModifierForm(string contentId, string operationName);

        IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm, string contentId, string operationName);

        FurtherValidationResult ValidateModifierForm(IDictionary<string, ContentModifierForm> modifierForm,
            string contentId, string operationName);

        void PerformModification(IDictionary<string, ContentModifierForm> modifierForm, string contentId,
            out ProtoContent modifiedContent, string operationName);
    }
}