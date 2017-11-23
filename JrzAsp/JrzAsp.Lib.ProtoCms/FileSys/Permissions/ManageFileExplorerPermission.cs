using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.FileSys.Permissions {
    public class ManageFileExplorerPermission : ProtoPermission {
        public const string PERMISSION_ID = "ManageFileExplorer";
        public override string Id => PERMISSION_ID;
        public override string DisplayName => "Manage File Explorer";
        public override string Description => "Allow to manage files and directories using the file explorer.";
        public override string CategoryName => "System";
        public override string SubCategoryName => "File Explorer";
        public override bool AuthenticatedUserHasThisByDefault => false;
        public override bool GuestUserHasThisByDefault => false;
    }
}