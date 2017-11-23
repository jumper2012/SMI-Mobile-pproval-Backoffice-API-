using System;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Setting.Services;

namespace JrzAsp.Lib.ProtoCms.Setting.Models {
    public class SiteSettingSpec {
        public SiteSettingSpec(string id, string name, string description, Type settingModelType,
            Type settingHandlerType) {
            Id = id?.Trim();
            Name = name?.Trim();
            Description = description?.Trim();
            SettingModelType = settingModelType;
            SettingHandlerType = settingHandlerType;
            ValidateProps();
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public Type SettingModelType { get; }
        public Type SettingHandlerType { get; }

        private void ValidateProps() {
            if (string.IsNullOrWhiteSpace(Id)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting id is required.");
            }
            if (!ContentType.VALID_ID_REGEX.IsMatch(Id)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting id must match regex '{ContentType.VALID_ID_PATTERN}.");
            }
            if (string.IsNullOrWhiteSpace(Name)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting name is required.");
            }
            if (string.IsNullOrWhiteSpace(Description)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting description is required.");
            }
            if (SettingModelType == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting model type must be defined.");
            }
            if (!typeof(SiteSetting).IsAssignableFrom(SettingModelType)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting model type must be assignable from '{typeof(SiteSetting).FullName}'.");
            }
            if (SettingHandlerType == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting handler type must be defined.");
            }
            if (!typeof(ISiteSettingHandler).IsAssignableFrom(SettingHandlerType)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: site setting handler type must be assignable from " +
                    $"'{typeof(ISiteSettingHandler).FullName}'.");
            }
        }
    }
}