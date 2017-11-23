using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Mvc;
using JrzAsp.Lib.StrictAreaRegistration;

namespace JrzAsp.Mvc.Captcha {
    public class CaptchaAreaRegistration : BaseStrictAreaRegistration {
        public override string AreaName => MyAppSettings.AreaName;
        public override string BaseRoute => MyAppSettings.BaseRoute;
        public override Assembly AreaAssembly => Assembly.GetExecutingAssembly();
        public override string AreaControllersNamespace => "JrzAsp.Mvc.Captcha.Controllers";

        protected override void OnAfterRegisterArea(AreaRegistrationContext context, string areaName, string baseRoute,
            string areaControllersNamespace, IEnumerable<string> areaControllerNames) {
            base.OnAfterRegisterArea(context, areaName, baseRoute, areaControllersNamespace, areaControllerNames);

            // unpack "Zips\Views.zip" to mvc area
            var areaDir = Path.Combine(MyAppSettings.AreasFolderRelativePath, MyAppSettings.AreaName);
            using (var stream = AreaAssembly.GetManifestResourceStream("JrzAsp.Mvc.Captcha.Zips.Views.zip"))
            using (var archive = new ZipArchive(stream)) {
                foreach (var entry in archive.Entries) {
                    if (string.IsNullOrWhiteSpace(entry.Name)) continue;
                    var targetFile = HostingEnvironment.MapPath(Path.Combine(areaDir, entry.FullName));
                    var targetDir = Path.GetDirectoryName(targetFile);
                    var doExtract = true;
                    if (File.Exists(targetFile)) {
                        var targetFileInfo = new FileInfo(targetFile);
                        doExtract = entry.LastWriteTime.ToUniversalTime().UtcDateTime > targetFileInfo.LastWriteTimeUtc;
                    }
                    if (doExtract) {
                        if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
                        entry.ExtractToFile(targetFile, true);
                    }
                }
            }
        }
    }
}