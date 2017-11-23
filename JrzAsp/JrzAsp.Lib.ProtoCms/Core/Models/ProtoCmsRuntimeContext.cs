using System.Collections.Generic;
using System.Linq;
using System.Web;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Permission.Services;
using JrzAsp.Lib.ProtoCms.User.Models;
using JrzAsp.Lib.ProtoCms.User.Services;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Core.Models {
    public class ProtoCmsRuntimeContext {

        private static readonly string CONTEXT_KEY = $"{typeof(ProtoCmsRuntimeContext)}:Current";

        private readonly IDictionary<string, bool> _permMap = new Dictionary<string, bool>();

        public ProtoCmsRuntimeContext(HttpContextWrapper httpContext, ProtoUserProjection currentUser,
            ProtoPermission[] userPermissions) {
            HttpContext = httpContext;
            CurrentUser = currentUser;
            UserPermissions = userPermissions;
        }

        public HttpContextWrapper HttpContext { get; }
        public ProtoUserProjection CurrentUser { get; }
        public ProtoPermission[] UserPermissions { get; }

        public static ProtoCmsRuntimeContext Current {
            get {
                var hc = new HttpContextWrapper(System.Web.HttpContext.Current);

                var existing = hc.Items[CONTEXT_KEY] as ProtoCmsRuntimeContext;
                if (existing?.CurrentUser != null) return existing;

                var di = ProtoEngine.GetDependencyProvider();
                var usrMgr = di.GetService(typeof(IProtoUserManager)).DirectCastTo<IProtoUserManager>();
                var usr = usrMgr.GetAuthenticatedProtoUser(hc);
                var permMgr = di.GetService(typeof(IPermissionManager)).DirectCastTo<IPermissionManager>();
                var perms = (from pp in permMgr.AllPermissions
                    join pid in permMgr.GetUserPermissions(usr?.Id) on pp.Id equals pid
                    select pp).Distinct();

                existing = new ProtoCmsRuntimeContext(hc, usr, perms.ToArray());
                hc.Items[CONTEXT_KEY] = existing;
                return existing;
            }
        }

        public bool UserHasPermission(string permissionId) {
            bool hasPerm;
            if (_permMap.TryGetValue(permissionId, out hasPerm)) {
                return hasPerm;
            }
            hasPerm = UserPermissions.Any(x => x.Id == permissionId);
            _permMap[permissionId] = hasPerm;
            return hasPerm;
        }
    }
}