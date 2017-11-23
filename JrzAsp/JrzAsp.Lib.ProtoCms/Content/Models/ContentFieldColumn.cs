using System;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentFieldColumn {
        public ContentFieldColumn(string propName,
            Func<ContentFieldDefinition, string> columnHeader,
            Func<ContentFieldDefinition, bool> sortable,
            Func<ContentField, ContentFieldDefinition, VueComponentDefinition[]> cellValue,
            Func<ContentField, ContentFieldDefinition, string> summarizedValue,
            Func<ContentField, ContentFieldDefinition, VueComponentDefinition[]> fullPreviewValue) {
            PropName = propName?.Trim() ?? "";
            ColumnHeader = columnHeader;
            Sortable = sortable;
            CellValue = cellValue;
            SummarizedValue = summarizedValue;
            FullPreviewValue = fullPreviewValue;
            Validate();
        }

        public string PropName { get; }
        public Func<ContentFieldDefinition, string> ColumnHeader { get; }
        public Func<ContentFieldDefinition, bool> Sortable { get; }
        public Func<ContentField, ContentFieldDefinition, VueComponentDefinition[]> CellValue { get; }
        public Func<ContentField, ContentFieldDefinition, string> SummarizedValue { get; }
        public Func<ContentField, ContentFieldDefinition, VueComponentDefinition[]> FullPreviewValue { get; }

        private void Validate() {
            if (string.IsNullOrWhiteSpace(PropName)) {
                throw new InvalidOperationException($"ProtoCMS: content field column prop name is required.");
            }
            if (ColumnHeader == null) {
                throw new InvalidOperationException($"ProtoCMS: content field column header function is required.");
            }
            if (Sortable == null) {
                throw new InvalidOperationException($"ProtoCMS: content field sortable function is required.");
            }
            if (CellValue == null) {
                throw new InvalidOperationException($"ProtoCMS: content field column cell value function is required.");
            }
            if (SummarizedValue == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field column summarized value function is required.");
            }
            if (!ContentType.VALID_FIELD_NAME_REGEX.IsMatch(PropName)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: content field column prop name must match regex " +
                    $"'{ContentType.VALID_FIELD_NAME_PATTERN}'.");
            }
        }
    }
}