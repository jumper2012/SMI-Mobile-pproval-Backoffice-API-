using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Vue.Services {
    public class VueManager : IVueManager {
        private readonly IEnumerable<IVueProvider> _providers;

        public VueManager(IEnumerable<IVueProvider> providers) {
            _providers = providers;
            var provs = _providers.ToArray();
            SidebarMenuCategories = provs.SelectMany(x => x.DefineSidebarMenuCategories())
                .OrderBy(x => x.RowOrder).ThenBy(x => x.Label).ToArray();
            SidebarMenuItems = provs.SelectMany(x => x.DefineSidebarMenuItems())
                .OrderBy(x => x.RowOrder).ThenBy(x => x.Label).ToArray();
            UserMenuItems = provs.SelectMany(x => x.DefineUserMenuItems())
                .OrderBy(x => x.RowOrder).ThenBy(x => x.Label).ToArray();
            RequiredPartialViews = provs.SelectMany(x => x.DefineRequiredPartialViews()).ToArray();
        }

        public VueMenuItemCategory[] SidebarMenuCategories { get; }
        public VueMenuItem[] SidebarMenuItems { get; }
        public VueMenuItem[] UserMenuItems { get; }
        public string[] RequiredPartialViews { get; }
    }
}