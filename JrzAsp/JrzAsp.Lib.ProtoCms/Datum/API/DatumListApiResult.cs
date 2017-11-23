using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.API;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.API {
    public class DatumListApiResult {
        public const string DATUM_TABLE_ACTION_KEY = "__DatumTableActions";

        public DatumListApiResult(ProtoCmsRuntimeContext cmsContext, DatumType contentType, string search,
            string sortColumn, bool? isDescending, int offset, int limit, ContentListShape? shape,
            IDatumFinder finder = null) {

            finder = finder ?? contentType.FinderBase();
            var contentShape = shape ?? ContentListShape.Normal;

            if (!string.IsNullOrWhiteSpace(search)) {
                finder = finder.SearchBase(search.Trim());
            }
            if (!string.IsNullOrWhiteSpace(sortColumn)) {
                SortColumn = sortColumn;
                if (isDescending.HasValue) {
                    SortIsDescending = isDescending.Value;
                    finder = finder.SortBase(sortColumn, isDescending.Value);
                } else {
                    SortIsDescending = contentType.DefaultSortDescending;
                    finder = finder.SortBase(sortColumn, contentType.DefaultSortDescending);
                }
            } else {
                SortColumn = contentType.DefaultSortFieldName;
                if (isDescending.HasValue) {
                    SortIsDescending = isDescending.Value;
                    finder = finder.SortBase(contentType.DefaultSortFieldName, isDescending.Value);
                } else {
                    SortIsDescending = contentType.DefaultSortDescending;
                    finder = finder.SortBase(contentType.DefaultSortFieldName, contentType.DefaultSortDescending);
                }
            }

            if (offset < 0) offset = 0;
            if (limit < 1) limit = 100;
            var currentPage = offset / limit + 1;

            var paginated = finder.PaginatedBase(currentPage, limit);
            var rawData = paginated.CurrentPageQueryableBase.DirectCastTo<IQueryable<object>>().ToArray();
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
            var getter = new DatumShapedApiModelBuilder();
            foreach (var rd in rawData) {
                data.Add(getter.GetDatumShaped(rd, contentType, cmsContext, contentShape));
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
}