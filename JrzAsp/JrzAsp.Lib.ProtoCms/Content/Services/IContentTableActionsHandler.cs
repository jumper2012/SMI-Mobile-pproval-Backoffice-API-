using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentTableActionsHandler : IContentHandler {
        VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext, ContentType contentType);

        VueActionTrigger[] TableActionsForSingleContent(ProtoContent content, ProtoCmsRuntimeContext cmsContext,
            ContentType contentType);
    }
}