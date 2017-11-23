using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.ProtoCms.Vue.Services;

namespace WebApp.Features.VueComponents {
    public class VueComponentsProvider : IVueProvider {
        public const string FILEPATH_VUE_MIXINS = "~/Views/Shared/_VueComponents/Mixins";
        public const string FILEPATH_VUE_PAGES = "~/Views/Shared/_VueComponents/Pages";
        public const string FILEPATH_VUE_WIDGETS = "~/Views/Shared/_VueComponents/Widgets";
        public const string FILEPATH_VUE_FORM_FIELDS = "~/Views/Shared/_VueComponents/FormFields";

        public IEnumerable<VueMenuItemCategory> DefineSidebarMenuCategories() {
            yield break;
        }

        public IEnumerable<VueMenuItem> DefineSidebarMenuItems() {
            yield break;
        }

        public IEnumerable<VueMenuItem> DefineUserMenuItems() {
            yield break;
        }

        public IEnumerable<string> DefineRequiredPartialViews() {
            var basePath = HostingEnvironment.MapPath("~");
            
            var mixins = Directory.EnumerateFiles(HostingEnvironment.MapPath(FILEPATH_VUE_MIXINS))
                .Where(x => x.EndsWith(".cshtml")).Select(x => x.Replace(basePath, "~/").Replace("\\", "/"));
            var pages = Directory.EnumerateFiles(HostingEnvironment.MapPath(FILEPATH_VUE_PAGES))
                .Where(x => x.EndsWith(".cshtml")).Select(x => x.Replace(basePath, "~/").Replace("\\", "/"));
            var widgets = Directory.EnumerateFiles(HostingEnvironment.MapPath(FILEPATH_VUE_WIDGETS))
                .Where(x => x.EndsWith(".cshtml")).Select(x => x.Replace(basePath, "~/").Replace("\\", "/"));
            var formFields = Directory.EnumerateFiles(HostingEnvironment.MapPath(FILEPATH_VUE_FORM_FIELDS))
                .Where(x => x.EndsWith(".cshtml")).Select(x => x.Replace(basePath, "~/").Replace("\\", "/"));
            
            foreach (var p in mixins) {
                yield return p;
            }
            foreach (var p in pages) {
                yield return p;
            }
            foreach (var p in widgets) {
                yield return p;
            }
            foreach (var p in formFields) {
                yield return p;
            }
        }
    }
}