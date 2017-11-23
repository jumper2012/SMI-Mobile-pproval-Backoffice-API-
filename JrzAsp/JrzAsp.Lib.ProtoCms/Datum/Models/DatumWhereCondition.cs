using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.Models {
    public class DatumWhereCondition {
        public DatumWhereCondition(string name, string devDescription) {
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

        private void ValidateProps() {
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum where condition name is required.");
            }
            if (!ContentType.VALID_FIELD_NAME_REGEX.IsMatch(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum where condition name must match regex " +
                    $"'{ContentType.VALID_FIELD_NAME_PATTERN}'.");
            }
            if (string.IsNullOrWhiteSpace(DevDescription)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum where condition dev description must be provided.");
            }
        }
    }
}