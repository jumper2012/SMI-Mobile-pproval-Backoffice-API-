using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using Newtonsoft.Json;

namespace JrzAsp.Lib.ProtoCms.WebApi.API {
    [EnableCors("*", "*", "*")]
    public abstract class BaseProtoApiController : ApiController {

        protected BaseProtoApiController() {
            var hc = ProtoEngine.GetHttpConfiguration();
            var jsonSerStg = hc.Formatters.JsonFormatter.SerializerSettings;
            jsonSerStg.TypeNameHandling = TypeNameHandling.Auto;
            jsonSerStg.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            jsonSerStg.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        public JsonResult<T> JsonProto<T>(T data) {
            return Json(data, GetJsonSerializerSettingForAbstractTypeHandling());
        }

        public virtual HttpResponseException RestfulApiError(HttpException ex) {
            if (!Enum.TryParse(ex.GetHttpCode().ToString(), out HttpStatusCode statusCode)) {
                statusCode = HttpStatusCode.BadRequest;
            }
            var payload = Request.CreateErrorResponse(statusCode, ex.Message);
            return new HttpResponseException(payload);
        }

        protected JsonSerializerSettings GetJsonSerializerSettingForAbstractTypeHandling() {
            return Jsonizer.SerializerSettings;
        }
    }
}