using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentFinder : IAlwaysFreshDependency {
        ContentType ContentType { get; }
        IContentSearchHandler[] SearchHandlers { get; }
        IContentWhereHandler[] WhereHandlers { get; }
        IContentSortHandler[] SortHandlers { get; }
        IContentTableFilterHandler[] TableFilterHandlers { get; }
        IContentTableActionsHandler[] TableActionsHandlers { get; }

        void Initialize(ContentType contentType);

        ContentWhereCondition FindContentWhereCondition(string conditionName);

        IContentFinder Reset(IQueryable<ProtoContent> query = null);
        IContentFinder Search(string keywords);
        IContentFinder Where(string conditionName, object param = null);
        IContentFinder Where(Expression<Func<ProtoContent, bool>> expression);
        IContentFinder WhereOr(IEnumerable<ContentTableFilterOperation> filterOperations);
        IContentFinder WhereOr(IEnumerable<Expression<Func<ProtoContent, bool>>> expressions);
        IContentFinder Sort(string fieldName, bool isDescending = false);
        IContentFinder TableFilter(IEnumerable<ContentTableFilterOperation> filterOperations);

        ProtoContent FindById(string id);
        IQueryable<ProtoContent> AsQueryable();
        string AsSummarizedValue(ProtoContent content, IEnumerable<string> summarySourceFieldNames = null);
        ProtoContentDynamic AsDynamic(ProtoContent content);
        IDictionary<string, VueComponentDefinition[]> AsTableRowVue(ProtoContent content);
        ContentPreviewPart[] AsFullPreview(ProtoContent content);
        VueActionTrigger[] TableActionsForNoContent(ProtoCmsRuntimeContext cmsContext);
        VueActionTrigger[] TableActionsForSingleContent(string contentId, ProtoCmsRuntimeContext cmsContext);
        ContentTableHeader[] TableHeaders();
    }
}