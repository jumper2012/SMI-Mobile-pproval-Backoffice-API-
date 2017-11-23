using System;
using System.Web.Mvc;
using JrzAsp.Lib.RazorTools;

namespace WebApp.Features.MetronicTheme {
    public static class MetronicLayoutSections {
        public const string STYLE_0A_GLOBAL_MANDATORY = nameof(STYLE_0A_GLOBAL_MANDATORY);
        public const string STYLE_0B_GLOBAL_MANDATORY = nameof(STYLE_0B_GLOBAL_MANDATORY);
        public const string STYLE_1A_PAGE_LEVEL = nameof(STYLE_1A_PAGE_LEVEL);
        public const string STYLE_1B_PAGE_LEVEL = nameof(STYLE_1B_PAGE_LEVEL);
        public const string STYLE_2A_THEME_GLOBAL = nameof(STYLE_2A_THEME_GLOBAL);
        public const string STYLE_2B_THEME_GLOBAL = nameof(STYLE_2B_THEME_GLOBAL);
        public const string STYLE_3A_THEME_LAYOUT = nameof(STYLE_3A_THEME_LAYOUT);
        public const string STYLE_3B_THEME_LAYOUT = nameof(STYLE_3B_THEME_LAYOUT);
        public const string STYLE_9Z_OTHERS = nameof(STYLE_9Z_OTHERS);

        public const string SCRIPT_0A_CORE_PLUGINS = nameof(SCRIPT_0A_CORE_PLUGINS);
        public const string SCRIPT_0B_CORE_PLUGINS = nameof(SCRIPT_0B_CORE_PLUGINS);
        public const string SCRIPT_1A_PAGE_PLUGINS = nameof(SCRIPT_1A_PAGE_PLUGINS);
        public const string SCRIPT_1B_PAGE_PLUGINS = nameof(SCRIPT_1B_PAGE_PLUGINS);
        public const string SCRIPT_2A_THEME_GLOBAL = nameof(SCRIPT_2A_THEME_GLOBAL);
        public const string SCRIPT_2B_THEME_GLOBAL = nameof(SCRIPT_2B_THEME_GLOBAL);
        public const string SCRIPT_3A_PAGE = nameof(SCRIPT_3A_PAGE);
        public const string SCRIPT_3B_PAGE = nameof(SCRIPT_3B_PAGE);
        public const string SCRIPT_4A_THEME_LAYOUT = nameof(SCRIPT_4A_THEME_LAYOUT);
        public const string SCRIPT_4B_THEME_LAYOUT = nameof(SCRIPT_4B_THEME_LAYOUT);
        public const string SCRIPT_9Z_OTHERS = nameof(SCRIPT_9Z_OTHERS);

        public static string CmsDashUrl(this UrlHelper urlHelper) {
            return urlHelper.Action("Index", "Home", new {area = "CmsDash"});
        }
    
        public static IDisposable AtBeginGlobalMandatoryStyles(this HtmlHelper htmlHelper,
            string uniqueSectionId = null) {
            return htmlHelper.Section(STYLE_0A_GLOBAL_MANDATORY, uniqueSectionId);
        }

        public static IDisposable
            AtEndGlobalMandatoryStyles(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(STYLE_0B_GLOBAL_MANDATORY, uniqueSectionId);
        }

        public static IDisposable AtBeginPageLevelStyles(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(STYLE_1A_PAGE_LEVEL, uniqueSectionId);
        }

        public static IDisposable AtEndPageLevelStyles(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(STYLE_1B_PAGE_LEVEL, uniqueSectionId);
        }

        public static IDisposable AtBeginThemeGlobalStyles(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(STYLE_2A_THEME_GLOBAL, uniqueSectionId);
        }

        public static IDisposable AtEndThemeGlobalStyles(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(STYLE_2B_THEME_GLOBAL, uniqueSectionId);
        }

        public static IDisposable AtBeginThemeLayoutStyles(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(STYLE_3A_THEME_LAYOUT, uniqueSectionId);
        }

        public static IDisposable AtEndThemeLayoutStyles(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(STYLE_3B_THEME_LAYOUT, uniqueSectionId);
        }

        public static IDisposable AtOtherStyles(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(STYLE_9Z_OTHERS, uniqueSectionId);
        }

        public static IDisposable AtBeginCorePluginsScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_0A_CORE_PLUGINS, uniqueSectionId);
        }

        public static IDisposable AtEndCorePluginsScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_0B_CORE_PLUGINS, uniqueSectionId);
        }

        public static IDisposable AtBeginPagePluginsScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_1A_PAGE_PLUGINS, uniqueSectionId);
        }

        public static IDisposable AtEndPagePluginsScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_1B_PAGE_PLUGINS, uniqueSectionId);
        }

        public static IDisposable AtBeginThemeGlobalScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_2A_THEME_GLOBAL, uniqueSectionId);
        }

        public static IDisposable AtEndThemeGlobalScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_2B_THEME_GLOBAL, uniqueSectionId);
        }

        public static IDisposable AtBeginPageScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_3A_PAGE, uniqueSectionId);
        }

        public static IDisposable AtEndPageScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_3B_PAGE, uniqueSectionId);
        }

        public static IDisposable AtBeginThemeLayoutScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_4A_THEME_LAYOUT, uniqueSectionId);
        }

        public static IDisposable AtEndThemeLayoutScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_4B_THEME_LAYOUT, uniqueSectionId);
        }

        public static IDisposable AtOtherScripts(this HtmlHelper htmlHelper, string uniqueSectionId = null) {
            return htmlHelper.Section(SCRIPT_9Z_OTHERS, uniqueSectionId);
        }
    }
}