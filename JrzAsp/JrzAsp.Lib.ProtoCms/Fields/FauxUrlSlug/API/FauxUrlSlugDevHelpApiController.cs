using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.TypeUtilities;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;

namespace JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug.API {
    [RoutePrefixProtoCms("dev-help")]
    public class FauxUrlSlugDevHelpApiController : BaseProtoApiController {
        private readonly IProtoCmsDbContext _dbContext;
        private readonly IFauxUrlSlugService _fauxUrlSlugSvc;

        public FauxUrlSlugDevHelpApiController(IProtoCmsDbContext dbContext, IFauxUrlSlugService fauxUrlSlugSvc) {
            _dbContext = dbContext;
            _fauxUrlSlugSvc = fauxUrlSlugSvc;
        }

        [Route("faux-url-slug/get-slug-for/{contentId}")]
        [HttpGet]
        [ResponseType(typeof(FauxUrlSlugApiResult))]
        public IHttpActionResult GetFauxUrlSlugForContent(string contentId) {
            try {
                var pc = _dbContext.ProtoContents.Find(contentId);
                if (pc == null) {
                    throw new HttpException(404, $"No content found with id '{contentId}'");
                }
                var ctId = pc.ContentTypeId;
                var ct = ContentType.DefinedTypes.FirstOrDefault(x => x.Id == ctId);
                if (ct == null) {
                    throw new HttpException(404, $"No content type found with id '{ctId}'");
                }
                var fauxDef = ct.Fields.FirstOrDefault(x => x.ModelType == typeof(FauxUrlSlugField));
                if (fauxDef == null) {
                    throw new HttpException(400, $"Content type '{ctId}' doesn't use faux url slug field.");
                }
                var ff = fauxDef.FieldFinder().DirectCastTo<FauxUrlSlugFieldFinder>();
                var fauxField = ff.GetModel(pc, fauxDef).DirectCastTo<FauxUrlSlugField>();
                var result = new FauxUrlSlugApiResult {
                    Result = fauxField.Slug
                };
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("faux-url-slug/get-content-by/{*fauxUrlSlug}")]
        [HttpGet]
        [ResponseType(typeof(FauxUrlSlugApiMultiResult))]
        public IHttpActionResult GetContentByFauxUrlSlug(string fauxUrlSlug) {
            try {
                var pcs = _fauxUrlSlugSvc.FindContentByFauxUrlSlug(fauxUrlSlug);
                var ids = pcs.Select(x => x.Id);
                var result = new FauxUrlSlugApiMultiResult {
                    Result = ids.ToArray()
                };
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }
    }
}