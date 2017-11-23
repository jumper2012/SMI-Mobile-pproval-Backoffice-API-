using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Models {
    /// <summary>
    ///     Use <see cref="DatumType<TDat>"/> to instantiate this class.
    /// </summary>
    public abstract class DatumType {

        public delegate VueComponentDefinition[] GetExtraModifyOperationFormVuesBase(DatumType datumType,
            object datum,
            DatumModifyOperation operation);
        public const string DATUM_MENU_CATEGORY_ID = "protocms-data-menu-category";

        protected static readonly IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();

        private static DatumType[] _definedTypes;

        private static IReadOnlyDictionary<string, DatumType> _definedTypesMap;
        public abstract Type ModelType { get; }
        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ViewDatumPermission ViewPermissionBase { get; }
        public abstract ListDatumPermission ListPermissionBase { get; }
        public abstract ModifyDatumPermission[] ModifyPermissionsBase { get; }
        public abstract string[] FieldNamesIncludedInSummary { get; }
        public abstract string[] ShownFieldNamesInTable { get; }
        public abstract string DefaultSortFieldName { get; }
        public abstract bool DefaultSortDescending { get; }
        public abstract string[] DisabledModifyOperationNames { get; }
        public abstract VueMenuItem[] MenuItemToAccessContentType { get; }
        public abstract GetExtraModifyOperationFormVuesBase[] PreModifyOperationFormVuesBase { get; }
        public abstract GetExtraModifyOperationFormVuesBase[] PostModifyOperationFormVuesBase { get; }

        public static DatumType[] DefinedTypes {
            get {
                if (_definedTypes != null) return _definedTypes;
                var provs = _dp.GetServices(typeof(IDatumTypesProvider));
                var cts = new List<DatumType>();
                var dupsId = new Dictionary<string, int>();
                var dupsType = new Dictionary<Type, int>();
                foreach (var dti in provs.SelectMany(x => x.DirectCastTo<IDatumTypesProvider>().DefineDatumTypeInfos())
                ) {
                    if (!dupsId.TryGetValue(dti.Id, out var countId)) {
                        countId = 0;
                    }
                    countId++;
                    if (countId > 1) {
                        throw new InvalidOperationException(
                            $"ProtoCMS: datum type with id '{dti.Id}' is defined more than once.");
                    }
                    dupsId[dti.Id] = countId;

                    if (!dupsType.TryGetValue(dti.ModelType, out var countType)) {
                        countType = 0;
                    }
                    countType++;
                    if (countType > 1) {
                        throw new InvalidOperationException(
                            $"ProtoCMS: datum type '{dti.ModelType}' is defined more than once.");
                    }
                    dupsType[dti.ModelType] = countType;

                    cts.Add(dti);
                }
                _definedTypes = cts.ToArray();
                return _definedTypes;
            }
        }
        public static IReadOnlyDictionary<string, DatumType> DefinedTypesMap {
            get {
                if (_definedTypesMap != null) return _definedTypesMap;
                var dict = new Dictionary<string, DatumType>();
                foreach (var dt in DefinedTypes) {
                    dict[dt.Id] = dt;
                }
                _definedTypesMap = new ReadOnlyDictionary<string, DatumType>(dict);
                return _definedTypesMap;
            }
        }

        public abstract IDatumFinder FinderBase();
        public abstract IDatumModifier ModifierBase();

        public bool IsModifyOperationAllowed(string modifyOperationName) {
            return !DisabledModifyOperationNames.Any(x =>
                x == modifyOperationName || x == DatumModifyOperation.ANY_MODIFY_OPERATION_NAME);
        }

        public static VueMenuItem CreateDefaultMenuItemToAccessDatum(DatumType dt) {
            var mainUrlsProv = _dp.GetService<IProtoCmsMainUrlsProvider>();
            return new VueMenuItem {
                CategoryId = DATUM_MENU_CATEGORY_ID,
                Href = mainUrlsProv.GenerateManageDatumTypeUrl(dt.Id),
                Label = dt.Name,
                IconCssClass = "fa fa-cubes",
                IsVisible = ctx => ctx.UserHasPermission(dt.ListPermissionBase.Id)
            };
        }
    }
    public class DatumType<TDat> : DatumType {

        public delegate VueComponentDefinition[] GetExtraModifyOperationFormVues(DatumType datumType, TDat datum,
            DatumModifyOperation operation);
        private ListDatumPermission<TDat> _listPermission;
        private ModifyDatumPermission<TDat>[] _modifyPermissions;
        private GetExtraModifyOperationFormVues[] _postModifyOperationFormVues;
        private GetExtraModifyOperationFormVuesBase[] _postModifyOperationFormVuesBase;
        private GetExtraModifyOperationFormVues[] _preModifyOperationFormVues;
        private GetExtraModifyOperationFormVuesBase[] _preModifyOperationFormVuesBase;

        private ViewDatumPermission<TDat> _viewPermission;

        public DatumType(string id, string name, string description,
            string[] fieldNamesIncludedInSummary, string[] shownFieldNamesInTable,
            string defaultSortFieldName, bool defaultSortDescending, string[] disabledModifyOperationNames,
            VueMenuItem[] menuItemToAccessContentType) {
            Id = id;
            Name = name;
            Description = description;
            DefaultSortFieldName = defaultSortFieldName ?? "";
            DefaultSortDescending = defaultSortDescending;
            FieldNamesIncludedInSummary = fieldNamesIncludedInSummary ?? new string[0];
            ShownFieldNamesInTable = shownFieldNamesInTable ?? new string[0];
            DisabledModifyOperationNames = disabledModifyOperationNames ?? new string[0];
            MenuItemToAccessContentType = menuItemToAccessContentType ?? new[] {
                CreateDefaultMenuItemToAccessDatum(this)
            };
            ValidateProps();
        }

        public override Type ModelType => typeof(TDat);
        public override string Id { get; }
        public override string Name { get; }
        public override string Description { get; }
        public override ViewDatumPermission ViewPermissionBase => ViewPermission;
        public ViewDatumPermission<TDat> ViewPermission {
            get {
                if (_viewPermission != null) return _viewPermission;
                _viewPermission = Finder().PermissionToView;
                return _viewPermission;
            }
        }
        public ListDatumPermission<TDat> ListPermission {
            get {
                if (_listPermission != null) return _listPermission;
                _listPermission = Finder().PermissionToList;
                return _listPermission;
            }
        }
        public override ListDatumPermission ListPermissionBase => ListPermission;
        public ModifyDatumPermission<TDat>[] ModifyPermissions {
            get {
                if (_modifyPermissions != null) return _modifyPermissions;
                _modifyPermissions = Modifier().PermissionsToModify;
                return _modifyPermissions;
            }
        }
        public override ModifyDatumPermission[] ModifyPermissionsBase => ModifyPermissions;
        public override string[] FieldNamesIncludedInSummary { get; }
        public override string[] ShownFieldNamesInTable { get; }
        public override string DefaultSortFieldName { get; }
        public override bool DefaultSortDescending { get; }
        public override string[] DisabledModifyOperationNames { get; }
        public override VueMenuItem[] MenuItemToAccessContentType { get; }
        public override GetExtraModifyOperationFormVuesBase[] PreModifyOperationFormVuesBase {
            get {
                if (_preModifyOperationFormVuesBase != null) return _preModifyOperationFormVuesBase;
                var funcs = new List<GetExtraModifyOperationFormVuesBase>();
                foreach (var implFunc in PreModifyOperationFormVues) {
                    funcs.Add((datumType, datumObj, operation) =>
                        implFunc(datumType, datumObj.DirectCastTo<TDat>(), operation));
                }
                _preModifyOperationFormVuesBase = funcs.ToArray();
                return _preModifyOperationFormVuesBase;
            }
        }
        public override GetExtraModifyOperationFormVuesBase[] PostModifyOperationFormVuesBase {
            get {
                if (_postModifyOperationFormVuesBase != null) return _postModifyOperationFormVuesBase;
                var funcs = new List<GetExtraModifyOperationFormVuesBase>();
                foreach (var implFunc in PostModifyOperationFormVues) {
                    funcs.Add((datumType, datumObj, operation) =>
                        implFunc(datumType, datumObj.DirectCastTo<TDat>(), operation));
                }
                _postModifyOperationFormVuesBase = funcs.ToArray();
                return _postModifyOperationFormVuesBase;
            }
        }
        public GetExtraModifyOperationFormVues[] PreModifyOperationFormVues {
            get {
                if (_preModifyOperationFormVues != null) return _preModifyOperationFormVues;
                _preModifyOperationFormVues = new GetExtraModifyOperationFormVues[0];
                return _preModifyOperationFormVues;
            }
            set => _preModifyOperationFormVues = value;
        }
        public GetExtraModifyOperationFormVues[] PostModifyOperationFormVues {
            get {
                if (_postModifyOperationFormVues != null) return _postModifyOperationFormVues;
                _postModifyOperationFormVues = new GetExtraModifyOperationFormVues[0];
                return _postModifyOperationFormVues;
            }
            set => _postModifyOperationFormVues = value;
        }

        public override IDatumFinder FinderBase() {
            return Finder();
        }

        public override IDatumModifier ModifierBase() {
            return Modifier();
        }

        public IDatumFinder<TDat> Finder() {
            var finder = _dp.GetService(typeof(IDatumFinder<TDat>)).DirectCastTo<IDatumFinder<TDat>>();
            return finder;
        }

        public IDatumModifier<TDat> Modifier() {
            var modifier = _dp.GetService(typeof(IDatumModifier<TDat>)).DirectCastTo<IDatumModifier<TDat>>();
            return modifier;
        }

        private void ValidateProps() {
            if (ModelType == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum model type must be provided.");
            }
            if (string.IsNullOrWhiteSpace(Id)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum type id must be provided.");
            }
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum type name must be provided.");
            }
            if (!ContentType.VALID_ID_REGEX.IsMatch(Id)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: datum type id must match regex '{ContentType.VALID_ID_PATTERN}'.");
            }
        }
    }

    public static class DatumTypeExtensions {
        public static DatumType GetDatumTypeFromType(this Type type) {
            return DatumType.DefinedTypes.FirstOrDefault(x => x.ModelType == type);
        }

        public static DatumType<TDat> GetDatumTypeFromType<TDat>(this Type type) {
            return GetDatumTypeFromType(type).DirectCastTo<DatumType<TDat>>();
        }
    }
}