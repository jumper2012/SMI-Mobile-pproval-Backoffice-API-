using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Areas.CmsDash.Models;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Core.Permissions;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Mvc.Security;

namespace WebApp.Areas.CmsDash.Controllers {
    [AuthorizeMvcByProtoPermission(AccessCmsPermission.PERMISSION_ID)]
    public class DataController : Controller {

        [HttpGet]
        public ActionResult Index(string id) {
            var rctx = ProtoCmsRuntimeContext.Current;
            var datumType = FindDatumType(id);
            CheckUserHasPermissionToListDatum(datumType, rctx);
            var mdl = new DataViewModel {
                DatumType = datumType
            };
            return View(mdl);
        }

        protected DatumType FindDatumType(string datumTypeId) {
            var ct = DatumType.DefinedTypes.FirstOrDefault(x => x.Id == datumTypeId);
            if (ct == null) {
                throw new HttpException(404, $"ProtoCMS: no datum type found with id '{datumTypeId}'.");
            }
            return ct;
        }

        protected void CheckUserHasPermissionToListDatum(DatumType dt, ProtoCmsRuntimeContext rctx) {
            if (!rctx.UserHasPermission(dt.ListPermissionBase.Id)) {
                throw new HttpException(403, $"ProtoCMS: user is forbidden to list datum type '{dt.Id}'.");
            }
        }
    }
}