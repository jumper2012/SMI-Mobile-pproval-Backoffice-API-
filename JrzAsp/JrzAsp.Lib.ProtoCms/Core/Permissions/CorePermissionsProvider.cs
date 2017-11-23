using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Permission.Services;

namespace JrzAsp.Lib.ProtoCms.Core.Permissions {
    public class CorePermissionsProvider : IPermissionsProvider {

        public const string ACCESS_CMS_PERMISSION = AccessCmsPermission.PERMISSION_ID;
        public static AccessCmsPermission AccessCmsPermission => new AccessCmsPermission();

        public IEnumerable<ProtoPermission> DefinePermissions() {
            yield return AccessCmsPermission;
        }
    }
}