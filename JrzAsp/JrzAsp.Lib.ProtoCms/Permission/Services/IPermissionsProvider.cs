using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Permission.Services {
    /// <summary>
    ///     Provide permissions to ProtoCMS
    ///     Implemented class must be registered to DI container
    /// </summary>
    public interface IPermissionsProvider : IGlobalSingletonDependency {
        /// <summary>
        ///     Define the permissions
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProtoPermission> DefinePermissions();
    }
}