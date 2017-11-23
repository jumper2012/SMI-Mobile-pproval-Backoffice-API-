using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumWhereConditionsProvider : IDatumWhereConditionsProvider {
        public const string IS_ACTIVATED_WHERE_CONDITION_NAME = "IsActivated";
        public const string IS_IN_ROLE_WHERE_CONDITION_NAME = "IsInRole";
        public decimal Priority => 0;

        public IEnumerable<DatumWhereCondition> DefineWhereConditions() {
            yield return new DatumWhereCondition(IS_ACTIVATED_WHERE_CONDITION_NAME,
                $"Whether the user is activated or deactivated. " +
                $"Accepts {typeof(UserDatumIsActivatedTableFilterForm)} as parameter.");

            yield return new DatumWhereCondition(IS_IN_ROLE_WHERE_CONDITION_NAME,
                $"Whether the user is in role(s) or not. " +
                $"Accepts string[] as parameter that should contain role names.");
        }
    }
}