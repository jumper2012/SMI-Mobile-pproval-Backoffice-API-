using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Permission.Services;

namespace JrzAsp.Lib.ProtoCms.FileSys.Permissions {
    public class FileExplorerPermissionsProvider : IPermissionsProvider {
        public IEnumerable<ProtoPermission> DefinePermissions() {
            yield return new ManageFileExplorerPermission();
        }
    }
}