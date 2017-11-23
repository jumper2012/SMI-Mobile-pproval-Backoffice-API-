using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.QueryableUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.API {
    public class ContentListApiResult {
        public const string CONTENT_TABLE_ACTION_KEY = "__ContentTableActions";

        public ContentListApiResult(ProtoCmsRuntimeContext cmsContext, ContentType contentType, string search,
            string sortColumn, bool? isDescending, int offset, int limit, ContentListShape? shape,
            IContentFinder finder = null) {

            finder = finder ?? contentType.Finder();
            var contentShape = shape ?? ContentListShape.Normal;

            if (!string.IsNullOrWhiteSpace(search)) {
                finder = finder.Search(search.Trim());
            }
            if (!string.IsNullOrWhiteSpace(sortColumn)) {
                SortColumn = sortColumn;
                if (isDescending.HasValue) {
                    SortIsDescending = isDescending.Value;
                    finder = finder.Sort(sortColumn, isDescending.Value);
                } else {
                    SortIsDescending = contentType.DefaultSortDescending;
                    finder = finder.Sort(sortColumn, contentType.DefaultSortDescending);
                }
            } else {
                SortColumn = contentType.DefaultSortFieldName;
                if (isDescending.HasValue) {
                    SortIsDescending = isDescending.Value;
                    finder = finder.Sort(contentType.DefaultSortFieldName, isDescending.Value);
                } else {
                    SortIsDescending = contentType.DefaultSortDescending;
                    finder = finder.Sort(contentType.DefaultSortFieldName, contentType.DefaultSortDescending);
                }
            }

            if (offset < 0) offset = 0;
            if (limit < 1) limit = 100;
            var currentPage = offset / limit + 1;

            var paginated = new PaginatedQueryable<ProtoContent>(finder.AsQueryable(), currentPage, limit);
            var rawData = paginated.CurrentPageQueryable.ToArray();
            CurrentPage = paginated.CurrentPage;
            TotalPage = paginated.TotalPage;
            StartNumbering = paginated.StartNumbering;
            GlobalCount = paginated.Count;
            CurrentCount = rawData.Length;
            Offset = paginated.Offset;
            Limit = paginated.Limit;
            SearchKeywords = search;
            ResultShape = $"{contentShape.ToString()} ({(int) contentShape})";
            var data = new List<dynamic>();
            var getter = new ContentShapedApiModelBuilder();
            foreach (var rd in rawData) {
                data.Add(getter.GetContentShaped(rd, contentType, cmsContext, contentShape));
            }
            Data = data.ToArray();
        }

        public int CurrentPage { get; }
        public int TotalPage { get; }
        public int StartNumbering { get; }
        public int GlobalCount { get; }
        public int CurrentCount { get; }
        public int Offset { get; }
        public int Limit { get; }
        public string SearchKeywords { get; }
        public string SortColumn { get; }
        public bool SortIsDescending { get; }
        public string ResultShape { get; }
        public dynamic[] Data { get; }
    }

    public enum ContentListShape {
        Normal,
        TableRowVue,
        Summary,
        FullPreview
    }
}