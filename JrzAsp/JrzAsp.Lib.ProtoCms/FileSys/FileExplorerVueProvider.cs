using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.FileSys.Permissions;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.ProtoCms.Vue.Services;

namespace JrzAsp.Lib.ProtoCms.FileSys {
    public class FileExplorerVueProvider : IVueProvider {
        public const string MEDIA_MENU_CATEGORY_ID = "protocms-media-menu-category";

        private readonly IProtoCmsMainUrlsProvider _urlsProv;

        public FileExplorerVueProvider(IProtoCmsMainUrlsProvider urlsProv) {
            _urlsProv = urlsProv;
        }

        public IEnumerable<VueMenuItemCategory> DefineSidebarMenuCategories() {
            yield return new VueMenuItemCategory {Id = MEDIA_MENU_CATEGORY_ID, Label = "Media", RowOrder = 50000};
        }

        public IEnumerable<VueMenuItem> DefineSidebarMenuItems() {
            yield return new VueMenuItem {
                CategoryId = MEDIA_MENU_CATEGORY_ID,
                Label = "File Explorer",
                Href = _urlsProv.GenerateManageFileExplorerUrl(),
                IconCssClass = "fa fa-folder-open",
                RowOrder = 0,
                IsVisible = ctx => ctx.UserHasPermission(ManageFileExplorerPermission.PERMISSION_ID)
            };
        }

        public IEnumerable<VueMenuItem> DefineUserMenuItems() {
            yield break;
        }

        public IEnumerable<string> DefineRequiredPartialViews() {
            yield break;
        }
    }
}