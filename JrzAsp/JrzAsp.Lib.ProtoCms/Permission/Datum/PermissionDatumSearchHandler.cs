using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Permission.Datum {
    public class PermissionDatumSearchHandler : BaseDatumSearchHandler<ProtoPermission> {
        public override decimal Priority => 0;

        public override Expression<Func<ProtoPermission, bool>> HandleSearch(string keywords, string[] splittedKeywords,
            Type datumType, out bool callNextHandler) {
            callNextHandler = true;
            return x => splittedKeywords.Any(k =>
                x.DisplayName.Contains(k) || x.Description.Contains(k) ||
                x.CategoryName.Contains(k) || x.SubCategoryName.Contains(k) ||
                x.Id == k
            );
        }
    }
}