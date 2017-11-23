using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TextUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public abstract class DatumFinder : IDatumFinder {

        protected static readonly IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();

        private static DatumWhereCondition[] _definedWhereConditions;

        private static IDatumWhereConditionsProvider[] _whereConditionsProviders;

        private static Type[] _searchHandlerTypes;

        private static Type[] _whereHandlerTypes;
        private static Type[] _sortHandlerTypes;
        private static Type[] _tableActionsHandlerTypes;
        private static Type[] _getterHandlerTypes;
        private static Type[] _tableFilterHandlerTypes;
        private static Type[] _permissionsHandlerTypes;
        private static Type[] _viewerHandlerTypes;
        public static DatumWhereCondition[] DefinedWhereConditions {
            get {
                if (_definedWhereConditions != null) return _definedWhereConditions;
                var map = new Dictionary<string, List<Tuple<DatumWhereCondition, Type>>>();
                var mops = WhereConditionsProviders;
                foreach (var mop in mops) {
                    foreach (var mo in mop.DefineWhereConditions()) {
                        if (!map.TryGetValue(mo.Name, out var moMap)) {
                            moMap = new List<Tuple<DatumWhereCondition, Type>>();
                            map[mo.Name] = moMap;
                        }
                        moMap.Add(Tuple.Create(mo, mop.GetType()));
                    }
                }
                var dmos = new List<DatumWhereCondition>();
                foreach (var mapKv in map) {
                    var mos = mapKv.Value.Select(x => x.Item1);
                    var provs = mapKv.Value.Select(x => x.Item2);
                    var dmo = new CombinedDatumWhereCondition(mapKv.Key, mos, provs);
                    dmos.Add(dmo);
                }
                _definedWhereConditions = dmos.ToArray();
                return _definedWhereConditions;
            }
        }
        public static IDatumWhereConditionsProvider[] WhereConditionsProviders {
            get {
                if (_whereConditionsProviders != null) return _whereConditionsProviders;
                _whereConditionsProviders = _dp.GetServices(typeof(IDatumWhereConditionsProvider))
                    .Cast<IDatumWhereConditionsProvider>().OrderBy(x => x.Priority).ToArray();

                return _whereConditionsProviders;
            }
        }
        public static Type[] PermissionsHandlerTypes {
            get {
                if (_permissionsHandlerTypes != null) return _permissionsHandlerTypes;
                _permissionsHandlerTypes = TypesCache.AppDomainTypes.Where(x =>
                        typeof(IDatumPermissionsHandler).IsAssignableFrom(x) &&
                        x.IsNonDynamicallyGeneratedConcreteClass())
                    .ToArray();
                return _permissionsHandlerTypes;
            }
        }
        public static Type[] GetterHandlerTypes {
            get {
                if (_getterHandlerTypes != null) return _getterHandlerTypes;
                _getterHandlerTypes = TypesCache.AppDomainTypes.Where(x =>
                        typeof(IDatumGetterHandler).IsAssignableFrom(x) &&
                        x.IsNonDynamicallyGeneratedConcreteClass())
                    .ToArray();
                return _getterHandlerTypes;
            }
        }
        public static Type[] ViewerHandlerTypes {
            get {
                if (_viewerHandlerTypes != null) return _viewerHandlerTypes;
                _viewerHandlerTypes = TypesCache.AppDomainTypes.Where(x =>
                        typeof(IDatumViewerHandler).IsAssignableFrom(x) &&
                        x.IsNonDynamicallyGeneratedConcreteClass())
                    .ToArray();
                return _viewerHandlerTypes;
            }
        }
        public static Type[] SearchHandlerTypes {
            get {
                if (_searchHandlerTypes != null) return _searchHandlerTypes;
                _searchHandlerTypes = TypesCache.AppDomainTypes.Where(x =>
                        typeof(IDatumSearchHandler).IsAssignableFrom(x) &&
                        x.IsNonDynamicallyGeneratedConcreteClass())
                    .ToArray();
                return _searchHandlerTypes;
            }
        }
        public static Type[] WhereHandlerTypes {
            get {
                if (_whereHandlerTypes != null) return _whereHandlerTypes;
                _whereHandlerTypes = TypesCache.AppDomainTypes.Where(x =>
                        typeof(IDatumWhereHandler).IsAssignableFrom(x) &&
                        x.IsNonDynamicallyGeneratedConcreteClass())
                    .ToArray();
                return _whereHandlerTypes;
            }
        }
        public static Type[] SortHandlerTypes {
            get {
                if (_sortHandlerTypes != null) return _sortHandlerTypes;
                _sortHandlerTypes = TypesCache.AppDomainTypes.Where(x =>
                        typeof(IDatumSortHandler).IsAssignableFrom(x) &&
                        x.IsNonDynamicallyGeneratedConcreteClass())
                    .ToArray();
                return _sortHandlerTypes;
            }
        }
        public static Type[] TableActionsHandlerTypes {
            get {
                if (_tableActionsHandlerTypes != null) return _tableActionsHandlerTypes;
                _tableActionsHandlerTypes = TypesCache.AppDomainTypes.Where(x =>
                        typeof(IDatumTableActionsHandler).IsAssignableFrom(x) &&
                        x.IsNonDynamicallyGeneratedConcreteClass())
                    .ToArray();
                return _tableActionsHandlerTypes;
            }
        }
        public static Type[] TableFilterHandlerTypes {
            get {
                if (_tableFilterHandlerTypes != null) return _tableFilterHandlerTypes;
                _tableFilterHandlerTypes = TypesCache.AppDomainTypes.Where(x =>
                        typeof(IDatumTableFilterHandler).IsAssignableFrom(x) &&
                        x.IsNonDynamicallyGeneratedConcreteClass())
                    .ToArray();
                return _tableActionsHandlerTypes;
            }
        }

        public abstract Type DatumModelType { get; }
        public abstract DatumType DatumTypeBase { get; }
        public abstract IDatumGetterHandler[] GetterHandlersBase { get; }
        public abstract IDatumViewerHandler[] ViewerHandlersBase { get; }
        public abstract IDatumSearchHandler[] SearchHandlersBase { get; }
        public abstract IDatumWhereHandler[] WhereHandlersBase { get; }
        public abstract IDatumSortHandler[] SortHandlersBase { get; }
        public abstract IDatumTableFilterHandler[] TableFilterHandlers { get; }
        public abstract IDatumTableActionsHandler[] TableActionsHandlersBase { get; }
        public abstract IDatumPermissionsHandler[] PermissionsHandlersBase { get; }
        public abstract ViewDatumPermission PermissionToViewBase { get; }
        public abstract ListDatumPermission PermissionToListBase { get; }
        public abstract IDatumFinder ResetBase(IQueryable query = null);
        public abstract IDatumFinder SearchBase(string keywords);
        public abstract IDatumFinder WhereBase(string conditionName, object param);
        public abstract IDatumFinder WhereBase(Expression expression);
        public abstract IDatumFinder WhereOrBase(IEnumerable<ContentTableFilterOperation> filterOperations);
        public abstract IDatumFinder WhereOrBase(IEnumerable<Expression> expressions);
        public abstract IDatumFinder SortBase(string fieldName, bool isDescending = false);
        public abstract IDatumFinder TableFilterBase(IEnumerable<ContentTableFilterOperation> filterOperations);
        public abstract IPaginatedQueryable PaginatedBase(int currentPage, int limitPerPage);
        public abstract string GetDatumIdBase(object datum);
        public abstract object CreateInMemoryBase(string datumId);
        public abstract object FindByIdBase(string id);

        public string[] DefinedFieldNames() {
            var fns = new List<string>();
            foreach (var vh in ViewerHandlersBase) {
                fns.AddRange(vh.GetValidFieldNames());
            }
            return fns.ToArray();
        }

        public abstract IQueryable AsQueryableBase();
        public abstract string AsSummarizedValueBase(object datum);
        public abstract IDictionary<string, VueComponentDefinition[]> AsTableRowVueBase(object datum);
        public abstract ContentPreviewPart[] AsFullPreviewBase(object datum);
        public abstract VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext);

        public abstract VueActionTrigger[] TableActionsForSingleContent(string datumId,
            ProtoCmsRuntimeContext cmsContext);

        public abstract ContentTableHeader[] TableHeaders();
        public abstract DatumWhereCondition FindDatumWhereCondition(string conditionName);
    }

    public class DatumFinder<TDat> : DatumFinder, IDatumFinder<TDat> {

        private IQueryable<TDat> _currentQuery;

        private IDatumGetterHandler<TDat>[] _getterHandlers;
        private IDatumPermissionsHandler<TDat>[] _permissionsHandlers;
        private ListDatumPermission<TDat> _permissionToList;
        private ViewDatumPermission<TDat> _permissionToView;
        private IDatumSearchHandler<TDat>[] _searchHandlers;
        private IDatumSortHandler<TDat>[] _sortHandlers;
        private IDatumTableActionsHandler<TDat>[] _tableActionsHandlers;
        private IDatumTableFilterHandler[] _tableFilterHandlers;
        private IDatumViewerHandler<TDat>[] _viewerHandlers;
        private IDatumWhereHandler<TDat>[] _whereHandlers;
        private IQueryable<TDat> CurrentQuery {
            get {
                if (_currentQuery == null) Reset();
                return _currentQuery;
            }
            set => _currentQuery = value;
        }

        public override Type DatumModelType => typeof(TDat);
        public override DatumType DatumTypeBase => DatumType;
        public override IDatumGetterHandler[] GetterHandlersBase => GetterHandlers;
        public override IDatumViewerHandler[] ViewerHandlersBase => ViewerHandlers;
        public override IDatumSearchHandler[] SearchHandlersBase => SearchHandlers;
        public override IDatumWhereHandler[] WhereHandlersBase => WhereHandlers;
        public override IDatumSortHandler[] SortHandlersBase => SortHandlers;
        public override IDatumTableActionsHandler[] TableActionsHandlersBase => TableActionsHandlers;
        public override IDatumPermissionsHandler[] PermissionsHandlersBase => PermissionsHandlers;
        public override ViewDatumPermission PermissionToViewBase => PermissionToView;
        public override ListDatumPermission PermissionToListBase => PermissionToList;

        public override IDatumFinder ResetBase(IQueryable query = null) {
            return Reset(query.DirectCastTo<IQueryable<TDat>>());
        }

        public override IDatumFinder SearchBase(string keywords) {
            return Search(keywords);
        }

        public override IDatumFinder WhereBase(string conditionName, object param) {
            return Where(conditionName, param);
        }

        public override IDatumFinder WhereBase(Expression expression) {
            return Where(expression.DirectCastTo<Expression<Func<TDat, bool>>>());
        }

        public override IDatumFinder WhereOrBase(IEnumerable<ContentTableFilterOperation> filterOperations) {
            return WhereOr(filterOperations);
        }

        public override IDatumFinder WhereOrBase(IEnumerable<Expression> expressions) {
            return WhereOr(expressions.DirectCastTo<IEnumerable<Expression<Func<TDat, bool>>>>());
        }

        public override IDatumFinder SortBase(string fieldName, bool isDescending = false) {
            return Sort(fieldName, isDescending);
        }

        public override IDatumFinder TableFilterBase(IEnumerable<ContentTableFilterOperation> filterOperations) {
            return TableFilter(filterOperations);
        }

        public override IPaginatedQueryable PaginatedBase(int currentPage, int limitPerPage) {
            return Paginated(currentPage, limitPerPage);
        }

        public override string GetDatumIdBase(object datum) {
            return GetDatumId(datum.DirectCastTo<TDat>());
        }

        public override object CreateInMemoryBase(string datumId) {
            return CreateInMemory(datumId);
        }

        public override object FindByIdBase(string id) {
            return FindById(id);
        }

        public override IQueryable AsQueryableBase() {
            return AsQueryable();
        }

        public override string AsSummarizedValueBase(object datum) {
            return AsSummarizedValue(datum.DirectCastTo<TDat>());
        }

        public override IDictionary<string, VueComponentDefinition[]> AsTableRowVueBase(object datum) {
            return AsTableRowVue(datum.DirectCastTo<TDat>());
        }

        public override ContentPreviewPart[] AsFullPreviewBase(object datum) {
            return AsFullPreview(datum.DirectCastTo<TDat>());
        }

        public DatumType<TDat> DatumType => DatumModelType.GetDatumTypeFromType<TDat>();

        public IDatumGetterHandler<TDat>[] GetterHandlers {
            get {
                if (_getterHandlers != null) return _getterHandlers;
                _getterHandlers = _dp.GetServices(typeof(IDatumGetterHandler<TDat>))
                    .Cast<IDatumGetterHandler<TDat>>()
                    .OrderBy(x => x.Priority).ToArray();
                return _getterHandlers;
            }
        }
        public IDatumViewerHandler<TDat>[] ViewerHandlers {
            get {
                if (_viewerHandlers != null) return _viewerHandlers;
                _viewerHandlers = _dp.GetServices(typeof(IDatumViewerHandler<TDat>))
                    .Cast<IDatumViewerHandler<TDat>>()
                    .OrderBy(x => x.Priority).ToArray();
                return _viewerHandlers;
            }
        }
        public IDatumSearchHandler<TDat>[] SearchHandlers {
            get {
                if (_searchHandlers != null) return _searchHandlers;
                _searchHandlers = _dp.GetServices(typeof(IDatumSearchHandler<TDat>))
                    .Cast<IDatumSearchHandler<TDat>>()
                    .OrderBy(x => x.Priority).ToArray();
                return _searchHandlers;
            }
        }
        public IDatumWhereHandler<TDat>[] WhereHandlers {
            get {
                if (_whereHandlers != null) return _whereHandlers;
                _whereHandlers = _dp.GetServices(typeof(IDatumWhereHandler<TDat>))
                    .Cast<IDatumWhereHandler<TDat>>()
                    .OrderBy(x => x.Priority).ToArray();
                return _whereHandlers;
            }
        }
        public IDatumSortHandler<TDat>[] SortHandlers {
            get {
                if (_sortHandlers != null) return _sortHandlers;
                _sortHandlers = _dp.GetServices(typeof(IDatumSortHandler<TDat>))
                    .Cast<IDatumSortHandler<TDat>>()
                    .OrderBy(x => x.Priority).ToArray();
                return _sortHandlers;
            }
        }
        public override IDatumTableFilterHandler[] TableFilterHandlers {
            get {
                if (_tableFilterHandlers != null) return _tableFilterHandlers;
                _tableFilterHandlers = _dp.GetServices(typeof(IDatumTableFilterHandler))
                    .Cast<IDatumTableFilterHandler>()
                    .Where(x => x.DatumModelType == DatumModelType)
                    .OrderBy(x => x.Priority).ToArray();
                return _tableFilterHandlers;
            }
        }
        public IDatumTableActionsHandler<TDat>[] TableActionsHandlers {
            get {
                if (_tableActionsHandlers != null) return _tableActionsHandlers;
                _tableActionsHandlers = _dp.GetServices(typeof(IDatumTableActionsHandler<TDat>))
                    .Cast<IDatumTableActionsHandler<TDat>>()
                    .OrderBy(x => x.Priority).ToArray();
                return _tableActionsHandlers;
            }
        }
        public IDatumPermissionsHandler<TDat>[] PermissionsHandlers {
            get {
                if (_permissionsHandlers != null) return _permissionsHandlers;
                _permissionsHandlers = _dp.GetServices(typeof(IDatumPermissionsHandler<TDat>))
                    .Cast<IDatumPermissionsHandler<TDat>>()
                    .OrderBy(x => x.Priority).ToArray();
                return _permissionsHandlers;
            }
        }
        public ViewDatumPermission<TDat> PermissionToView {
            get {
                if (_permissionToView != null) return _permissionToView;
                _permissionToView = new ViewDatumPermission<TDat>(GetFirstPermissionsHandler());
                return _permissionToView;
            }
        }
        public ListDatumPermission<TDat> PermissionToList {
            get {
                if (_permissionToList != null) return _permissionToList;
                _permissionToList = new ListDatumPermission<TDat>(GetFirstPermissionsHandler());
                return _permissionToList;
            }
        }

        public IDatumFinder<TDat> Reset(IQueryable<TDat> query = null) {
            CurrentQuery = query ?? GetFirstGetterHandler().BuildBaseQuery();
            return this;
        }

        public IDatumFinder<TDat> Search(string keywords) {
            var pred = PredicateBuilder.False<TDat>();
            var splitted = keywords.SplitAsKeywords().ToArray();
            foreach (var sh in SearchHandlers) {
                var cond = sh.HandleSearch(keywords, splitted, DatumModelType, out var callNext);
                if (cond != null) {
                    pred = pred.Or(cond);
                }
                if (!callNext) break;
            }
            CurrentQuery = CurrentQuery.Where(pred);
            return this;
        }

        public IDatumFinder<TDat> Where(string conditionName, object param) {
            var condi = FindDatumWhereCondition(conditionName);
            var pred = PredicateBuilder.True<TDat>();
            foreach (var wh in WhereHandlers) {
                var cond = wh.HandleWhere(condi, param, DatumModelType, out var callNext);
                if (cond != null) {
                    pred = pred.And(cond);
                }
                if (!callNext) break;
            }
            CurrentQuery = CurrentQuery.Where(pred);
            return this;
        }

        public IDatumFinder<TDat> Where(Expression<Func<TDat, bool>> expression) {
            CurrentQuery = CurrentQuery.Where(expression);
            return this;
        }

        public IDatumFinder<TDat> WhereOr(IEnumerable<ContentTableFilterOperation> filterOperations) {
            var finalPred = PredicateBuilder.False<TDat>();
            foreach (var fo in filterOperations) {
                var condi = FindDatumWhereCondition(fo.WhereConditionName);
                var pred = PredicateBuilder.True<TDat>();
                foreach (var wh in WhereHandlers) {
                    var cond = wh.HandleWhere(condi, fo.WhereMethodParam, DatumModelType, out var callNext);
                    if (cond != null) {
                        pred = pred.And(cond);
                    }
                    if (!callNext) break;
                }
                finalPred = finalPred.Or(pred);
            }
            CurrentQuery = CurrentQuery.Where(finalPred);
            return this;
        }

        public IDatumFinder<TDat> WhereOr(IEnumerable<Expression<Func<TDat, bool>>> expressions) {
            var pred = PredicateBuilder.False<TDat>();
            foreach (var exp in expressions) {
                pred = pred.Or(exp);
            }
            CurrentQuery = CurrentQuery.Where(pred);
            return this;
        }

        public IDatumFinder<TDat> Sort(string fieldName, bool isDescending = false) {
            var q = CurrentQuery;
            foreach (var sh in SortHandlers) {
                var sortQ = sh.HandleSort(q, fieldName, isDescending, DatumModelType, out var callNext);
                if (sortQ != null) {
                    q = sortQ;
                }
                if (!callNext) break;
            }
            CurrentQuery = q;
            return this;
        }

        public IDatumFinder<TDat> TableFilter(IEnumerable<ContentTableFilterOperation> filterOperations) {
            var finder = this;
            foreach (var fo in filterOperations) {
                if (string.IsNullOrWhiteSpace(fo?.WhereConditionName)) continue;
                finder.Where(fo.WhereConditionName, fo.WhereMethodParam);
            }
            return finder;
        }

        public IPaginatedQueryable<TDat> Paginated(int currentPage, int limitPerPage) {
            var pag = new PaginatedQueryable<TDat>(CurrentQuery, currentPage, limitPerPage);
            return pag;
        }

        public string GetDatumId(TDat datum) {
            return GetFirstGetterHandler().GetDatumId(datum);
        }

        public TDat CreateInMemory(string datumId) {
            return GetFirstGetterHandler().CreateInMemory(datumId);
        }

        public TDat FindById(string id) {
            return GetFirstGetterHandler().FindById(id);
        }

        public IQueryable<TDat> AsQueryable() {
            return CurrentQuery;
        }

        public string AsSummarizedValue(TDat datum) {
            var sums = new List<string>();
            foreach (var fdn in DatumType.FieldNamesIncludedInSummary) {
                foreach (var vh in ViewerHandlers) {
                    var sum = vh.GetSummarizedValue(datum, fdn);
                    if (!string.IsNullOrWhiteSpace(sum)) {
                        sums.Add(sum);
                        break;
                    }
                }
            }
            return string.Join(" | ", sums);
        }

        public IDictionary<string, VueComponentDefinition[]> AsTableRowVue(TDat datum) {
            var trv = new Dictionary<string, VueComponentDefinition[]>();
            foreach (var fdn in DatumType.ShownFieldNamesInTable) {
                foreach (var vh in ViewerHandlers) {
                    var vds = vh.GetTableRowVue(datum, fdn);
                    if (vds != null && vds.Length > 0) {
                        trv[fdn] = vds;
                        break;
                    }
                }
            }
            return trv;
        }

        public ContentPreviewPart[] AsFullPreview(TDat datum) {
            var parts = new List<ContentPreviewPart>();
            foreach (var vh in ViewerHandlers) {
                foreach (var fdn in vh.GetValidFieldNames()) {
                    var part = vh.GetFullPreview(datum, fdn);
                    if (part != null) {
                        parts.Add(part);
                    }
                }
            }
            return parts.OrderBy(x => x.Label).ToArray();
        }

        public override VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext) {
            var actions = new List<VueActionTrigger>();
            foreach (var prov in TableActionsHandlers) {
                var acts = prov.TableActionsForNoContent(cmsContext, DatumModelType);
                if (acts != null && acts.Length > 0) actions.AddRange(acts);
            }
            return actions.ToArray();
        }

        public override VueActionTrigger[] TableActionsForSingleContent(string datumId,
            ProtoCmsRuntimeContext cmsContext) {
            var datum = FindById(datumId);
            var actions = new List<VueActionTrigger>();
            if (DatumType != null) {
                actions.Add(new VueButton {
                    Label = $"View Raw",
                    IconCssClass = "fa fa-eye",
                    OnClick = $"protoCms.utils.popupEntityRawDataViewer('datum', '{datumId}', '{DatumType.Id}', " +
                              $"'Raw Data View', '{DatumType.Name}')"
                });
            }
            foreach (var prov in TableActionsHandlers) {
                var acts = prov.TableActionsForSingleContent(datum, cmsContext, DatumModelType);
                if (acts != null && acts.Length > 0) actions.AddRange(acts);
            }
            return actions.ToArray();
        }

        public override ContentTableHeader[] TableHeaders() {
            var cths = new List<ContentTableHeader>();
            foreach (var fdn in DatumType.ShownFieldNamesInTable) {
                foreach (var vh in ViewerHandlers) {
                    var cth = vh.GetTableHeader(fdn);
                    if (cth != null) {
                        cths.Add(cth);
                        break;
                    }
                }
            }
            return cths.ToArray();
        }

        public override DatumWhereCondition FindDatumWhereCondition(string conditionName) {
            var modOp = DefinedWhereConditions.FirstOrDefault(x => x.Name == conditionName);
            if (modOp == null) {
                throw new HttpException(400, $"ProtoCMS: datum where condition '{conditionName}' " +
                                             $"doesn't exist.");
            }
            return modOp;
        }

        private IDatumGetterHandler<TDat> GetFirstGetterHandler() {
            if (GetterHandlers.Length > 0) {
                return GetterHandlers[0];
            }
            throw new InvalidOperationException(
                $"ProtoCMS: no getter handler defined for datum type '{DatumModelType}'.");
        }

        private IDatumPermissionsHandler<TDat> GetFirstPermissionsHandler() {
            if (PermissionsHandlers.Length > 0) {
                return PermissionsHandlers[0];
            }
            throw new InvalidOperationException(
                $"ProtoCMS: no permissions handler defined for datum type '{DatumModelType}'.");
        }
    }
}