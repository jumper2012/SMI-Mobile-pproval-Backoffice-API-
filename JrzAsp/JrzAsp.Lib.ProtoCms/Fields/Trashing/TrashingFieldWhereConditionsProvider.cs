using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingFieldWhereConditionsProvider : IContentWhereConditionsProvider {
        public const string IS_TRASHED_WHERE_CONDITION_NAME = "IsTrashed";
        public decimal Priority => 0;

        public IEnumerable<ContentWhereCondition> DefineWhereConditions() {
            yield return new ContentWhereCondition(
                IS_TRASHED_WHERE_CONDITION_NAME,
                $"Whether content is trashed or not. Accepts {typeof(TrashingFieldTableFilterForm)} as parameter.");
        }
    }
}