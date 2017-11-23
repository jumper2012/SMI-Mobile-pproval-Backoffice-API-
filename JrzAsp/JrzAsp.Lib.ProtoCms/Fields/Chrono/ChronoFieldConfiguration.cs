using System;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoFieldConfiguration : ContentFieldConfiguration {
        private string _dateTimeFormat;

        public ChronoFieldConfiguration() {
            PickerKind = ChronoFieldPickerKind.DateAndTime;
        }

        public DateTimeKind Kind { get; set; } = DateTimeKind.Local;
        public DateTime? InitialValue { get; set; }
        public DateTime? DefaultValue { get; set; }
        public ChronoFieldPickerKind PickerKind { get; set; }
        public string DateTimeFormat {
            get {
                if (_dateTimeFormat != null) return _dateTimeFormat;
                _dateTimeFormat = ProtoContent.DATE_FORMAT_FOR_DISPLAY;
                return _dateTimeFormat;
            }
            set => _dateTimeFormat = value;
        }
    }

    public enum ChronoFieldPickerKind {
        DateAndTime,
        DateOnly,
        TimeOnly
    }
}