using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Core.Permissions {
    public sealed class AccessCmsPermission : ProtoPermission {
        public const string PERMISSION_ID = "AccessCmsDash";
        public override string Id => PERMISSION_ID;
        public override string DisplayName => "Access CMS";
        public override string Description => "Whether the user can access CMS";
        public override string CategoryName => "CMS";
        public override string SubCategoryName => "Dashboard";
        public override bool AuthenticatedUserHasThisByDefault => false;
        public override bool GuestUserHasThisByDefault => false;
    }
}