using System;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoFieldTableFilterFormItem {
        public string FieldName { get; set; }
        public bool IsNullValue { get; set; }
        public DateTime? MinDateTime { get; set; }
        public bool GreaterThanOrEqualToMin { get; set; }
        public DateTime? MaxDateTime { get; set; }
        public bool LessThanOrEqualToMax { get; set; }
        public bool IsEnabled { get; set; }
        public dynamic VueProps { get; set; }
    }
}