using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;

namespace JrzAsp.Lib.ProtoCms.Datum.Models {
    public class DatumModifyOperation {
        public const string ANY_MODIFY_OPERATION_NAME = "*";

        public DatumModifyOperation(string name, string devDescription, bool isCreatingNewDatum) {
            IsCreatingNewDatum = isCreatingNewDatum;
            Name = name?.Trim();
            DevDescription = devDescription?.Trim();
            ValidateProps();
        }

        public string Name { get; }
        public string DevDescription { get; }
        public bool IsCreatingNewDatum { get; }

        public bool Is(string modifyOperationName) {
            return Name == modifyOperationName || modifyOperationName == ANY_MODIFY_OPERATION_NAME;
        }

        public bool IsAny(params string[] modifyOperationNames) {
            return modifyOperationNames.Any(x => x == Name || x == ANY_MODIFY_OPERATION_NAME);
        }

        public bool IsCreateOperation() {
            return Is(StandardModifyOperationsProvider.CREATE_OPERATION_NAME);
        }

        public bool IsUpdateOperation() {
            return Is(StandardModifyOperationsProvider.UPDATE_OPERATION_NAME);
        }

        public bool IsDeleteOperation() {
            return Is(StandardModifyOperationsProvider.DELETE_OPERATION_NAME);
        }

        public bool IsCreateOrUpdateOperation() {
            return IsAny(StandardModifyOperationsProvider.CREATE_OPERATION_NAME,
                StandardModifyOperationsProvider.UPDATE_OPERATION_NAME);
        }

        private void ValidateProps() {
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new InvalidOperationException($"ProtoCMS: datum modify operation name is required.");
            }
            if (!ContentType.VALID_ID_REGEX.IsMatch(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum modify operation name must match regex '{ContentType.VALID_ID_PATTERN}'."
                );
            }
            if (string.IsNullOrWhiteSpace(DevDescription)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum modify operation dev description must be provided.");
            }
        }
    }
}