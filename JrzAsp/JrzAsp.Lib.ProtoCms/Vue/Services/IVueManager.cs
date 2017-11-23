using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Vue.Services {
    public interface IVueManager : IGlobalSingletonDependency {
        VueMenuItemCategory[] SidebarMenuCategories { get; }
        VueMenuItem[] SidebarMenuItems { get; }
        VueMenuItem[] UserMenuItems { get; }
        string[] RequiredPartialViews { get; }
    }
}