using System;
using System.Linq;
using System.Linq.Expressions;
using WebApp.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumSearchHandler : BaseDatumSearchHandler<ApplicationUser> {
        public override Expression<Func<ApplicationUser, bool>> HandleSearch(string keywords, string[] splittedKeywords,
            Type datumType, out bool callNextHandler) {
            callNextHandler = true;
            return x => splittedKeywords.Any(k => 
                x.DisplayName.Contains(k)
                || x.UserName.Contains(k)
                || x.Email.Contains(k)
                || x.PhoneNumber.Contains(k)
                || x.Id == k);
        }

        public override decimal Priority => 0;
    }
}