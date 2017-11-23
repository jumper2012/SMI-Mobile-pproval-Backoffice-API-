using System.Collections.Generic;
using System.Linq;
using System.Web;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.ProtoCms.Vue.Services;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public class DatumVueProvider : IVueProvider {
        public IEnumerable<VueMenuItemCategory> DefineSidebarMenuCategories() {
            yield return new VueMenuItemCategory {
                Id = DatumType.DATUM_MENU_CATEGORY_ID,
                Label = "Data",
                RowOrder = 10000
            };
        }

        public IEnumerable<VueMenuItem> DefineSidebarMenuItems() {
            return DatumType.DefinedTypes.SelectMany(x => x.MenuItemToAccessContentType);
        }

        public IEnumerable<VueMenuItem> DefineUserMenuItems() {
            yield break;
        }

        public IEnumerable<string> DefineRequiredPartialViews() {
            yield break;
        }

        protected void CheckUserHasPermissionToListDatum(DatumType dt, ProtoCmsRuntimeContext rctx) {
            if (!rctx.UserHasPermission(dt.ListPermissionBase.Id)) {
                throw new HttpException(403, $"ProtoCMS: user is forbidden to list datum type '{dt.Id}'.");
            }
        }
    }
}