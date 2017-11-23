using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;

namespace JrzAsp.Lib.ProtoCms.Fields.Boolean {
    public class BooleanFieldWhereConditionsProvider : IContentWhereConditionsProvider {
        public const string CONDITION_NAME = "IsBooleanFieldChecked";
        public decimal Priority => 0;

        public IEnumerable<ContentWhereCondition> DefineWhereConditions() {
            yield return new ContentWhereCondition(CONDITION_NAME,
                $"Whether a boolean field is checked or not. Accepts {typeof(BooleanFieldTableFilterFormItem)} as parameter.");
        }
    }
}