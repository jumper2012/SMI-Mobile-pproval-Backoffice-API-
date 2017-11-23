using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using JrzAsp.Lib.ProtoCms.Content.Permissions;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Fields.Common;
using JrzAsp.Lib.ProtoCms.Fields.Publishing;
using JrzAsp.Lib.ProtoCms.Fields.Trashing;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentType {
        public const string CONTENT_MENU_CATEGORY_ID = "protocms-contents-menu-category";
        public const string ANY_CONTENT_TYPE_ID = "*";
        public const string FIELD_NAME_PUBLISH_STATUS = "PublishStatus";
        public const string FIELD_NAME_TRASH_STATUS = "TrashStatus";
        public const string FIELD_NAME_COMMON_META = "CommonMeta";
        public const string VALID_FIELD_NAME_PATTERN = @"^[A-Z_][_A-Za-z0-9]*$";
        public const string VALID_ID_PATTERN = @"^[a-z][a-z0-9\-]*$";

        public static readonly Regex VALID_FIELD_NAME_REGEX =
            new Regex(VALID_FIELD_NAME_PATTERN, RegexOptions.Compiled);

        public static readonly Regex VALID_ID_REGEX =
            new Regex(VALID_ID_PATTERN, RegexOptions.Compiled);

        private static readonly IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();

        private static ContentType[] _definedTypes;

        private static IReadOnlyDictionary<string, ContentType> _definedTypesMap;

        private readonly ContentFieldDefinition[] _customFields;

        private readonly IDictionary<string, ContentFieldDefinition> _fieldMap =
            new Dictionary<string, ContentFieldDefinition>();

        private ContentFieldDefinition[] _fields;
        private GetExtraModifyOperationFormVues[] _postModifyOperationFormVues;
        private GetExtraModifyOperationFormVues[] _preModifyOperationFormVues;

        public ContentType(string id, string name, string description,
            ContentFieldDefinition[] customFields,
            string[] fieldNamesIncludedInSummary,
            string[] shownFieldNamesInTable,
            string defaultSortFieldName,
            bool defaultSortDescending,
            string[] disabledModifyOperationNames,
            VueMenuItem[] menuItemToAccessContentType) {
            Id = id?.Trim() ?? "";
            Name = name?.Trim() ?? "";
            ValidateProps();
            Description = description?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new ArgumentException($"ProtoCMS: content type name must be provided.", nameof(name));
            }
            DefaultSortFieldName = defaultSortFieldName?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(DefaultSortFieldName)) {
                DefaultSortFieldName = $"{FIELD_NAME_COMMON_META}.{nameof(CommonField.CreatedUtc)}";
            }
            DefaultSortDescending = defaultSortDescending;
            MenuItemToAccessContentType = menuItemToAccessContentType ?? new[] {
                CreateDefaultMenuItemToAccessContent(this)
            };
            DisabledModifyOperationNames = disabledModifyOperationNames ?? new string[0];
            _customFields = customFields ?? new ContentFieldDefinition[0];
            FieldNamesIncludedInSummary = fieldNamesIncludedInSummary;
            if (FieldNamesIncludedInSummary == null || FieldNamesIncludedInSummary.Length == 0) {
                FieldNamesIncludedInSummary = new[] {
                    $"{FIELD_NAME_COMMON_META}.{nameof(CommonField.ContentTypeId)}",
                    $"{FIELD_NAME_PUBLISH_STATUS}.{nameof(PublishingField.IsDraft)}",
                    $"{FIELD_NAME_TRASH_STATUS}.{nameof(TrashingField.IsTrashed)}",
                    $"{FIELD_NAME_COMMON_META}.{nameof(CommonField.Id)}"
                };
            }
            ShownFieldNamesInTable = shownFieldNamesInTable;
            if (ShownFieldNamesInTable == null || ShownFieldNamesInTable.Length == 0) {
                ShownFieldNamesInTable = new[] {
                    $"{FIELD_NAME_COMMON_META}.{nameof(CommonField.ContentTypeId)}",
                    $"{FIELD_NAME_PUBLISH_STATUS}.{nameof(PublishingField.IsDraft)}",
                    $"{FIELD_NAME_TRASH_STATUS}.{nameof(TrashingField.IsTrashed)}",
                    $"{FIELD_NAME_COMMON_META}.{nameof(CommonField.CreatedUtc)}",
                    $"{FIELD_NAME_COMMON_META}.{nameof(CommonField.UpdatedUtc)}",
                    $"{FIELD_NAME_COMMON_META}.{nameof(CommonField.Id)}"
                };
            }
        }

        public static ContentType[] DefinedTypes {
            get {
                if (_definedTypes != null) return _definedTypes;
                var provs = _dp.GetServices(typeof(IContentTypesProvider));
                var cts = new List<ContentType>();
                var dups = new Dictionary<string, int>();
                foreach (var ct in provs.SelectMany(x => x.DirectCastTo<IContentTypesProvider>().DefineContentTypes())
                ) {
                    if (!dups.TryGetValue(ct.Id, out var count)) {
                        count = 0;
                    }
                    count++;
                    if (count > 1) {
                        throw new InvalidOperationException(
                            $"ProtoCMS: content type with id '{ct.Id}' is defined more than once.");
                    }
                    dups[ct.Id] = count;
                    cts.Add(ct);
                }
                _definedTypes = cts.ToArray();
                return _definedTypes;
            }
        }
        public static IReadOnlyDictionary<string, ContentType> DefinedTypesMap {
            get {
                if (_definedTypesMap != null) return _definedTypesMap;
                var dict = new Dictionary<string, ContentType>();
                foreach (var ct in DefinedTypes) {
                    dict[ct.Id] = ct;
                }
                _definedTypesMap = new ReadOnlyDictionary<string, ContentType>(dict);
                return _definedTypesMap;
            }
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public ContentFieldDefinition[] Fields {
            get {
                if (_fields != null) return _fields;
                var fs = new List<ContentFieldDefinition>(_customFields) {
                    new ContentFieldDefinition(FIELD_NAME_PUBLISH_STATUS, typeof(PublishingField),
                        CustomPublishingFieldConfiguration),
                    new ContentFieldDefinition(FIELD_NAME_TRASH_STATUS, typeof(TrashingField),
                        CustomTrashingFieldConfiguration),
                    new ContentFieldDefinition(FIELD_NAME_COMMON_META, typeof(CommonField))
                };
                var dupMap = new Dictionary<string, int>();
                var instCountMap = new Dictionary<string, int>();
                foreach (var cf in fs) {
                    cf.ContentType = this;
                    if (!dupMap.TryGetValue(cf.FieldName, out var count)) count = 0;
                    count++;
                    if (count > 1) {
                        var extraFieldNameErrorMessage = new[] {
                            FIELD_NAME_COMMON_META, FIELD_NAME_PUBLISH_STATUS, FIELD_NAME_TRASH_STATUS
                        }.Any(x => string.Equals(cf.FieldName, x, StringComparison.InvariantCultureIgnoreCase))
                            ? $"{cf.FieldName} is a reserved field name that can only be used by the system."
                            : "";
                        throw new InvalidOperationException(
                            $"ProtoCMS: content type may not have multiple fields with the same name ({cf.FieldName})." +
                            $"{extraFieldNameErrorMessage}");
                    }
                    dupMap[cf.FieldName] = count;

                    if (!instCountMap.TryGetValue(cf.FieldName, out var instCount)) instCount = 0;
                    instCount++;
                    var fmdl = Activator.CreateInstance(cf.ModelType).DirectCastTo<ContentField>();
                    if (instCount > fmdl.__FieldSpec.MaxInstancePerContentType) {
                        throw new InvalidOperationException(
                            $"ProtoCMS: content type may not define more than " +
                            $"{fmdl.__FieldSpec.MaxInstancePerContentType} fields of type " +
                            $"'{cf.ModelType.FullName}'.");
                    }
                    instCountMap[cf.FieldName] = instCount;
                }
                _fields = fs.ToArray();
                return _fields;
            }
        }
        public string[] FieldNamesIncludedInSummary { get; }
        public string[] ShownFieldNamesInTable { get; }
        public string DefaultSortFieldName { get; }
        public bool DefaultSortDescending { get; }
        public string[] DisabledModifyOperationNames { get; }
        public VueMenuItem[] MenuItemToAccessContentType { get; }
        public PublishingFieldConfiguration CustomPublishingFieldConfiguration { get; set; }
        public TrashingFieldConfiguration CustomTrashingFieldConfiguration { get; set; }
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

        public static VueMenuItem CreateDefaultMenuItemToAccessContent(ContentType ct) {
            var mainUrlsProv = _dp.GetService<IProtoCmsMainUrlsProvider>();
            return new VueMenuItem {
                CategoryId = CONTENT_MENU_CATEGORY_ID,
                Href = mainUrlsProv.GenerateManageContentTypeUrl(ct.Id),
                Label = ct.Name,
                IconCssClass = "fa fa-file-o",
                IsVisible = ctx => ctx.UserHasPermission(ListContentPermission.GetIdFor(ct.Id))
            };
        }

        public ContentFieldDefinition Field(string fieldName) {
            if (_fieldMap.TryGetValue(fieldName, out var fd)) return fd;
            fd = Fields.First(x => fieldName == x.FieldName || fieldName.StartsWith($"{x.FieldName}."));
            _fieldMap[fieldName] = fd;
            return fd;
        }

        public IContentFinder Finder() {
            var finder = _dp.GetService(typeof(IContentFinder)).DirectCastTo<IContentFinder>();
            finder.Initialize(this);
            return finder;
        }

        public IContentModifier Modifier() {
            var modifier = _dp.GetService(typeof(IContentModifier)).DirectCastTo<IContentModifier>();
            modifier.Initialize(this);
            return modifier;
        }

        public bool IsModifyOperationAllowed(string modifyOperationName) {
            return !DisabledModifyOperationNames.Any(x =>
                x == modifyOperationName || x == ContentModifyOperation.ANY_MODIFY_OPERATION_NAME);
        }

        private void ValidateProps() {
            if (string.IsNullOrWhiteSpace(Id)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content type id must be provided.");
            }
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content type name must be provided.");
            }
            if (!VALID_ID_REGEX.IsMatch(Id)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content type id must match regex '{VALID_ID_PATTERN}'.");
            }
        }
    }

    public delegate VueComponentDefinition[] GetExtraModifyOperationFormVues(ContentType contentType,
        ProtoContent content,
        ContentModifyOperation operation);
}