using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.FilePicker {
    public class FilePickerFieldConfiguration : ContentFieldConfiguration {
        public FilePickerFieldConfiguration() {
            IncludeWhenSearching = false;
        }

        public bool IsMultiSelect { get; set; }
    }
}