namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberFieldTableFilterFormItem {
        public string FieldName { get; set; }
        public bool IsNullValue { get; set; }
        public decimal? MinNumber { get; set; }
        public bool GreaterThanOrEqualToMin { get; set; }
        public decimal? MaxNumber { get; set; }
        public bool LessThanOrEqualToMax { get; set; }
        public bool IsEnabled { get; set; }
        public dynamic VueProps { get; set; }
    }
}