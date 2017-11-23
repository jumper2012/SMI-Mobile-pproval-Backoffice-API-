using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.QueryableUtilities;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumSortHandler : BaseDatumSortHandler<ApplicationRole> {

        public override decimal Priority => 0;

        public override IQueryable<ApplicationRole> HandleSort(IQueryable<ApplicationRole> currentQuery,
            string fieldName,
            bool descending, Type datumType,
            out bool callNextHandler) {
            callNextHandler = true;
            if (fieldName == nameof(ApplicationRole.Id)) {
                return currentQuery.AddOrderBy(x => x.Id, descending);
            }
            if (fieldName == nameof(ApplicationRole.CreatedUtc)) {
                return currentQuery.AddOrderBy(x => x.CreatedUtc, descending);
            }
            if (fieldName == nameof(ApplicationRole.UpdatedUtc)) {
                return currentQuery.AddOrderBy(x => x.UpdatedUtc, descending);
            }
            return currentQuery.AddOrderBy(x => x.Name, descending);
        }
    }
}