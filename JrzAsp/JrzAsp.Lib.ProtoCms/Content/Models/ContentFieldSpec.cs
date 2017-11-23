using System;
using JrzAsp.Lib.ProtoCms.Content.Services;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentFieldSpec {
        public ContentFieldSpec(Type finderType, Type modifierType, Type configType,
            int maxInstancePerContentType = int.MaxValue) {
            FinderType = finderType;
            ModifierType = modifierType;
            ConfigType = configType;
            MaxInstancePerContentType = maxInstancePerContentType;
            Validate();
        }

        public Type FinderType { get; }
        public Type ModifierType { get; }
        public Type ConfigType { get; }
        public int MaxInstancePerContentType { get; }

        private void Validate() {
            if (FinderType == null) {
                throw new InvalidOperationException($"ProtoCMS: content field must define a valid finder class type.");
            }
            if (ModifierType == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field must define a valid modifier class type.");
            }
            if (ConfigType == null) {
                throw new InvalidOperationException($"ProtoCMS: content field must define a valid config class type.");
            }
            if (!typeof(IContentFieldFinder).IsAssignableFrom(FinderType)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field finder type must inherit from '{typeof(IContentFieldFinder).FullName}'.");
            }
            if (!typeof(IContentFieldModifier).IsAssignableFrom(ModifierType)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field modifier type must inherit from '{typeof(IContentFieldModifier).FullName}'.");
            }
            if (!typeof(ContentFieldConfiguration).IsAssignableFrom(ConfigType)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field config type must inherit from '{typeof(ContentFieldConfiguration).FullName}'.");
            }
            if (MaxInstancePerContentType < 1) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field max instance per content type must be a non-zero positive number.");
            }
        }
    }
}