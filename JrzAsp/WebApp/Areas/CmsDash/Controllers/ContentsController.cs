using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Areas.CmsDash.Models;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Permissions;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Core.Permissions;
using JrzAsp.Lib.ProtoCms.Mvc.Security;

namespace WebApp.Areas.CmsDash.Controllers {
    [AuthorizeMvcByProtoPermission(AccessCmsPermission.PERMISSION_ID)]
    public class ContentsController : Controller {

        [HttpGet]
        public ActionResult Index(string id) {
            var rctx = ProtoCmsRuntimeContext.Current;
            var contentType = FindContentType(id);
            CheckUserHasPermissionToListContent(contentType, rctx);
            var mdl = new ContentsViewModel {
                ContentType = contentType
            };
            return View(mdl);
        }
        
        protected ContentType FindContentType(string contentTypeId) {
            var ct = ContentType.DefinedTypes.FirstOrDefault(x => x.Id == contentTypeId);
            if (ct == null) {
                throw new HttpException(404, $"ProtoCMS: no content type found with id '{contentTypeId}'.");
            }
            return ct;
        }

        protected void CheckUserHasPermissionToListContent(ContentType ct, ProtoCmsRuntimeContext rctx) {
            var viewPerm = new ListContentPermission(ct);
            if (!rctx.UserHasPermission(viewPerm.Id)) {
                throw new HttpException(403, $"ProtoCMS: user is forbidden to list content type '{ct.Id}'.");
            }
        }
    }
}