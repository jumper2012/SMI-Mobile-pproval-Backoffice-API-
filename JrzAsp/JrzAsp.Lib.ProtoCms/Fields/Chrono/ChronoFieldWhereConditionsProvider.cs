using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoFieldWhereConditionsProvider : IContentWhereConditionsProvider {
        public const string CONDITION_NAME = "ChronoFieldRange";
        public decimal Priority => 0;

        public IEnumerable<ContentWhereCondition> DefineWhereConditions() {
            yield return new ContentWhereCondition(CONDITION_NAME,
                $"Whether a datetime field is within range or not. Accepts {typeof(ChronoFieldTableFilterFormItem)} as parameter.");
        }
    }
}