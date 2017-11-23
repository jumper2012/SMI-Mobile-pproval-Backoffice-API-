using System.Linq;
using System.Web.Mvc;
using JrzAsp.Lib.Logging;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Permissions;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug;
using WebApp.Features.FauxUrlSlugMvc;

namespace WebApp.Controllers {
    public class FauxUrlSlugController : Controller {
        private readonly IFauxUrlSlugService _fauxUrlSvc;
        private static readonly ILog _logger = LogService.GetLogger(typeof(FauxUrlSlugController));

        public FauxUrlSlugController(IFauxUrlSlugService fauxUrlSvc) {
            _fauxUrlSvc = fauxUrlSvc;
        }

        public ActionResult DisplayContent(string fauxUrlSlug) {
            var matchedContents = _fauxUrlSvc.FindContentByFauxUrlSlug(fauxUrlSlug);
            if (matchedContents.Length == 0) {
                return HttpNotFound();
            }
            if (matchedContents.Length > 1) {
                var warnMultiMatch = string.Join(", ",
                    matchedContents.Select(x => $"{x.Id} | {x.ContentTypeId}").ToArray());
                _logger.Warn($"Found multiple proto content that match url slug '{fauxUrlSlug}' ({warnMultiMatch}).");
            }
            var firstProtoContent = matchedContents[0];
            var contentType = ContentType.DefinedTypes.FirstOrDefault(x => x.Id == firstProtoContent.ContentTypeId);
            if (contentType == null) {
                _logger.Error($"Found proto content that match url slug '{fauxUrlSlug}' with id " +
                              $"'{firstProtoContent.Id}' but its content type '{firstProtoContent.ContentTypeId}' " +
                              $"has not been defined in the system.");
                return HttpNotFound();
            }
            var rctx = ProtoCmsRuntimeContext.Current;
            if (!rctx.UserHasPermission(ViewContentPermission.GetIdFor(contentType.Id))) {
                return HttpNotFound();
            }
            dynamic contentAsDynamic = contentType.Finder().AsDynamic(firstProtoContent);
            var isPublished = (bool) contentAsDynamic.PublishStatus.IsPublished;
            var isTrashed = (bool) contentAsDynamic.TrashStatus.IsTrashed;
            if (!(isPublished && !isTrashed)) return HttpNotFound();
            var mdl = new FauxUrlSlugContentModel(contentType, firstProtoContent, contentAsDynamic);
            var viewName = $"_ContentTypeViews/{contentType.Id}/_View";
            return View(viewName, mdl);
        }
    }
}