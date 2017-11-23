using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Vue.Services {
    /// <summary>
    ///     Provide vue info to proto CMS
    ///     Implementor must be registered to DI
    /// </summary>
    public interface IVueProvider : IPerRequestDependency {
        /// <summary>
        ///     Define vue sidebar menu categories in CMS
        /// </summary>
        /// <returns></returns>
        IEnumerable<VueMenuItemCategory> DefineSidebarMenuCategories();

        /// <summary>
        ///     Define vue sidebar menu in CMS
        /// </summary>
        /// <returns></returns>
        IEnumerable<VueMenuItem> DefineSidebarMenuItems();

        /// <summary>
        ///     Defined menu items to be shown as user account dropdown
        /// </summary>
        /// <returns></returns>
        IEnumerable<VueMenuItem> DefineUserMenuItems();

        /// <summary>
        ///     Defined partial view names to be included in CMS main layout
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> DefineRequiredPartialViews();
    }
}