using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.QueryableUtilities;

namespace JrzAsp.Lib.ProtoCms.Permission.Datum {
    public class PermissionDatumSortHandler : BaseDatumSortHandler<ProtoPermission> {

        public override decimal Priority => 0;

        public override IQueryable<ProtoPermission> HandleSort(IQueryable<ProtoPermission> currentQuery,
            string fieldName,
            bool descending, Type datumType,
            out bool callNextHandler) {
            callNextHandler = true;
            if (fieldName == nameof(ProtoPermission.SubCategoryName)) {
                return currentQuery.AddOrderBy(x => x.SubCategoryName, descending);
            }
            if (fieldName == nameof(ProtoPermission.DisplayName)) {
                return currentQuery.AddOrderBy(x => x.DisplayName, descending);
            }
            if (fieldName == nameof(ProtoPermission.Id)) {
                return currentQuery.AddOrderBy(x => x.Id, descending);
            }
            return currentQuery.AddOrderBy(x => x.CategoryName, descending).ThenBy(x => x.SubCategoryName);
        }
    }
}