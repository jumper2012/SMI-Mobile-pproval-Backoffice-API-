namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentTableHeader {
        public ContentTableHeader(string name, string label, bool sortable) {
            Name = name;
            Label = label;
            Sortable = sortable;
        }

        public string Name { get; }
        public string Label { get; }
        public bool Sortable { get; }
    }
}