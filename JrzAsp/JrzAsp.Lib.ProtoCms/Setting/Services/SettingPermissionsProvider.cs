using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Permission.Services;
using JrzAsp.Lib.ProtoCms.Setting.Permissions;

namespace JrzAsp.Lib.ProtoCms.Setting.Services {
    public class SettingPermissionsProvider : IPermissionsProvider {
        private readonly ISiteSettingManager _ssmgr;

        public SettingPermissionsProvider(ISiteSettingManager ssmgr) {
            _ssmgr = ssmgr;
        }

        public IEnumerable<ProtoPermission> DefinePermissions() {
            foreach (var ssp in _ssmgr.SettingSpecs) {
                yield return new ModifySiteSettingPermission(ssp);
                yield return new ViewSiteSettingPermission(ssp);
            }
        }
    }
}