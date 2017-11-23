using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug {
    public class FauxUrlSlugService : IFauxUrlSlugService {
        private readonly IProtoCmsDbContext _dbContext;

        public FauxUrlSlugService(IProtoCmsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public ProtoContent[] FindContentByFauxUrlSlug(string fauxUrlSlug) {
            var slugParts = fauxUrlSlug.Trim('/').Split('/');
            var pcs = new List<ProtoContent>();
            foreach (var fsf in ContentType.DefinedTypes.Where(x =>
                x.Fields.Any(f => f.ModelType == typeof(FauxUrlSlugField))).Select(x =>
                x.Fields.First(f => f.ModelType == typeof(FauxUrlSlugField)))) {

                var fcfg = fsf.Config.DirectCastTo<FauxUrlSlugFieldConfiguration>();
                var cfgSlugPatternParts = fcfg.SlugPattern.Trim('/').Split('/');

                if (cfgSlugPatternParts.Length != slugParts.Length) continue;
                for (var i = 0; i < cfgSlugPatternParts.Length; i++) {
                    var cspp = cfgSlugPatternParts[i];
                    if (cspp != FauxUrlSlugFieldFinder.ID_PATTERN_PART) continue;
                    var sp = slugParts[i];
                    var pcMany = _dbContext.ProtoContents.Where(x => x.Id.Replace("-", "").StartsWith(sp));
                    pcs.AddRange(pcMany);
                }
            }
            return pcs.OrderByDescending(x => x.CreatedUtc).ToArray();
        }

        public bool IsContentMatchFauxUrlSlug(ProtoContent content, string fauxUrlSlug) {
            var slugParts = fauxUrlSlug.Trim('/').Split('/');
            foreach (var fsf in ContentType.DefinedTypes.Where(x =>
                x.Fields.Any(f => f.ModelType == typeof(FauxUrlSlugField))).Select(x =>
                x.Fields.First(f => f.ModelType == typeof(FauxUrlSlugField)))) {

                var fcfg = fsf.Config.DirectCastTo<FauxUrlSlugFieldConfiguration>();
                var idLen = fcfg.ContentIdSlugPartLength;
                var cfgSlugPatternParts = fcfg.SlugPattern.Trim('/').Split('/');

                if (cfgSlugPatternParts.Length != slugParts.Length) continue;
                for (var i = 0; i < cfgSlugPatternParts.Length; i++) {
                    var cspp = cfgSlugPatternParts[i];
                    if (cspp != FauxUrlSlugFieldFinder.ID_PATTERN_PART) continue;
                    var sp = slugParts[i];
                    if (sp.Length != idLen) continue;
                    var match = content.Id.Replace("-", "").StartsWith(sp);
                    if (match) return true;
                }
            }
            return false;
        }
    }
}