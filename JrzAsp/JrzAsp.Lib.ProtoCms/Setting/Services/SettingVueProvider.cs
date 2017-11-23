using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Setting.Permissions;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.ProtoCms.Vue.Services;

namespace JrzAsp.Lib.ProtoCms.Setting.Services {
    public class SettingVueProvider : IVueProvider {
        public const string SETTING_MENU_CATEGORY_ID = "protocms-settings-menu-category";

        private readonly ISiteSettingManager _ssmgr;
        private readonly IProtoCmsMainUrlsProvider _mainUrlsProv;

        public SettingVueProvider(ISiteSettingManager ssmgr, IProtoCmsMainUrlsProvider mainUrlsProv) {
            _ssmgr = ssmgr;
            _mainUrlsProv = mainUrlsProv;
        }

        public IEnumerable<VueMenuItemCategory> DefineSidebarMenuCategories() {
            yield return new VueMenuItemCategory {
                Id = SETTING_MENU_CATEGORY_ID,
                Label = "Settings",
                RowOrder = 100000
            };
        }

        public IEnumerable<VueMenuItem> DefineSidebarMenuItems() {
            foreach (var ssp in _ssmgr.SettingSpecs) {
                yield return new VueMenuItem {
                    Id = $"protocms-setting-{ssp.Id}",
                    CategoryId = SETTING_MENU_CATEGORY_ID,
                    Label = ssp.Name,
                    Href = _mainUrlsProv.GenerateManageSiteSettingUrl(ssp.Id),
                    IconCssClass = "fa fa-cog",
                    IsVisible = ctx => ctx.UserHasPermission(ModifySiteSettingPermission.GetIdFor(ssp.Id))
                };
            }
        }

        public IEnumerable<VueMenuItem> DefineUserMenuItems() {
            yield break;
        }

        public IEnumerable<string> DefineRequiredPartialViews() {
            yield break;
        }
    }
}