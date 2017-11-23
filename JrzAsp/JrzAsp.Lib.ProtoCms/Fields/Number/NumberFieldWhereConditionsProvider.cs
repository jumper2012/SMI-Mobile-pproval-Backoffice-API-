using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;

namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberFieldWhereConditionsProvider : IContentWhereConditionsProvider {
        public const string CONDITION_NAME = "NumberFieldRange";
        public decimal Priority => 0;

        public IEnumerable<ContentWhereCondition> DefineWhereConditions() {
            yield return new ContentWhereCondition(CONDITION_NAME,
                $"Whether a number field is within range or not. Accepts {typeof(NumberFieldTableFilterFormItem)} as parameter.");
        }
    }
}