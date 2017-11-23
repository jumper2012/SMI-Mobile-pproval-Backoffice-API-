using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Permissions {
    public class ListContentPermission : ProtoPermission {
        public ListContentPermission(ContentType contentType) {
            ContentType = contentType;
        }

        public ContentType ContentType { get; }

        public override string Id => GetIdFor(ContentType.Id);
        public override string DisplayName => $"List {ContentType.Name} Content in CMS";
        public override string Description => $"Enables to view a list of {ContentType.Name} contents in CMS.";
        public override string CategoryName => $"Content Management";
        public override string SubCategoryName => $"{ContentType.Name}";
        public override bool AuthenticatedUserHasThisByDefault => false;
        public override bool GuestUserHasThisByDefault => false;

        public static string GetIdFor(string contentTypeId) {
            return $"Content({contentTypeId}):List";
        }
    }
}