using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Role.Models;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumWhereConditionsProvider : IDatumWhereConditionsProvider {
        public const string IS_SYSTEM_RESERVED_WHERE_CONDITION_NAME = "IsSystemReserved";
        public decimal Priority => 0;

        public IEnumerable<DatumWhereCondition> DefineWhereConditions() {
            yield return new DatumWhereCondition(IS_SYSTEM_RESERVED_WHERE_CONDITION_NAME,
                $"Whether the role is system reserved or not. {RequiredRoleInfo.Authenticated.Name}, " +
                $"{RequiredRoleInfo.Guest.Name}, and {RequiredRoleInfo.SuperAdmin.Name} are system reserved. " +
                $"Accepts {typeof(RoleDatumIsReservedTableFilterForm)} as parameter.");
        }
    }
}