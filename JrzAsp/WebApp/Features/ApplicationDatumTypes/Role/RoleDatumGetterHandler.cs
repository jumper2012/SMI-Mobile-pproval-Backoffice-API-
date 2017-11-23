using System.Linq;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using Microsoft.AspNet.Identity;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumGetterHandler : BaseDatumGetterHandler<ApplicationRole> {
        private readonly ApplicationRoleManager _roleMgr;

        public RoleDatumGetterHandler(ApplicationRoleManager roleMgr) {
            _roleMgr = roleMgr;
        }

        public override decimal Priority => 0;

        public override string GetDatumId(ApplicationRole datum) {
            return datum.Id;
        }

        public override IQueryable<ApplicationRole> BuildBaseQuery() {
            return _roleMgr.Roles;
        }

        public override ApplicationRole CreateInMemory(string datumId) {
            return new ApplicationRole {
                Id = datumId
            };
        }

        public override ApplicationRole FindById(string id) {
            return _roleMgr.FindById(id);
        }
    }
}