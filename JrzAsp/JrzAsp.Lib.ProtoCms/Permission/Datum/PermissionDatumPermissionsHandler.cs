using System;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Permission.Datum {
    public class PermissionDatumPermissionsHandler : BaseDatumPermissionsHandler<ProtoPermission> {
        public override decimal Priority => 0;

        public override DatumPermissionCustomProps ViewPermissionCustomProps(DatumType<ProtoPermission> datumType) {
            return new DatumPermissionCustomProps("View Role Permission", "Allow viewing role permission data.",
                "Account", "Role Permission");
        }

        public override DatumPermissionCustomProps ListPermissionCustomProps(DatumType<ProtoPermission> datumType) {
            return new DatumPermissionCustomProps("List Role Permission", "Allow listing role permission data.",
                "Account", "Role Permission");
        }

        public override DatumPermissionCustomProps ModifyPermissionCustomProps(DatumModifyOperation modifyOperation,
            DatumType<ProtoPermission> datumType) {
            // not used
            return null;
        }
    }
}