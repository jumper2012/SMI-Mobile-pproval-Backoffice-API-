using System;
using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.ProtoCms.User.Services;

namespace JrzAsp.Lib.ProtoCms.Permission.Services {
    public class PermissionManager : IPermissionManager {
        protected readonly IProtoCmsDbContext _dbContext;
        private readonly object _initLock = new object();
        protected readonly IEnumerable<IPermissionsProvider> _permProvs;
        protected readonly IProtoUserManager _userMgr;
        private bool _isInitialized;

        public PermissionManager(IProtoCmsDbContext dbContext, IEnumerable<IPermissionsProvider> permProvs,
            IProtoUserManager userMgr) {
            _dbContext = dbContext;
            _permProvs = permProvs;
            _userMgr = userMgr;
            Initialize();
        }

        public IEnumerable<ProtoPermission> AllPermissions { get; protected set; }

        public IEnumerable<string> GetUserPermissions(string userId) {
            if (userId == null) return GetRolesPermissions(new[] {RequiredRoleInfo.Guest.Name});
            var user = _dbContext.UsersProjected.FirstOrDefault(u => u.Id == userId);
            if (user == null) return GetRolesPermissions(new[] {RequiredRoleInfo.Guest.Name});
            var roles = _userMgr.GetProtoRoleNames(user.Id).ToArray();
            if (!roles.Contains(RequiredRoleInfo.Authenticated.Name)) {
                var r = new string[roles.Length + 1];
                r[0] = RequiredRoleInfo.Authenticated.Name;
                Array.Copy(roles, 0, r, 1, roles.Length);
                roles = r;
            }
            return GetRolesPermissions(roles);
        }

        public IEnumerable<string> GetRolesPermissions(string[] roleNames) {
            if (roleNames == null) return new string[0];

            var hasGuest = false;
            var hasAuthenticated = false;
            var hasSuperAdmin = false;

            foreach (var rn in roleNames) {
                if (rn == RequiredRoleInfo.Authenticated.Name) {
                    hasAuthenticated = true;
                } else if (rn == RequiredRoleInfo.Guest.Name) {
                    hasGuest = true;
                } else if (rn == RequiredRoleInfo.SuperAdmin.Name) hasSuperAdmin = true;
            }

            if (hasSuperAdmin) return AllPermissions.Select(p => p.Id).ToArray();

            var permIds = new List<string>();

            var definedPerms = (
                from ro in _dbContext.RolesProjected
                join pm in _dbContext.ProtoPermissionsMaps on ro.Name equals pm.RoleName
                where roleNames.Contains(ro.Name)
                select pm
            ).ToArray();

            // if multiple roles define conflicting hasPermission for same permission id
            // then that hasPermission is automatically false
            var mergedHasPermissions = new Dictionary<string, bool>();

            foreach (var dpr in definedPerms) {
                if (!mergedHasPermissions.ContainsKey(dpr.Id)) {
                    mergedHasPermissions[dpr.Id] = dpr.HasPermission;
                } else if (!dpr.HasPermission) mergedHasPermissions[dpr.Id] = false;
            }

            foreach (var pr in AllPermissions) {
                var dpr = definedPerms.FirstOrDefault(d => d.PermissionId == pr.Id);
                if (dpr == null) {
                    if (hasGuest && pr.GuestUserHasThisByDefault ||
                        hasAuthenticated && pr.AuthenticatedUserHasThisByDefault) {
                        permIds.Add(pr.Id);
                    }
                } else if (mergedHasPermissions[dpr.Id]) permIds.Add(pr.Id);
            }

            return permIds.Distinct().ToArray();

        }

        public bool UserHasAllPermissions(string userId, params string[] permissionIds) {
            var userPerms = GetUserPermissions(userId).ToArray();
            foreach (var pid in permissionIds) if (!userPerms.Contains(pid)) return false;
            return true;
        }

        public bool UserHasAnyPermission(string userId, params string[] permissionIds) {
            var userPerms = GetUserPermissions(userId).ToArray();
            foreach (var pid in permissionIds) if (userPerms.Contains(pid)) return true;
            return false;
        }

        public bool RoleHasAllPermissions(string roleName, params string[] permissionIds) {
            var rolePerms = GetRolesPermissions(new[] {roleName}).ToArray();
            foreach (var pid in permissionIds) if (!rolePerms.Contains(pid)) return false;
            return true;
        }

        public bool RoleHasAnyPermission(string roleName, params string[] permissionIds) {
            var rolePerms = GetRolesPermissions(new[] {roleName}).ToArray();
            foreach (var pid in permissionIds) if (rolePerms.Contains(pid)) return true;
            return false;
        }

        private void Initialize() {
            lock (_initLock) {
                if (_isInitialized) return;

                AllPermissions = _permProvs.SelectMany(p => p.DefinePermissions()).ToArray();

                _isInitialized = true;
            }
        }
    }
}