using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using JrzAsp.Lib.ProtoCms.Permission.Services;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.ProtoCms.User.Services;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.WebApi.Security {
    /// <summary>
    ///     Authorize according to user's permissions.
    ///     For use in WebAPI ApiController.
    /// </summary>
    public class AuthorizeByProtoPermissionsAttribute : AuthorizeAttribute {

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="requiredPermissionIds">required permission ids</param>
        public AuthorizeByProtoPermissionsAttribute(params string[] requiredPermissionIds) {
            RequiredPermissionIds = requiredPermissionIds;
        }

        /// <summary>
        ///     Permission ids that's required
        /// </summary>
        protected string[] RequiredPermissionIds { get; }

        protected override bool IsAuthorized(HttpActionContext actionContext) {
            if (!base.IsAuthorized(actionContext)) return false;

            var httpContext = new HttpContextWrapper(HttpContext.Current);
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