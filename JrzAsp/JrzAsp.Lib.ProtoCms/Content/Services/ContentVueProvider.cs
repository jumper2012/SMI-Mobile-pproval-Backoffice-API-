using System.Collections.Generic;
using System.Linq;
using System.Web;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Permissions;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.ProtoCms.Vue.Services;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public class ContentVueProvider : IVueProvider {

        public IEnumerable<VueMenuItemCategory> DefineSidebarMenuCategories() {
            yield return new VueMenuItemCategory {
                Id = ContentType.CONTENT_MENU_CATEGORY_ID,
                Label = "Contents",
                RowOrder = 0
            };
        }

        public IEnumerable<VueMenuItem> DefineSidebarMenuItems() {
            return ContentType.DefinedTypes.SelectMany(x => x.MenuItemToAccessContentType);
        }

        public IEnumerable<VueMenuItem> DefineUserMenuItems() {
            yield break;
        }

        public IEnumerable<string> DefineRequiredPartialViews() {
            yield break;
        }

        protected void CheckUserHasPermissionToListContent(ContentType ct, ProtoCmsRuntimeContext rctx) {
            var listPerm = new ListContentPermission(ct);
            if (!rctx.UserHasPermission(listPerm.Id)) {
                throw new HttpException(403, $"ProtoCMS: user is forbidden to list content type '{ct.Id}'.");
            }
        }
    }
}