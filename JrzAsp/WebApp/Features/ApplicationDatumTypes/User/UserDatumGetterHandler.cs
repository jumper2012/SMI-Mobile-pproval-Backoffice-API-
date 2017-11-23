using System.Linq;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using Microsoft.AspNet.Identity;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumGetterHandler : BaseDatumGetterHandler<ApplicationUser> {
        private readonly ApplicationUserManager _userMgr;

        public UserDatumGetterHandler(ApplicationUserManager userMgr) {
            _userMgr = userMgr;
        }

        public override decimal Priority => 0;

        public override string GetDatumId(ApplicationUser datum) {
            return datum.Id;
        }

        public override IQueryable<ApplicationUser> BuildBaseQuery() {
            return _userMgr.Users;
        }

        public override ApplicationUser CreateInMemory(string datumId) {
            return new ApplicationUser {Id = datumId};
        }

        public override ApplicationUser FindById(string id) {
            return _userMgr.FindById(id);
        }
    }
}