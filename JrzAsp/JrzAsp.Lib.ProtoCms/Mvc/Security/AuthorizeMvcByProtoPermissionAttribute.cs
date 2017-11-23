using System.Web;
using System.Web.Mvc;
using JrzAsp.Lib.TypeUtilities;
using JrzAsp.Lib.ProtoCms.Permission.Services;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.ProtoCms.User.Services;

namespace JrzAsp.Lib.ProtoCms.Mvc.Security {
    /// <summary>
    ///     Authorize according to user's permissions.
    ///     For use in MVC Controller.
    /// </summary>
    public class AuthorizeMvcByProtoPermissionAttribute : AuthorizeAttribute {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="requiredPermissionIds">required permission ids</param>
        public AuthorizeMvcByProtoPermissionAttribute(params string[] requiredPermissionIds) {
            RequiredPermissionIds = requiredPermissionIds;
        }

        /// <summary>
        ///     Permission ids that's required
        /// </summary>
        protected string[] RequiredPermissionIds { get; }

        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            if (!base.AuthorizeCore(httpContext)) return false;

            var di = ProtoEngine.GetDependencyProvider();

            var perMgr = di.GetService(typeof(IPermissionManager))
                .DirectCastTo<IPermissionManager>();

            var usrMgr = di.GetService(typeof(IProtoUserManager))
                .DirectCastTo<IProtoUserManager>();

            var authUser = usrMgr.GetAuthenticatedProtoUser(httpContext);
            if (authUser == null) {
                return perMgr.RoleHasAllPermissions(RequiredRoleInfo.Guest.Name, RequiredPermissionIds);
            }
            return perMgr.UserHasAllPermissions(authUser.Id, RequiredPermissionIds);
        }
    }
}