using System.Web;
using System.Web.Mvc;
using WebApp.Areas.CmsDash.Models;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Core.Permissions;
using JrzAsp.Lib.ProtoCms.Mvc.Security;
using JrzAsp.Lib.ProtoCms.Setting.Permissions;
using JrzAsp.Lib.ProtoCms.Setting.Services;

namespace WebApp.Areas.CmsDash.Controllers {
    [AuthorizeMvcByProtoPermission(AccessCmsPermission.PERMISSION_ID)]
    public class SettingsController : Controller {
        private readonly ISiteSettingManager _stgMgr;
        public SettingsController(ISiteSettingManager stgMgr) {
            _stgMgr = stgMgr;
        }

        [HttpGet]
        public ActionResult Index(string id) {
            var rctx = ProtoCmsRuntimeContext.Current;
            CheckUserCanModifySetting(rctx, id);
            var mdl = new SettingViewModel {
                SettingId = id,
                SettingManager = _stgMgr
            };
            return View(mdl);
        }

        protected void CheckUserCanModifySetting(ProtoCmsRuntimeContext rctx, string settingId) {
            if (!rctx.UserHasPermission(ModifySiteSettingPermission.GetIdFor(settingId))) {
                throw new HttpException(403,
                    $"ProtoCMS: user has no permission to modify setting '{settingId}'.");
            }
        }
    }
}