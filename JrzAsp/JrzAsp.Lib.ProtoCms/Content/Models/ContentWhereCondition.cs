using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Fields.Publishing;
using JrzAsp.Lib.ProtoCms.Fields.Trashing;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentWhereCondition {
        public ContentWhereCondition(string name, string devDescription) {
            Name = name?.Trim();
            DevDescription = devDescription?.Trim();
            ValidateProps();
        }

        public string Name { get; }
        public string DevDescription { get; }

        public bool Is(string conditionName) {
            return Name == conditionName;
        }

        public bool IsAny(params string[] conditionNames) {
            return conditionNames.Any(x => x == Name);
        }

        public bool IsTrashedCondition() {
            return Name == TrashingFieldWhereConditionsProvider.IS_TRASHED_WHERE_CONDITION_NAME;
        }

        public bool IsPublishedCondition() {
            return Name == PublishingFieldWhereConditionsProvider.IS_PUBLISHED_WHERE_CONDITION_NAME;
        }

        private void ValidateProps() {
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content where condition name is required.");
            }
            if (!ContentType.VALID_FIELD_NAME_REGEX.IsMatch(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content where condition name must match regex " +
                    $"'{ContentType.VALID_FIELD_NAME_PATTERN}'.");
            }
            if (string.IsNullOrWhiteSpace(DevDescription)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content where condition dev description must be provided.");
            }
        }
    }
}