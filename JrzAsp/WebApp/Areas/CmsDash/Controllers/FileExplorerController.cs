using System.Web.Mvc;
using JrzAsp.Lib.ProtoCms.FileSys.Permissions;
using JrzAsp.Lib.ProtoCms.Mvc.Security;

namespace WebApp.Areas.CmsDash.Controllers {
    [AuthorizeMvcByProtoPermission(ManageFileExplorerPermission.PERMISSION_ID)]
    public class FileExplorerController : Controller {
        [HttpGet]
        public ActionResult Index() {
            return View();
        }
    }
}