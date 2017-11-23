using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumPermissionsHandler : BaseDatumPermissionsHandler<ApplicationUser> {
        public override decimal Priority => 0;

        public override DatumPermissionCustomProps ViewPermissionCustomProps(DatumType<ApplicationUser> datumType) {
            return new DatumPermissionCustomProps(
                "View User",
                "Allow viewing user data.",
                "Account",
                "User"
            );
        }

        public override DatumPermissionCustomProps ListPermissionCustomProps(DatumType<ApplicationUser> datumType) {
            return new DatumPermissionCustomProps(
                "List User",
                "Allow listing user data.",
                "Account",
                "User"
            );
        }

        public override DatumPermissionCustomProps ModifyPermissionCustomProps(DatumModifyOperation modifyOperation,
            DatumType<ApplicationUser> datumType) {
            if (modifyOperation.IsCreateOperation()) {
                return new DatumPermissionCustomProps(
                    "Create User", "Allow creating user data.", "Account", "User");
            }
            if (modifyOperation.IsUpdateOperation()) {
                return new DatumPermissionCustomProps(
                    "Update User", "Allow updating user data.", "Account", "User");
            }
            if (modifyOperation.IsDeleteOperation()) {
                return new DatumPermissionCustomProps(
                    "Delete User", "Allow deleting user data.", "Account", "User");
            }
            return null;
        }
    }
}