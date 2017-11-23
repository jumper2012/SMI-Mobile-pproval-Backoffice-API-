using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public class StandardContentWhereConditionsProvider : IContentWhereConditionsProvider {
        public const string IS_PUBLISHED_AND_NOT_TRASHED = "IsPublishedAndNotTrashed";
        public const string IS_PUBLISHED_AND_TRASHED = "IsPublishedAndTrashed";
        public const string IS_NOT_PUBLISHED_AND_NOT_TRASHED = "IsNotPublishedAndNotTrashed";
        public const string IS_NOT_PUBLISHED_AND_TRASHED = "IsNotPublishedAndTrashed";

        public decimal Priority => 0;

        public IEnumerable<ContentWhereCondition> DefineWhereConditions() {
            yield return new ContentWhereCondition(IS_PUBLISHED_AND_NOT_TRASHED,
                "Where content is published and not trashed.");
            yield return new ContentWhereCondition(IS_PUBLISHED_AND_TRASHED,
                "Where content is published and trashed.");
            yield return new ContentWhereCondition(IS_NOT_PUBLISHED_AND_NOT_TRASHED,
                "Where content is not published and not trashed.");
            yield return new ContentWhereCondition(IS_NOT_PUBLISHED_AND_TRASHED,
                "Where content is not published and trashed.");
        }
    }
}