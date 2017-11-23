using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;

namespace JrzAsp.Lib.ProtoCms.FileSys.API {
    [RoutePrefixProtoCms("dev-help")]
    public class FileExplorerDevHelpApiController : BaseProtoApiController {
        private readonly IFileExplorerManager _fileMgr;

        public FileExplorerDevHelpApiController(IFileExplorerManager fileMgr) {
            _fileMgr = fileMgr;
        }

        [Route("file-explorer/handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<IFileExplorerHandler>))]
        public IHttpActionResult GetHandlers() {
            return JsonProto(_fileMgr.Handlers);
        }
    }
}