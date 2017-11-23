using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.Setting.Models;
using JrzAsp.Lib.ProtoCms.Setting.Services;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;

namespace JrzAsp.Lib.ProtoCms.Setting.API {
    [RoutePrefixProtoCms("dev-help")]
    public class SiteSettingDevHelpApiController : BaseProtoApiController {
        private readonly ISiteSettingManager _ssmgr;

        public SiteSettingDevHelpApiController(ISiteSettingManager ssmgr) {
            _ssmgr = ssmgr;
        }

        [Route("site-setting-specs")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<SiteSettingSpec>))]
        public IHttpActionResult GetSiteSettingSpecs() {
            return JsonProto(_ssmgr.SettingSpecs);
        }
    }
}