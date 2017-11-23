using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumFinder : IAlwaysFreshDependency {
        Type DatumModelType { get; }
        DatumType DatumTypeBase { get; }
        IDatumGetterHandler[] GetterHandlersBase { get; }
        IDatumViewerHandler[] ViewerHandlersBase { get; }
        IDatumSearchHandler[] SearchHandlersBase { get; }
        IDatumWhereHandler[] WhereHandlersBase { get; }
        IDatumSortHandler[] SortHandlersBase { get; }
        IDatumTableFilterHandler[] TableFilterHandlers { get; }
        IDatumTableActionsHandler[] TableActionsHandlersBase { get; }
        IDatumPermissionsHandler[] PermissionsHandlersBase { get; }
        ViewDatumPermission PermissionToViewBase { get; }
        ListDatumPermission PermissionToListBase { get; }

        IDatumFinder ResetBase(IQueryable query = null);
        IDatumFinder SearchBase(string keywords);
        IDatumFinder WhereBase(string conditionName, object param);
        IDatumFinder WhereBase(Expression expression);
        IDatumFinder WhereOrBase(IEnumerable<ContentTableFilterOperation> filterOperations);
        IDatumFinder WhereOrBase(IEnumerable<Expression> expressions);
        IDatumFinder SortBase(string fieldName, bool isDescending = false);
        IDatumFinder TableFilterBase(IEnumerable<ContentTableFilterOperation> filterOperations);

        IPaginatedQueryable PaginatedBase(int currentPage, int limitPerPage);

        string GetDatumIdBase(object datum);
        object CreateInMemoryBase(string datumId);
        object FindByIdBase(string id);
        string[] DefinedFieldNames();
        IQueryable AsQueryableBase();
        string AsSummarizedValueBase(object datum);
        IDictionary<string, VueComponentDefinition[]> AsTableRowVueBase(object datum);
        ContentPreviewPart[] AsFullPreviewBase(object datum);
        VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext);
        VueActionTrigger[] TableActionsForSingleContent(string datumId, ProtoCmsRuntimeContext cmsContext);
        ContentTableHeader[] TableHeaders();

        DatumWhereCondition FindDatumWhereCondition(string conditionName);
    }

    public interface IDatumFinder<TDat> : IDatumFinder {
        DatumType<TDat> DatumType { get; }
        IDatumGetterHandler<TDat>[] GetterHandlers { get; }
        IDatumViewerHandler<TDat>[] ViewerHandlers { get; }
        IDatumSearchHandler<TDat>[] SearchHandlers { get; }
        IDatumWhereHandler<TDat>[] WhereHandlers { get; }
        IDatumSortHandler<TDat>[] SortHandlers { get; }
        IDatumTableActionsHandler<TDat>[] TableActionsHandlers { get; }
        IDatumPermissionsHandler<TDat>[] PermissionsHandlers { get; }
        ViewDatumPermission<TDat> PermissionToView { get; }
        ListDatumPermission<TDat> PermissionToList { get; }

        IDatumFinder<TDat> Reset(IQueryable<TDat> query = null);
        IDatumFinder<TDat> Search(string keywords);
        IDatumFinder<TDat> Where(string conditionName, object param);
        IDatumFinder<TDat> Where(Expression<Func<TDat, bool>> expression);
        IDatumFinder<TDat> WhereOr(IEnumerable<ContentTableFilterOperation> filterOperations);
        IDatumFinder<TDat> WhereOr(IEnumerable<Expression<Func<TDat, bool>>> expressions);
        IDatumFinder<TDat> Sort(string fieldName, bool isDescending = false);
        IDatumFinder<TDat> TableFilter(IEnumerable<ContentTableFilterOperation> filterOperations);

        IPaginatedQueryable<TDat> Paginated(int currentPage, int limitPerPage);

        string GetDatumId(TDat datum);
        TDat CreateInMemory(string datumId);
        TDat FindById(string id);
        IQueryable<TDat> AsQueryable();
        string AsSummarizedValue(TDat datum);
        IDictionary<string, VueComponentDefinition[]> AsTableRowVue(TDat datum);
        ContentPreviewPart[] AsFullPreview(TDat datum);
    }
}