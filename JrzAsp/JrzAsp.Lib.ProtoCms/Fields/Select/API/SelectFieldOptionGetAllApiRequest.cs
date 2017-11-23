namespace JrzAsp.Lib.ProtoCms.Fields.Select.API {
    public class SelectFieldOptionGetAllApiRequest {
        public string Search { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public string HandlerParam { get; set; }
    }
}