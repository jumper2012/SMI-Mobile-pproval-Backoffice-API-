using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Publishing {
    public class PublishingFieldTableActionsHandler : IContentTableActionsHandler {
        public string[] HandledContentTypeIds => new[] {"*"};
        public decimal Priority => 0;

        public VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext, ContentType contentType) {
            return null;
        }

        public VueActionTrigger[] TableActionsForSingleContent(ProtoContent content, ProtoCmsRuntimeContext cmsContext,
            ContentType contentType) {
            var trigs = new List<VueActionTrigger>();
            if (contentType.IsModifyOperationAllowed(PublishingFieldModifyOperationsProvider
                .CHANGE_PUBLISH_STATUS_OPERATION_NAME)) {
                trigs.Add(new VueButton {
                    Label = "Publish/Unpublish",
                    IconCssClass = "fa fa-cog",
                    OnClick = $"protoCms.utils.popupEntityOperationForm('content', '{content.Id}', '{contentType.Id}', " +
                              $"'{PublishingFieldModifyOperationsProvider.CHANGE_PUBLISH_STATUS_OPERATION_NAME}', " +
                              $"'Publish/Unpublish', '{contentType.Name}')"
                });
            }
            return trigs.ToArray();
        }
    }
}