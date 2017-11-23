using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Permissions {
    public class ViewContentPermission : ProtoPermission {
        public ViewContentPermission(ContentType contentType) {
            ContentType = contentType;
        }

        public ContentType ContentType { get; }

        public override string Id => GetIdFor(ContentType.Id);
        public override string DisplayName => $"View {ContentType.Name} Content";
        public override string Description => $"Enables to view {ContentType.Name} content in website or via API.";
        public override string CategoryName => $"Content Management";
        public override string SubCategoryName => $"{ContentType.Name}";
        public override bool AuthenticatedUserHasThisByDefault => true;
        public override bool GuestUserHasThisByDefault => true;

        public static string GetIdFor(string contentTypeId) {
            return $"Content({contentTypeId}):View";
        }
    }
}