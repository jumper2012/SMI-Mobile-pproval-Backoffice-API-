using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Permission.Models;

namespace JrzAsp.Lib.ProtoCms.Permission.Services {
    /// <summary>
    ///     Provide services related to ProtoCMS permission management
    /// </summary>
    public interface IPermissionManager : IPerRequestDependency {

        /// <summary>
        ///     Get all defined permissions in ProtoCMS
        /// </summary>
        IEnumerable<ProtoPermission> AllPermissions { get; }

        /// <summary>
        ///     Get permissions that the user has
        /// </summary>
        /// <param name="userId">The user id. If null then it means any user that has Guest role</param>
        /// <returns>A collection of permission id</returns>
        IEnumerable<string> GetUserPermissions(string userId);

        /// <summary>
        ///     Get permissions that one or more roles have
        /// </summary>
        /// <param name="roleNames">A collection of role name</param>
        /// <returns>A collection of permission id</returns>
        IEnumerable<string> GetRolesPermissions(string[] roleNames);

        /// <summary>
        ///     Check whether a user has all permissions in param
        /// </summary>
        /// <param name="userId">The user id, if null then it means any user in Guest role.</param>
        /// <param name="permissionIds">Permission ids</param>
        /// <returns>True if has all permission</returns>
        bool UserHasAllPermissions(string userId, params string[] permissionIds);

        /// <summary>
        ///     Check whether a user has any permission in param
        /// </summary>
        /// <param name="userId">The user id, if null then it means any user in Guest role.</param>
        /// <param name="permissionIds">Permission ids</param>
        /// <returns>True if has at least 1 permission in param</returns>
        bool UserHasAnyPermission(string userId, params string[] permissionIds);

        /// <summary>
        ///     Check whether a role has all permissions in param
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <param name="permissionIds">Permission ids</param>
        /// <returns>True if has all permission</returns>
        bool RoleHasAllPermissions(string roleName, params string[] permissionIds);

        /// <summary>
        ///     Check whether a role has any permission in param
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <param name="permissionIds">Permission ids</param>
        /// <returns>True if has at least 1 permission in param</returns>
        bool RoleHasAnyPermission(string roleName, params string[] permissionIds);
    }
}