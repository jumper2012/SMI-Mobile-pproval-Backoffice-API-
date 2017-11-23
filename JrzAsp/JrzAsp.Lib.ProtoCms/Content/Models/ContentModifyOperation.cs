using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Permissions;
using JrzAsp.Lib.ProtoCms.Fields.Common;
using Newtonsoft.Json;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentModifyOperation {
        public const string ANY_MODIFY_OPERATION_NAME = "*";

        public ContentModifyOperation(string name, string devDescription,
            Func<ContentType, string> generatePermissionDisplayName,
            Func<ContentType, string> generatePermissionDescription,
            bool isCreatingNewContent) {
            Name = name?.Trim();
            DevDescription = devDescription?.Trim();
            GeneratePermissionDisplayName = generatePermissionDisplayName;
            GeneratePermissionDescription = generatePermissionDescription;
            IsCreatingNewContent = isCreatingNewContent;
            ValidateProps();
        }

        public string Name { get; }
        public string DevDescription { get; }
        public bool IsCreatingNewContent { get; }

        [JsonIgnore]
        public Func<ContentType, string> GeneratePermissionDisplayName { get; }

        [JsonIgnore]
        public Func<ContentType, string> GeneratePermissionDescription { get; }

        public ModifyContentPermission BuildPermission(ContentType contentType) {
            return new ModifyContentPermission(Name, contentType, GeneratePermissionDisplayName,
                GeneratePermissionDescription);
        }

        public bool Is(string modifyOperationName) {
            return Name == modifyOperationName || modifyOperationName == ANY_MODIFY_OPERATION_NAME;
        }

        public bool IsAny(params string[] modifyOperationNames) {
            return modifyOperationNames.Any(x => x == Name || x == ANY_MODIFY_OPERATION_NAME);
        }

        public bool IsCreateOperation() {
            return Is(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME);
        }

        public bool IsUpdateOperation() {
            return Is(CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME);
        }

        public bool IsDeleteOperation() {
            return Is(CommonFieldModifyOperationsProvider.DELETE_OPERATION_NAME);
        }

        public bool IsCreateOrUpdateOperation() {
            return IsAny(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME,
                CommonFieldModifyOperationsProvider.UPDATE_OPERATION_NAME);
        }

        private void ValidateProps() {
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new InvalidOperationException($"ProtoCMS: content modify operation name is required.");
            }
            if (!ContentType.VALID_ID_REGEX.IsMatch(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content modify operation name must match regex '{ContentType.VALID_ID_PATTERN}'."
                );
            }
            if (string.IsNullOrWhiteSpace(DevDescription)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content modify operation dev description must be provided.");
            }
            if (GeneratePermissionDisplayName == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content modify operation's GeneratePermissionDisplayName function is required.");
            }
            if (GeneratePermissionDescription == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content modify operation's GeneratePermissionDescription function is required.");
            }
        }
    }
}