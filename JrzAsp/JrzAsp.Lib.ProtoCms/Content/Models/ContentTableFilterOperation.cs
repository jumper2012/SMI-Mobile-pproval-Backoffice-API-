namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentTableFilterOperation {
        public ContentTableFilterOperation(string whereConditionName, object whereMethodParam) {
            WhereConditionName = whereConditionName;
            WhereMethodParam = whereMethodParam;
        }

        public string WhereConditionName { get; }
        public object WhereMethodParam { get; }
    }
}