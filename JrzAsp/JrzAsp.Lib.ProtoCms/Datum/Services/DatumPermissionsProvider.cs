using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Permission.Services;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public class DatumPermissionsProvider : IPermissionsProvider {
        public IEnumerable<ProtoPermission> DefinePermissions() {
            foreach (var dt in DatumType.DefinedTypes) {
                yield return dt.ViewPermissionBase;
                yield return dt.ListPermissionBase;
                foreach (var mp in dt.ModifyPermissionsBase) {
                    yield return mp;
                }
            }
        }
    }
}