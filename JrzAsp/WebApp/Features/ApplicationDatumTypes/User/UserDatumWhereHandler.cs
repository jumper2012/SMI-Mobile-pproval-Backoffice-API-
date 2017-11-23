using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.TypeUtilities;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumWhereHandler : BaseDatumWhereHandler<ApplicationUser> {
        private readonly ApplicationDbContext _dbContext;

        public UserDatumWhereHandler(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        public override decimal Priority => 0;

        public override Expression<Func<ApplicationUser, bool>> HandleWhere(DatumWhereCondition condition, object param,
            Type datumType, out bool callNextHandler) {
            callNextHandler = true;
            if (!condition.Is(UserDatumWhereConditionsProvider.IS_IN_ROLE_WHERE_CONDITION_NAME)) return null;
            if (!(param is string[])) return null;
            var par = param.DirectCastTo<string[]>();
            var roleIds = _dbContext.Roles.Where(x => par.Contains(x.Name)).Select(x => x.Id).ToArray();
            return x => x.Roles.Any(r => roleIds.Contains(r.RoleId));

        }
    }
}