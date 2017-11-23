using System.Collections.Generic;

namespace JrzAsp.Lib.ProtoCms.Vue.Models {
    public class VueMenus {

        public VueMenus(IEnumerable<VueMenuItemCategory> mainMenuCategories,
            IEnumerable<VueMenuItem> mainMenuItems, IEnumerable<VueMenuItemCategory> userMenuCategories,
            IEnumerable<VueMenuItem> userMenuItems) {
            MainMenuCategories = mainMenuCategories;
            MainMenuItems = mainMenuItems;
            UserMenuCategories = userMenuCategories;
            UserMenuItems = userMenuItems;
        }

        public IEnumerable<VueMenuItemCategory> MainMenuCategories { get; }
        public IEnumerable<VueMenuItem> MainMenuItems { get; }
        public IEnumerable<VueMenuItemCategory> UserMenuCategories { get; }
        public IEnumerable<VueMenuItem> UserMenuItems { get; }
    }
}