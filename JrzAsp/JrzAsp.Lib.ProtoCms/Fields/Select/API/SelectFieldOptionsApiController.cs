using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.Core.Permissions;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;
using JrzAsp.Lib.ProtoCms.WebApi.Security;

namespace JrzAsp.Lib.ProtoCms.Fields.Select.API {

    [AuthorizeByProtoPermissions(AccessCmsPermission.PERMISSION_ID)]
    [RoutePrefixProtoCms("select-field-options")]
    public class SelectFieldOptionsApiController : BaseProtoApiController {

        private readonly ISelectFieldOptionsManager _sfoMgr;

        public SelectFieldOptionsApiController(ISelectFieldOptionsManager sfoMgr) {
            _sfoMgr = sfoMgr;
        }

        [Route("{handlerId}/get-all")]
        [HttpPost]
        [ResponseType(typeof(SelectFieldOptionsGetAllApiResult))]
        public IHttpActionResult GetAll([FromBody] SelectFieldOptionGetAllApiRequest req, string handlerId) {
            req.Page = req.Page >= 1 ? req.Page : 1;
            req.Limit = req.Limit >= 1 ? req.Limit : 30;
            try {
                var handler = _sfoMgr.GetHandler(handlerId);
                if (handler == null) {
                    throw new HttpException(404,
                        $"ProtoCMS: no select field options handler found with id '{handlerId}'.");
                }
                var result =
                    new SelectFieldOptionsGetAllApiResult(handler, req.Search, req.Page, req.Limit, req.HandlerParam);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{handlerId}/get-display")]
        [HttpPost]
        [ResponseType(typeof(SelectFieldOption))]
        public IHttpActionResult GetDisplay([FromBody] SelectFieldOptionGetDisplayApiRequest req, string handlerId) {
            try {
                if (!ModelState.IsValid) {
                    throw new HttpException(400,
                        $"ProtoCMS: {string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).SelectMany(x => string.Join(" | ", x.ErrorMessage, x.Exception?.Message)))}");
                }
                var handler = _sfoMgr.GetHandler(handlerId);
                if (handler == null) {
                    throw new HttpException(404,
                        $"ProtoCMS: no select field options handler found with id '{handlerId}'.");
                }
                var optionValue = req.OptionValue;
                var optObj = handler.GetOptionObject(optionValue, req.HandlerParam);
                if (optObj == null) return JsonProto<SelectFieldOption>(null);
                var result = handler.GetOptionDisplay(optObj, req.HandlerParam);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{handlerId}/get-display-multi")]
        [HttpPost]
        [ResponseType(typeof(IEnumerable<SelectFieldOption>))]
        public IHttpActionResult GetDisplayMulti([FromBody] SelectFieldOptionGetDisplayMultiApiRequest req,
            string handlerId) {
            try {
                if (!ModelState.IsValid) {
                    throw new HttpException(400,
                        $"ProtoCMS: {string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).SelectMany(x => string.Join(" | ", x.ErrorMessage, x.Exception?.Message)))}");
                }
                var handler = _sfoMgr.GetHandler(handlerId);
                if (handler == null) {
                    throw new HttpException(404,
                        $"ProtoCMS: no select field options handler found with id '{handlerId}'.");
                }
                if (req.OptionValues.Length == 0) {
                    throw new HttpException(400,
                        $"ProtoCMS: no select field options can be found without options values param.");
                }
                var result = new List<SelectFieldOption>();
                foreach (var optionValue in req.OptionValues) {
                    var optObj = handler.GetOptionObject(optionValue, req.HandlerParam);
                    if (optObj == null) return JsonProto<SelectFieldOption>(null);
                    var opd = handler.GetOptionDisplay(optObj, req.HandlerParam);
                    if (opd != null) result.Add(opd);
                }
                return JsonProto(result.ToArray());
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }
    }
}