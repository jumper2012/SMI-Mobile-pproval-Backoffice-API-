using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Fields.Select;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public class DatumSelectFieldOptionsHandler : ISelectFieldOptionsHandler {
        public const string HANDLER_ID = "datum-select-field-options-handler";

        public string Id => HANDLER_ID;

        public string DevDescription => $"Get datum as select field options. Accepts a JSON string " +
                                        $"that should be serializable to an instance of " +
                                        $"{typeof(DatumSelectFieldOptionsHandlerParam).FullName}.";

        public decimal Priority => 0;

        public PaginatedQueryable<object> GetOptions(string keywords, int page, int limitPerPage, string handlerParam) {
            var param = ParseParam(handlerParam);
            var ct = FindDatumType(param.DatumTypeId);
            var finder = string.IsNullOrWhiteSpace(keywords) ? ct.FinderBase() : ct.FinderBase().SearchBase(keywords);
            foreach (var wc in param.WhereConditions) {
                finder = finder.WhereBase(wc.Item1, wc.Item2);
            }
            var hasSort = false;
            foreach (var si in param.SortInfos) {
                finder = finder.SortBase(si.Item1, si.Item2);
                hasSort = true;
            }
            if (!hasSort) {
                finder = finder.SortBase(ct.DefaultSortFieldName, ct.DefaultSortDescending);
            }
            var q = finder.AsQueryableBase().DirectCastTo<IQueryable<object>>();
            var pag = new PaginatedQueryable<object>(q, page, limitPerPage);
            return pag;
        }

        public object GetOptionObject(string optionValue, string handlerParam) {
            var param = ParseParam(handlerParam);
            var c = FindDatumType(param.DatumTypeId).FinderBase().FindByIdBase(optionValue);
            return c;
        }

        public SelectFieldOption GetOptionDisplay(object option, string handlerParam) {
            if (option == null) return null;
            var ct = FindDatumTypeFromObject(option);
            var finder = ct.FinderBase();
            var cid = finder.GetDatumIdBase(option);
            var sfo = new SelectFieldOption(cid, finder.AsSummarizedValueBase(option), null);
            return sfo;
        }

        private DatumSelectFieldOptionsHandlerParam ParseParam(string handlerParam) {
            return Jsonizer.Parse<DatumSelectFieldOptionsHandlerParam>(handlerParam);
        }

        private DatumType FindDatumType(string datumTypeId) {
            return DatumType.DefinedTypesMap[datumTypeId];
        }

        private DatumType FindDatumTypeFromObject(object obj) {
            DatumType dt = null;
            var objType = obj.GetType();
            while (dt == null && objType != null) {
                dt = DatumType.DefinedTypes.FirstOrDefault(x => x.ModelType == objType);
                objType = objType.BaseType;
            }
            if (dt == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: Can't find matching datum type for model type '{obj.GetType()}'.");
            }
            return dt;
        }
    }
}