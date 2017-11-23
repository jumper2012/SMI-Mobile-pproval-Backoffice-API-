using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.Fields.Chrono;
using JrzAsp.Lib.ProtoCms.Fields.Select;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TextUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public class ContentSelectFieldOptionsHandler : ISelectFieldOptionsHandler {
        public const string HANDLER_ID = "content-select-field-options-handler";

        private readonly IProtoCmsDbContext _dbContext;

        public ContentSelectFieldOptionsHandler(IProtoCmsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Id => HANDLER_ID;

        public string DevDescription => $"Get content as select field options. Accepts param string that is JSON " +
                                        $"deserializable to an instance of " +
                                        $"{typeof(ContentSelectFieldOptionsHandlerParam).FullName}.";

        public decimal Priority => 0;

        public PaginatedQueryable<object> GetOptions(string keywords, int page, int limitPerPage, string handlerParam) {
            var param = ParseParam(handlerParam);
            var ctIds = param.ContentTypeIds;
            if (ctIds.Length == 1) {
                var ct = FindContentType(ctIds[0]);
                var finder = string.IsNullOrWhiteSpace(keywords) ? ct.Finder() : ct.Finder().Search(keywords);
                foreach (var wc in param.WhereConditions) {
                    finder = finder.Where(wc.Item1, wc.Item2);
                }
                var hasSort = false;
                foreach (var si in param.SortInfos) {
                    finder = finder.Sort(si.Item1, si.Item2);
                    hasSort = true;
                }
                if (!hasSort) {
                    finder = finder.Sort(ct.DefaultSortFieldName, ct.DefaultSortDescending);
                }
                var q = finder.AsQueryable().DirectCastTo<IQueryable<object>>();
                var pag = new PaginatedQueryable<object>(q, page, limitPerPage);
                return pag;
            }
            if (!string.IsNullOrWhiteSpace(keywords)) {
                var kws = keywords.SplitAsKeywords();
                var preds = PredicateBuilder.False<ProtoContent>();
                preds = preds.And(x => x.ContentFields.Any(cf => kws.Any(k => cf.StringValue.Contains(k))));
                var hasDtExpr = ChronoUtils.Self.TryBuildSearchConditionExpression<ProtoField>(out var searchDtCond,
                    keywords,
                    cf => cf.DateTimeValue, true);
                if (hasDtExpr) {
                    preds = preds.And(x => x.ContentFields.AsQueryable().Any(searchDtCond));
                }
                var q = _dbContext.ProtoContents.AsQueryable().Where(preds)
                    .Where(x => ctIds.Any(ctId => x.ContentTypeId == ctId))
                    .OrderBy(x => x.ContentFields.FirstOrDefault(cf => cf.StringValue != null).StringValue)
                    .ThenBy(x => x.ContentTypeId).DirectCastTo<IQueryable<object>>();

                var pag = new PaginatedQueryable<object>(q, page, limitPerPage);
                return pag;
            } else {
                var q = _dbContext.ProtoContents.Where(x => ctIds.Any(ctId => x.ContentTypeId == ctId))
                    .OrderBy(x => x.ContentFields.FirstOrDefault(cf => cf.StringValue != null).StringValue)
                    .ThenBy(x => x.ContentTypeId).DirectCastTo<IQueryable<object>>();

                var pag = new PaginatedQueryable<object>(q, page, limitPerPage);
                return pag;
            }
        }

        public object GetOptionObject(string optionValue, string handlerParam) {
            var param = ParseParam(handlerParam);
            foreach (var ctId in param.ContentTypeIds) {
                var ct = FindContentType(ctId);
                var c = ct.Finder().FindById(optionValue);
                if (c != null) return c;
            }
            return null;
        }

        public SelectFieldOption GetOptionDisplay(object option, string handlerParam) {
            if (option == null) return null;
            var c = option.DirectCastTo<ProtoContent>();
            var ctId = c.ContentTypeId;
            var ct = FindContentType(ctId);
            var finder = ct.Finder();
            var sfo = new SelectFieldOption(c.Id, finder.AsSummarizedValue(c), null);
            return sfo;
        }

        private ContentType FindContentType(string contentTypeId) {
            return ContentType.DefinedTypesMap[contentTypeId];
        }

        private ContentSelectFieldOptionsHandlerParam ParseParam(string handlerParam) {
            return Jsonizer.Parse<ContentSelectFieldOptionsHandlerParam>(handlerParam);
        }
    }
}