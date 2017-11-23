using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.Setting.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Setting.Services {
    public class SiteSettingManager : ISiteSettingManager {
        private static readonly IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();
        private static ISiteSettingsProvider[] _siteSettingsProviders;
        private static SiteSettingSpec[] _definedSettingSpecs;
        private static IReadOnlyDictionary<string, SiteSettingSpec> _definedSettingSpecsMap;

        private readonly IDictionary<string, SiteSetting> _settingData = new ConcurrentDictionary<string, SiteSetting>()
            ;

        private readonly IDictionary<string, SiteSetting> _settingsData =
            new ConcurrentDictionary<string, SiteSetting>();

        private IReadOnlyDictionary<string, ISiteSettingHandler> _settingHandlers;

        private IProtoCmsDbContext Db => _dp.GetService(typeof(IProtoCmsDbContext)).DirectCastTo<IProtoCmsDbContext>();

        public static ISiteSettingsProvider[] SiteSettingsProviders {
            get {
                if (_siteSettingsProviders != null) return _siteSettingsProviders;

                var sspTypes = TypesCache.AppDomainTypes.Where(x => typeof(ISiteSettingsProvider).IsAssignableFrom(x) &&
                                                                    x.IsClass && !x.IsAbstract && !x.IsInterface &&
                                                                    !x.IsGenericTypeDefinition);
                var ssps = new List<ISiteSettingsProvider>();
                foreach (var sspt in sspTypes) {
                    var ssp = Activator.CreateInstance(sspt).DirectCastTo<ISiteSettingsProvider>();
                    ssps.Add(ssp);
                }
                _siteSettingsProviders = ssps.ToArray();
                return _siteSettingsProviders;
            }
        }

        public static ISiteSettingManager Current =>
            _dp.GetService(typeof(ISiteSettingManager)).DirectCastTo<ISiteSettingManager>();

        public static SiteSettingSpec[] DefinedSettingSpecs {
            get {
                if (_definedSettingSpecs != null) return _definedSettingSpecs;
                var dups = new ConcurrentDictionary<string, int>();
                var specs = new List<SiteSettingSpec>();
                foreach (var spec in SiteSettingsProviders.SelectMany(x => x.DefineSiteSettingSpecs())
                    .OrderBy(x => x.Name)) {
                    if (!dups.TryGetValue(spec.Id, out var count)) {
                        count = 0;
                    }
                    count++;
                    if (count > 1) {
                        throw new InvalidOperationException(
                            $"ProtoCMS: site setting with id '{spec.Id}' is defined more than once.");
                    }
                    dups[spec.Id] = count;
                    specs.Add(spec);
                }
                _definedSettingSpecs = specs.ToArray();
                return _definedSettingSpecs;
            }
        }
        public static IReadOnlyDictionary<string, SiteSettingSpec> DefinedSettingSpecsMap {
            get {
                if (_definedSettingSpecsMap != null) return _definedSettingSpecsMap;
                var dict = new ConcurrentDictionary<string, SiteSettingSpec>();
                foreach (var sp in DefinedSettingSpecs) {
                    dict[sp.Id] = sp;
                }
                _definedSettingSpecsMap = new ReadOnlyDictionary<string, SiteSettingSpec>(dict);
                return _definedSettingSpecsMap;
            }
        }
        public SiteSettingSpec[] SettingSpecs => DefinedSettingSpecs;
        public IReadOnlyDictionary<string, ISiteSettingHandler> SettingHandlers {
            get {
                if (_settingHandlers != null) return _settingHandlers;
                var dict = new ConcurrentDictionary<string, ISiteSettingHandler>();
                foreach (var spec in SettingSpecs) {
                    dict[spec.Id] = Activator.CreateInstance(spec.SettingHandlerType)
                        .DirectCastTo<ISiteSettingHandler>();
                }
                _settingHandlers = new ReadOnlyDictionary<string, ISiteSettingHandler>(dict);
                return _settingHandlers;
            }
        }

        public SiteSetting GetSetting(string settingId, bool forceRefresh = false) {
            if (forceRefresh) {
                return GetSettingFresh(settingId);
            }
            return _settingData.TryGetValue(settingId, out var sobj) ? sobj : GetSettingFresh(settingId);
        }

        public TStg GetSetting<TStg>(bool forceRefresh = false) where TStg : SiteSetting {
            var spec = DefinedSettingSpecs.First(x => x.SettingModelType == typeof(TStg));
            return GetSetting(spec.Id, forceRefresh).DirectCastTo<TStg>();
        }

        public IReadOnlyDictionary<string, SiteSetting> GetSettings(bool forceRefresh = false) {
            var dict = new ConcurrentDictionary<string, SiteSetting>();
            foreach (var ssp in SettingSpecs) {
                dict[ssp.Id] = GetSetting(ssp.Id, forceRefresh);
            }
            return new ReadOnlyDictionary<string, SiteSetting>(dict);
        }

        public SiteSettingModifierForm BuildSettingModifierForm(string settingId) {
            var handler = SettingHandlers[settingId];
            var oldSetting = GetSetting(settingId, true);
            var form = handler.BuildSettingModifierForm(oldSetting);
            return form;
        }

        public FurtherValidationResult ValidateSettingModifierForm(string settingId, SiteSettingModifierForm form) {
            var handler = SettingHandlers[settingId];
            var oldSetting = GetSetting(settingId, true);
            var result = handler.ValidateSettingModifierForm(form, oldSetting);
            return result ?? FurtherValidationResult.Ok;
        }

        public VueComponentDefinition[] CreateSettingModifierFormVues(string settingId, SiteSettingModifierForm form) {
            var handler = SettingHandlers[settingId];
            var oldSetting = GetSetting(settingId, true);
            var vues = handler.CreateSettingModifierFormVues(form, oldSetting);
            return vues;
        }

        public void UpdateSetting(string settingId, SiteSettingModifierForm form) {
            var db = Db.ThisDbContext();
            var handler = SettingHandlers[settingId];
            using (var dbTrx = db.Database.BeginTransaction()) {
                try {
                    var oldSetting = GetSetting(settingId, true);
                    foreach (var sf in Db.ProtoSettingFields.Where(x => x.SettingId == settingId).ToList()) {
                        Db.ProtoSettingFields.Remove(sf);
                    }
                    db.SaveChanges();
                    var newSettingFields = handler.UpdateAndRebuildNewSettingFields(form, oldSetting);
                    foreach (var nsf in newSettingFields) {
                        nsf.SettingId = settingId;
                        Db.ProtoSettingFields.Add(nsf);
                    }
                    db.SaveChanges();
                    dbTrx.Commit();
                } catch (Exception) {
                    dbTrx.Rollback();
                    throw;
                }
            }
            GetSetting(settingId, true); // refresh
        }

        private SiteSetting GetSettingFresh(string settingId) {
            if (!SettingHandlers.ContainsKey(settingId)) {
                throw new HttpException(400, $"ProtoCMS: there's no setting with id '{settingId}' found.");
            }
            var handler = SettingHandlers[settingId];
            var settingFields = Db.ProtoSettingFields.ToArray();
            var settingObject = handler.GetSettingObject(settingFields.Where(x => x.SettingId == settingId).ToArray());
            if (settingObject == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting handler must always return a non-null setting object. " +
                    $"Problem handler: {handler.GetType().FullName}.");
            }
            if (!DefinedSettingSpecsMap[settingId].SettingModelType.IsInstanceOfType(settingObject)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting handler for id '{settingId}' must return a setting object of type " +
                    $"'{DefinedSettingSpecsMap[settingId].SettingModelType.FullName}'. " +
                    $"Problem handler: {handler.GetType().FullName}.");
            }
            _settingData[settingId] = settingObject;
            return settingObject;
        }
    }
}