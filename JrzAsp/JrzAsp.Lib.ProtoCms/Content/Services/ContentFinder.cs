using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TextUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public class ContentFinder : IContentFinder {
        public const string SUMMARY_VALUE_SEPARATOR = "|";
        public static readonly Type[] SearchHandlerTypes;
        public static readonly Type[] WhereHandlerTypes;
        public static readonly Type[] SortHandlerTypes;
        public static readonly Type[] TableFilterHandlerTypes;
        public static readonly Type[] TableActionsHandlerTypes;
        private static readonly IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();

        private static IContentWhereConditionsProvider[] _whereConditionsProviders;
        private static ContentWhereCondition[] _definedWhereConditions;
        private readonly IProtoCmsDbContext _dbContext;
        private IQueryable<ProtoContent> _currentQuery;
        private IContentSearchHandler[] _searchHandlers;
        private IContentSortHandler[] _sortHandlers;
        private IContentTableActionsHandler[] _tableActionsHandlers;
        private IContentTableFilterHandler[] _tableFilterHandlers;
        private IContentWhereHandler[] _whereHandlers;

        static ContentFinder() {
            var shh = new List<Type>();
            var whh = new List<Type>();
            var soh = new List<Type>();
            var fhh = new List<Type>();
            var tap = new List<Type>();
            foreach (var t in TypesCache.AppDomainTypes.Where(
                x => x.IsNonDynamicallyGeneratedConcreteClass())) {
                if (typeof(IContentSearchHandler).IsAssignableFrom(t)) shh.Add(t);
                if (typeof(IContentWhereHandler).IsAssignableFrom(t)) whh.Add(t);
                if (typeof(IContentSortHandler).IsAssignableFrom(t)) soh.Add(t);
                if (typeof(IContentTableFilterHandler).IsAssignableFrom(t)) fhh.Add(t);
                if (typeof(IContentTableActionsHandler).IsAssignableFrom(t)) tap.Add(t);
            }
            SearchHandlerTypes = shh.ToArray();
            WhereHandlerTypes = whh.ToArray();
            SortHandlerTypes = soh.ToArray();
            TableActionsHandlerTypes = tap.ToArray();
            TableFilterHandlerTypes = fhh.ToArray();
        }

        public ContentFinder(IProtoCmsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public static IContentWhereConditionsProvider[] WhereConditionsProviders {
            get {
                if (_whereConditionsProviders != null) return _whereConditionsProviders;
                _whereConditionsProviders = _dp.GetServices(typeof(IContentWhereConditionsProvider))
                    .Cast<IContentWhereConditionsProvider>().OrderBy(x => x.Priority).ToArray();

                return _whereConditionsProviders;
            }
        }
        public static ContentWhereCondition[] DefinedWhereConditions {
            get {
                if (_definedWhereConditions != null) return _definedWhereConditions;
                var map = new Dictionary<string, List<Tuple<ContentWhereCondition, Type>>>();
                var mops = WhereConditionsProviders;
                foreach (var mop in mops) {
                    foreach (var mo in mop.DefineWhereConditions()) {
                        if (!map.TryGetValue(mo.Name, out var moMap)) {
                            moMap = new List<Tuple<ContentWhereCondition, Type>>();
                            map[mo.Name] = moMap;
                        }
                        moMap.Add(Tuple.Create(mo, mop.GetType()));
                    }
                }
                var dmos = new List<ContentWhereCondition>();
                foreach (var mapKv in map) {
                    var mos = mapKv.Value.Select(x => x.Item1);
                    var provs = mapKv.Value.Select(x => x.Item2);
                    var dmo = new CombinedContentWhereCondition(mapKv.Key, mos, provs);
                    dmos.Add(dmo);
                }
                _definedWhereConditions = dmos.ToArray();
                return _definedWhereConditions;
            }
        }

        public ContentType ContentType { get; private set; }
        public IContentSearchHandler[] SearchHandlers {
            get {
                if (_searchHandlers == null) InitSearchHandlers();
                return _searchHandlers;

            }
        }
        public IContentWhereHandler[] WhereHandlers {
            get {
                if (_whereHandlers == null) InitWhereHandlers();
                return _whereHandlers;
            }
        }
        public IContentSortHandler[] SortHandlers {
            get {
                if (_sortHandlers == null) InitSortHandlers();
                return _sortHandlers;
            }
        }
        public IContentTableFilterHandler[] TableFilterHandlers {
            get {
                if (_tableFilterHandlers == null) InitTableFilterHandlers();
                return _tableFilterHandlers;
            }
        }
        public IContentTableActionsHandler[] TableActionsHandlers {
            get {
                if (_tableActionsHandlers == null) InitTableActionsProviders();
                return _tableActionsHandlers;
            }
        }

        public void Initialize(ContentType contentType) {
            ContentType = contentType;
            Reset();
        }

        public IContentFinder Reset(IQueryable<ProtoContent> query = null) {
            var contentTypeId = ContentType.Id;
            if (query == null) {
                _currentQuery = from c in _dbContext.ProtoContents
                    where c.ContentTypeId == contentTypeId
                    select c;
            } else {
                _currentQuery = query;
            }
            return this;
        }

        public IContentFinder Search(string keywords) {
            var prevStack = new Stack<IContentSearchHandler>();
            var nextStack = new Stack<IContentSearchHandler>(SearchHandlers);
            var callNext = true;
            var pred = PredicateBuilder.False<ProtoContent>();
            var kws = keywords.SplitAsKeywords().ToArray();
            while (nextStack.Count > 0 && callNext) {
                var h = nextStack.Pop();
                prevStack.Push(h);
                var cond = h.HandleSearch(keywords, kws, ContentType, out callNext);
                if (cond != null) {
                    pred = pred.Or(cond);
                }
            }
            _currentQuery = _currentQuery.Where(pred);
            return this;
        }

        public IContentFinder Where(string conditionName, object param = null) {
            var prevStack = new Stack<IContentWhereHandler>();
            var nextStack = new Stack<IContentWhereHandler>(WhereHandlers);
            var callNext = true;
            var pred = PredicateBuilder.True<ProtoContent>();
            var condition = FindContentWhereCondition(conditionName);
            while (nextStack.Count > 0 && callNext) {
                var h = nextStack.Pop();
                prevStack.Push(h);
                var cond = h.HandleWhere(condition, param, ContentType, out callNext);
                if (cond != null) pred = pred.And(cond);
            }
            _currentQuery = _currentQuery.Where(pred);
            return this;
        }

        public IContentFinder Where(Expression<Func<ProtoContent, bool>> expression) {
            _currentQuery = _currentQuery.Where(expression);
            return this;
        }

        public IContentFinder WhereOr(IEnumerable<ContentTableFilterOperation> filterOperations) {
            var finalPred = PredicateBuilder.False<ProtoContent>();
            foreach (var fo in filterOperations) {
                var prevStack = new Stack<IContentWhereHandler>();
                var nextStack = new Stack<IContentWhereHandler>(WhereHandlers);
                var callNext = true;
                var pred = PredicateBuilder.True<ProtoContent>();
                var condition = FindContentWhereCondition(fo.WhereConditionName);
                while (nextStack.Count > 0 && callNext) {
                    var h = nextStack.Pop();
                    prevStack.Push(h);
                    var cond = h.HandleWhere(condition, fo.WhereMethodParam, ContentType, out callNext);
                    if (cond != null) pred = pred.And(cond);
                }
                finalPred = finalPred.Or(pred);
            }
            _currentQuery = _currentQuery.Where(finalPred);
            return this;
        }

        public IContentFinder WhereOr(IEnumerable<Expression<Func<ProtoContent, bool>>> expressions) {
            var pred = PredicateBuilder.False<ProtoContent>();
            foreach (var exp in expressions) {
                pred = pred.Or(exp);
            }
            _currentQuery = _currentQuery.Where(pred);
            return this;
        }

        public IContentFinder Sort(string fieldName, bool isDescending = false) {
            var q = _currentQuery;
            var prevStack = new Stack<IContentSortHandler>();
            var nextStack = new Stack<IContentSortHandler>(SortHandlers);
            var callNext = true;
            while (nextStack.Count > 0 && callNext) {
                var h = nextStack.Pop();
                prevStack.Push(h);
                var nextQ = h.HandleSort(q, fieldName, isDescending, ContentType, out callNext);
                if (nextQ != null) q = nextQ;
            }
            _currentQuery = q;
            return this;
        }

        public IContentFinder TableFilter(IEnumerable<ContentTableFilterOperation> filterOperations) {
            var finder = this;
            foreach (var fo in filterOperations) {
                if (string.IsNullOrWhiteSpace(fo?.WhereConditionName)) continue;
                finder.Where(fo.WhereConditionName, fo.WhereMethodParam);
            }
            return finder;
        }

        public ProtoContent FindById(string id) {
            return _currentQuery.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ProtoContent> AsQueryable() {
            return _currentQuery;
        }

        public string AsSummarizedValue(ProtoContent content, IEnumerable<string> summarySourceFieldNames = null) {
            var parts = new List<string>();
            summarySourceFieldNames = summarySourceFieldNames ?? ContentType.FieldNamesIncludedInSummary;
            foreach (var fn in summarySourceFieldNames) {
                var fd = ContentType.Field(fn);
                var ffr = fd.FieldFinder();
                var fm = ffr.GetModel(content, fd);
                if (fn == $"{fd.FieldName}") {
                    foreach (var fc in ffr.Columns) {
                        var fsumz = fc.SummarizedValue(fm, fd)?.Trim();
                        if (!string.IsNullOrWhiteSpace(fsumz)) parts.Add($"{fsumz}");
                    }
                } else {
                    var fc = ffr.Columns.FirstOrDefault(x => $"{fd.FieldName}.{x.PropName}" == fn);
                    if (fc == null) continue;
                    var fsumz = fc.SummarizedValue(fm, fd)?.Trim();
                    if (!string.IsNullOrWhiteSpace(fsumz)) parts.Add($"{fsumz}");
                }
            }
            var sv = string.Join($" {SUMMARY_VALUE_SEPARATOR} ", parts);
            return sv;
        }

        public ProtoContentDynamic AsDynamic(ProtoContent content) {
            return content != null ? new ProtoContentDynamic(content) : null;
        }

        public IDictionary<string, VueComponentDefinition[]> AsTableRowVue(ProtoContent content) {
            var result = new Dictionary<string, VueComponentDefinition[]>();
            foreach (var fd in ContentType.Fields) {
                var ff = fd.FieldFinder();
                var fmdl = ff.GetModel(content, fd);
                foreach (var col in ff.Columns) result[$"{fd.FieldName}.{col.PropName}"] = col.CellValue(fmdl, fd);
            }
            return result;
        }

        public ContentPreviewPart[] AsFullPreview(ProtoContent content) {
            var result = new List<ContentPreviewPart>();
            foreach (var fd in ContentType.Fields) {
                var ff = fd.FieldFinder();
                var fmdl = ff.GetModel(content, fd);
                foreach (var col in ff.Columns) {
                    result.Add(new ContentPreviewPart(col.ColumnHeader(fd), $"{fd.FieldName}.{col.PropName}",
                        col.FullPreviewValue(fmdl, fd)));
                }
            }
            return result.OrderBy(x => x.Label).ToArray();
        }

        public VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext) {
            var actions = new List<VueActionTrigger>();
            foreach (var prov in TableActionsHandlers) {
                var acts = prov.TableActionsForNoContent(cmsContext, ContentType);
                if (acts != null && acts.Length > 0) actions.AddRange(acts);
            }
            return actions.ToArray();
        }

        public VueActionTrigger[] TableActionsForSingleContent(string contentId, ProtoCmsRuntimeContext cmsContext) {
            var actions = new List<VueActionTrigger>();
            var content = _currentQuery.FirstOrDefault(x => x.Id == contentId);
            if (ContentType != null) {
                actions.Add(new VueButton {
                    Label = $"View Raw",
                    IconCssClass = "fa fa-eye",
                    OnClick =
                        $"protoCms.utils.popupEntityRawDataViewer('content', '{contentId}', '{ContentType.Id}', " +
                        $"'Raw Data View', '{ContentType.Name}')"
                });
            }
            foreach (var prov in TableActionsHandlers) {
                var acts = prov.TableActionsForSingleContent(content, cmsContext, ContentType);
                if (acts != null && acts.Length > 0) actions.AddRange(acts);
            }
            return actions.ToArray();
        }

        public ContentTableHeader[] TableHeaders() {
            var hs = new List<ContentTableHeader>();
            foreach (var shown in ContentType.ShownFieldNamesInTable) {
                var fieldNameParts = shown.Split('.');
                var fd = ContentType.Fields.First(x => x.FieldName == fieldNameParts[0]);
                var ff = fd.FieldFinder();
                foreach (var col in ff.Columns) {
                    if (fieldNameParts.Length > 1) {
                        var colNameParts = new string[fieldNameParts.Length - 1];
                        Array.Copy(fieldNameParts, 1, colNameParts, 0, colNameParts.Length);
                        var colPropName = string.Join(".", colNameParts);
                        if (col.PropName == colPropName) {
                            hs.Add(
                                new ContentTableHeader(
                                    $"{fd.FieldName}.{col.PropName}",
                                    col.ColumnHeader(fd),
                                    col.Sortable(fd)
                                )
                            );
                        }
                    } else {
                        hs.Add(
                            new ContentTableHeader(
                                $"{fd.FieldName}.{col.PropName}",
                                col.ColumnHeader(fd),
                                col.Sortable(fd)
                            )
                        );
                    }
                }
            }
            return hs.ToArray();
        }

        public ContentWhereCondition FindContentWhereCondition(string conditionName) {
            var modOp = DefinedWhereConditions.FirstOrDefault(x => x.Name == conditionName);
            if (modOp == null) {
                throw new HttpException(400, $"ProtoCMS: content where condition '{conditionName}' " +
                                             $"doesn't exist.");
            }
            return modOp;
        }

        private void InitSearchHandlers() {
            var handlerTypes = SearchHandlerTypes;
            var handlers = new List<IContentSearchHandler>();
            foreach (var ht in handlerTypes) {
                var h = _dp.GetService(ht).DirectCastTo<IContentSearchHandler>();
                if (h.HandledContentTypeIds != null &&
                    (h.HandledContentTypeIds.Contains(ContentType.ANY_CONTENT_TYPE_ID) ||
                     h.HandledContentTypeIds.Contains(ContentType.Id))) {
                    handlers.Add(h);
                }
            }
            _searchHandlers = handlers.OrderBy(x => x.Priority).ToArray();
        }

        private void InitWhereHandlers() {
            var handlerTypes = WhereHandlerTypes;
            var handlers = new List<IContentWhereHandler>();
            foreach (var ht in handlerTypes) {
                var h = _dp.GetService(ht).DirectCastTo<IContentWhereHandler>();
                if (h.HandledContentTypeIds != null &&
                    (h.HandledContentTypeIds.Contains(ContentType.ANY_CONTENT_TYPE_ID) ||
                     h.HandledContentTypeIds.Contains(ContentType.Id))) {
                    handlers.Add(h);
                }
            }
            _whereHandlers = handlers.OrderBy(x => x.Priority).ToArray();
        }

        private void InitSortHandlers() {
            var handlerTypes = SortHandlerTypes;
            var handlers = new List<IContentSortHandler>();
            foreach (var ht in handlerTypes) {
                var h = _dp.GetService(ht).DirectCastTo<IContentSortHandler>();
                if (h.HandledContentTypeIds != null &&
                    (h.HandledContentTypeIds.Contains(ContentType.ANY_CONTENT_TYPE_ID) ||
                     h.HandledContentTypeIds.Contains(ContentType.Id))) {
                    handlers.Add(h);
                }
            }
            _sortHandlers = handlers.OrderBy(x => x.Priority).ToArray();
        }

        private void InitTableFilterHandlers() {
            var provTypes = TableFilterHandlerTypes;
            var provs = new List<IContentTableFilterHandler>();
            var dups = new Dictionary<string, int>();
            foreach (var ht in provTypes) {
                var h = _dp.GetService(ht).DirectCastTo<IContentTableFilterHandler>();
                if (h.HandledContentTypeIds != null &&
                    (h.HandledContentTypeIds.Contains(ContentType.ANY_CONTENT_TYPE_ID) ||
                     h.HandledContentTypeIds.Contains(ContentType.Id))) {

                    if (!dups.TryGetValue(h.Id, out var count)) {
                        count = 0;
                    }
                    count++;
                    if (count > 1) {
                        throw new InvalidOperationException(
                            $"ProtoCMS: content table filter handler with id '{h.Id}' is defined more than once."
                        );
                    }
                    if (!h.CanFilter(ContentType)) continue;
                    provs.Add(h);
                }
            }
            _tableFilterHandlers = provs.OrderBy(x => x.Priority).ToArray();
        }

        private void InitTableActionsProviders() {
            var provTypes = TableActionsHandlerTypes;
            var provs = new List<IContentTableActionsHandler>();
            foreach (var ht in provTypes) {
                var h = _dp.GetService(ht).DirectCastTo<IContentTableActionsHandler>();
                if (h.HandledContentTypeIds != null &&
                    (h.HandledContentTypeIds.Contains(ContentType.ANY_CONTENT_TYPE_ID) ||
                     h.HandledContentTypeIds.Contains(ContentType.Id))) {
                    provs.Add(h);
                }
            }
            _tableActionsHandlers = provs.OrderBy(x => x.Priority).ToArray();
        }
    }
}