using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Common {
    public class CommonFieldConfiguration : ContentFieldConfiguration {
        private string _dateTimeFormat;

        public CommonFieldConfiguration() {
            HandledModifyOperationNames = new[] {
                ContentModifyOperation.ANY_MODIFY_OPERATION_NAME
            };
        }

        public string DateTimeFormat {
            get {
                if (_dateTimeFormat != null) return _dateTimeFormat;
                _dateTimeFormat = ProtoContent.DATE_FORMAT_FOR_DISPLAY;
                return _dateTimeFormat;
            }
            set => _dateTimeFormat = value;
        }
    }
}