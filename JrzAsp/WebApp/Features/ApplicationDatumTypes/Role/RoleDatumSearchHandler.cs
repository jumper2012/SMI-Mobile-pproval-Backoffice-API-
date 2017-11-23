using System;
using System.Linq;
using System.Linq.Expressions;
using WebApp.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumSearchHandler : BaseDatumSearchHandler<ApplicationRole> {
        public override Expression<Func<ApplicationRole, bool>> HandleSearch(string keywords, string[] splittedKeywords,
            Type datumType, out bool callNextHandler) {
            callNextHandler = true;
            return x => splittedKeywords.Any(k => x.Name.Contains(k) || x.Description.Contains(k) || x.Id == k);
        }

        public override decimal Priority => 0;
    }
}