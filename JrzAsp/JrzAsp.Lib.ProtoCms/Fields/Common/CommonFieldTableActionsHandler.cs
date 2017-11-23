using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Common {
    public class CommonFieldTableActionsHandler : IContentTableActionsHandler {
        public string[] HandledContentTypeIds => new[] {"*"};
        public decimal Priority => 0;

        public VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext, ContentType contentType) {
            var trigs = new List<VueActionTrigger>();
            if (contentType.IsModifyOperationAllowed(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME)) {
                trigs.Add(new VueButton {
                    Label = $"Create New {contentType.Name}",
                    IconCssClass = "fa fa-plus",
                    OnClick = $"protoCms.utils.popupEntityOperationForm('content', null, '{contentType.Id}', " +
                              $"'{CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME}', " +
                              $"'Create {contentType.Name}', null)"
                });
            }
            return trigs.ToArray();
        }

        public VueActionTrigger[] TableActionsForSingleContent(ProtoContent content, ProtoCmsRuntimeContext cmsContext,
            ContentType contentType) {
            var trigs = new List<VueActionTrigger>();
            if (contentType.IsModifyOperationAllowed(CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME)) {
                trigs.Add(new VueButton {
                    Label = $"Update",
                    IconCssClass = "fa fa-pencil",
                    OnClick = $"protoCms.utils.popupEntityOperationForm('content', '{content.Id}', '{contentType.Id}', " +
                              $"'{CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME}', " +
                              $"'Update', '{contentType.Name}')"
                });
            }
            if (contentType.IsModifyOperationAllowed(CommonFieldModifyOperationsProvider.DELETE_OPERATION_NAME)) {
                trigs.Add(new VueButton {
                    Label = $"Delete",
                    IconCssClass = "fa fa-trash",
                    OnClick = $"protoCms.utils.popupEntityOperationForm('content', '{content.Id}', '{contentType.Id}', " +
                              $"'{CommonFieldModifyOperationsProvider.DELETE_OPERATION_NAME}', " +
                              $"'Delete', '{contentType.Name}')"
                });
            }
            return trigs.ToArray();
        }
    }
}