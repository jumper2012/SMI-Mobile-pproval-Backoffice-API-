using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;

namespace JrzAsp.Lib.ProtoCms.Fields.Select.API {
    [RoutePrefixProtoCms("dev-help")]
    public class SelectFieldOptionsDevHelpApiController : BaseProtoApiController {
        private readonly ISelectFieldOptionsManager _sfoMgr;

        public SelectFieldOptionsDevHelpApiController(ISelectFieldOptionsManager sfoMgr) {
            _sfoMgr = sfoMgr;
        }

        [Route("select-field-options/handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ISelectFieldOptionsHandler>))]
        public IHttpActionResult GetOptionsHandlers() {
            var handlers = _sfoMgr.Handlers;
            return JsonProto(handlers);
        }
    }
}