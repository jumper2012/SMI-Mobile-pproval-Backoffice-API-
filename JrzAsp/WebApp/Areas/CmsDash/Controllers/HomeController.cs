using System.Web.Mvc;
using WebApp.Areas.CmsDash.Models;
using JrzAsp.Lib.ProtoCms.Core.Permissions;
using JrzAsp.Lib.ProtoCms.Mvc.Security;
using JrzAsp.Lib.ProtoCms.Setting.CoreSettings.WelcomeCard;
using JrzAsp.Lib.ProtoCms.Setting.Services;

namespace WebApp.Areas.CmsDash.Controllers {
    [AuthorizeMvcByProtoPermission(AccessCmsPermission.PERMISSION_ID)]
    public class HomeController : Controller {
        private readonly ISiteSettingManager _stgMgr;
        public HomeController(ISiteSettingManager stgMgr) {
            _stgMgr = stgMgr;
        }

        [HttpGet]
        public ActionResult Index() {
            var wcs = _stgMgr.GetSetting<WelcomeCardSetting>();
            var mdl = new HomeViewModel {
                WelcomeCardSetting = wcs
            };
            return View(mdl);
        }
    }
}