using System.Linq;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Permission.Services;

namespace JrzAsp.Lib.ProtoCms.Permission.Datum {
    public class PermissionDatumGetterHandler : BaseDatumGetterHandler<ProtoPermission> {
        private readonly IPermissionManager _perMgr;

        public PermissionDatumGetterHandler(IPermissionManager perMgr) {
            _perMgr = perMgr;
        }

        public override decimal Priority => 0;

        public override string GetDatumId(ProtoPermission datum) {
            return datum.Id;
        }

        public override IQueryable<ProtoPermission> BuildBaseQuery() {
            return _perMgr.AllPermissions.AsQueryable();
        }

        public override ProtoPermission CreateInMemory(string datumId) {
            // not supported
            return null;
        }

        public override ProtoPermission FindById(string id) {
            return _perMgr.AllPermissions.FirstOrDefault(x => x.Id == id);
        }
    }
}