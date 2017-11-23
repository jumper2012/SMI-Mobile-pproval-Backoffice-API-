using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumPermissionsHandler : BaseDatumPermissionsHandler<ApplicationRole> {
        public override decimal Priority => 0;

        public override DatumPermissionCustomProps ViewPermissionCustomProps(DatumType<ApplicationRole> datumType) {
            return new DatumPermissionCustomProps(
                "View User Role", "Allow viewing user role data.", "Account", "User Role");
        }

        public override DatumPermissionCustomProps ListPermissionCustomProps(DatumType<ApplicationRole> datumType) {
            return new DatumPermissionCustomProps(
                "List User Role", "Allow listing user role data.", "Account", "User Role");
        }

        public override DatumPermissionCustomProps ModifyPermissionCustomProps(DatumModifyOperation modifyOperation,
            DatumType<ApplicationRole> datumType) {
            if (modifyOperation.IsCreateOperation()) {
                return new DatumPermissionCustomProps(
                    "Create User Role", "Allow creating user role data.", "Account", "User Role");
            }
            if (modifyOperation.IsUpdateOperation()) {
                return new DatumPermissionCustomProps(
                    "Update User Role", "Allow updating user role data.", "Account", "User Role");
            }
            if (modifyOperation.IsDeleteOperation()) {
                return new DatumPermissionCustomProps(
                    "Delete User Role", "Allow deleting user role data.", "Account", "User Role");
            }
            return null;
        }
    }
}