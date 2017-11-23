using System;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Permissions {
    public class ModifyContentPermission : ProtoPermission {
        public ModifyContentPermission(string modifyOperationName, ContentType contentType,
            Func<ContentType, string> generateDisplayName, Func<ContentType, string> generateDescription) {
            ModifyOperationName = modifyOperationName;
            ContentType = contentType;
            GenerateDisplayName = generateDisplayName;
            GenerateDescription = generateDescription;
        }

        public string ModifyOperationName { get; }
        public ContentType ContentType { get; }
        public Func<ContentType, string> GenerateDisplayName { get; }
        public Func<ContentType, string> GenerateDescription { get; }

        public override string Id => GetIdFor(ContentType.Id, ModifyOperationName);
        public override string DisplayName => GenerateDisplayName(ContentType);
        public override string Description => GenerateDescription(ContentType);
        public override string CategoryName => $"Content Management";
        public override string SubCategoryName => $"{ContentType.Name}";
        public override bool AuthenticatedUserHasThisByDefault => false;
        public override bool GuestUserHasThisByDefault => false;

        public static string GetIdFor(string contentTypeId, string modifyOperationName) {
            return $"Content({contentTypeId}):Modify({modifyOperationName})";
        }
    }
}