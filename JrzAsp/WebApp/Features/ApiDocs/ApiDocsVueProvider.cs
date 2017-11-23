using System.Collections.Generic;
using JrzAsp.Lib.RazorTools;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.ProtoCms.Vue.Services;

namespace WebApp.Features.ApiDocs {
    public class ApiDocsVueProvider : IVueProvider {
        public const string API_DOC_MENU_CATEGORY_ID = "api-doc-menu-category";

        public IEnumerable<VueMenuItemCategory> DefineSidebarMenuCategories() {
            yield return new VueMenuItemCategory {
                Id = API_DOC_MENU_CATEGORY_ID,
                Label = "API Documentation",
                RowOrder = 1000000
            };
        }

        public IEnumerable<VueMenuItem> DefineSidebarMenuItems() {
            yield return new VueMenuItem {
                CategoryId = API_DOC_MENU_CATEGORY_ID,
                Href = UrlHelperGlobal.Self.Action("Index", "Help", new {area = "HelpPage"}),
                Label = "API Doc",
                IconCssClass = "fa fa-code",
                HtmlTarget = "_blank"
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