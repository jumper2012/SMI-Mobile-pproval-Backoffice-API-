using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingFieldTableActionsHandler : IContentTableActionsHandler {
        public string[] HandledContentTypeIds => new[] {"*"};
        public decimal Priority => 0;

        public VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext, ContentType contentType) {
            return null;
        }

        public VueActionTrigger[] TableActionsForSingleContent(ProtoContent content, ProtoCmsRuntimeContext cmsContext,
            ContentType contentType) {
            var trigs = new List<VueActionTrigger>();
            if (contentType.IsModifyOperationAllowed(
                TrashingFieldModifyOperationsProvider.CHANGE_TRASH_STATUS_OPERATION_NAME)) {
                trigs.Add(new VueButton {
                    Label = "Trash/Untrash",
                    IconCssClass = "fa fa-trash-o",
                    OnClick = $"protoCms.utils.popupEntityOperationForm('content', '{content.Id}', '{contentType.Id}', " +
                              $"'{TrashingFieldModifyOperationsProvider.CHANGE_TRASH_STATUS_OPERATION_NAME}', " +
                              $"'Trash/Untrash', '{contentType.Name}')"
                });
            }
            return trigs.ToArray();
        }
    }
}