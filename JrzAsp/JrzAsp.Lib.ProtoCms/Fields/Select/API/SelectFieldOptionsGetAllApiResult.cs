using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ProtoCms.Fields.Select.API {
    public class SelectFieldOptionsGetAllApiResult {

        public SelectFieldOptionsGetAllApiResult(ISelectFieldOptionsHandler handler, string search, int page,
            int limitPerPage,
            string handlerParam) {

            var pag = handler.GetOptions(search, page, limitPerPage, handlerParam);

            CurrentPage = pag.CurrentPage;
            TotalPage = pag.TotalPage;
            Offset = pag.Offset;
            Limit = pag.Limit;
            GlobalCount = pag.Count;
            StartNumbering = pag.StartNumbering;

            var data = pag.CurrentPageQueryable.ToArray();

            CurrentCount = data.Length;

            var sfoData = new List<SelectFieldOption>();
            foreach (var dt in data) {
                var sfo = handler.GetOptionDisplay(dt, handlerParam);
                sfoData.Add(sfo);
            }
            Data = sfoData.ToArray();

        }

        public int CurrentPage { get; }
        public int TotalPage { get; }
        public int Offset { get; }
        public int Limit { get; }
        public int CurrentCount { get; }
        public int GlobalCount { get; }
        public int StartNumbering { get; }
        public SelectFieldOption[] Data { get; }
    }
}