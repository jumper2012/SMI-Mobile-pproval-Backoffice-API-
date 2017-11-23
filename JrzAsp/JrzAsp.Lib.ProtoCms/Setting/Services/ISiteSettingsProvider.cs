using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Setting.Models;

namespace JrzAsp.Lib.ProtoCms.Setting.Services {
    public interface ISiteSettingsProvider : IGlobalSingletonDependency {
        IEnumerable<SiteSettingSpec> DefineSiteSettingSpecs();
    }
}