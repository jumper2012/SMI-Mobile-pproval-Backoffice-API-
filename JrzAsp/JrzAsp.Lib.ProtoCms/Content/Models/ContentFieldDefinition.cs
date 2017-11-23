using System;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentFieldDefinition {
        private readonly IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();

        public ContentFieldDefinition(string fieldName, Type modelType, ContentFieldConfiguration config = null) {
            FieldName = fieldName?.Trim() ?? "";
            ModelType = modelType;
            Config = config;
            Validate();
        }
        
        /// <summary>
        ///     This is the content type object that defines this field. Will be injected by <see cref="ContentType" />.
        /// </summary>
        public ContentType ContentType { get; set; }
        public string FieldName { get; }
        public Type ModelType { get; }
        public ContentFieldConfiguration Config { get; private set; }

        public IContentFieldFinder FieldFinder() {
            var mdl = Activator.CreateInstance(ModelType).DirectCastTo<ContentField>();
            var ff = _dp.GetService(mdl.__FieldSpec.FinderType).DirectCastTo<IContentFieldFinder>();
            return ff;
        }

        public IContentFieldModifier FieldModifier() {
            var mdl = Activator.CreateInstance(ModelType).DirectCastTo<ContentField>();
            var fm = _dp.GetService(mdl.__FieldSpec.ModifierType).DirectCastTo<IContentFieldModifier>();
            return fm;
        }

        private void Validate() {
            if (string.IsNullOrWhiteSpace(FieldName)) {
                throw new InvalidOperationException($"ProtoCMS: content field name is required.");
            }
            if (ModelType == null) {
                throw new InvalidOperationException($"ProtoCMS: content field model type is required.");
            }
            if (!ContentType.VALID_FIELD_NAME_REGEX.IsMatch(FieldName)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field name must match regex '{ContentType.VALID_FIELD_NAME_PATTERN}'.");
            }
            if (!typeof(ContentField).IsAssignableFrom(ModelType)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field model type must inherit from '{typeof(ContentField).FullName}'.");
            }
            if (ModelType.GetConstructor(Type.EmptyTypes) == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field model type must have a default constructor.");
            }

            var mdl = Activator.CreateInstance(ModelType).DirectCastTo<ContentField>();

            if (mdl.__FieldSpec.ConfigType.GetConstructor(Type.EmptyTypes) == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field config type must have a default constructor.");
            }

            if (Config == null) {
                Config = Activator.CreateInstance(mdl.__FieldSpec.ConfigType)
                    .DirectCastTo<ContentFieldConfiguration>();
            }

            if (!mdl.__FieldSpec.ConfigType.IsInstanceOfType(Config)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field config type for model '{ModelType.FullName}' must be an instance of " +
                    $"'{mdl.__FieldSpec.ConfigType.FullName}'.");
            }
        }
    }
}