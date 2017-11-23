using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;

namespace JrzAsp.Lib.ProtoCms.Fields.Publishing {
    public class PublishingFieldWhereConditionsProvider : IContentWhereConditionsProvider {
        public const string IS_PUBLISHED_WHERE_CONDITION_NAME = "IsPublished";
        public decimal Priority => 0;

        public IEnumerable<ContentWhereCondition> DefineWhereConditions() {
            yield return new ContentWhereCondition(
                IS_PUBLISHED_WHERE_CONDITION_NAME,
                $"Whether content is published or not. Accepts {typeof(PublishingFieldTableFilterForm)} as parameter.");
        }
    }
}