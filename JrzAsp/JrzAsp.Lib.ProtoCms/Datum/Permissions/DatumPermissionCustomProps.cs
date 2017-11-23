namespace JrzAsp.Lib.ProtoCms.Datum.Permissions {
    public class DatumPermissionCustomProps {
        public DatumPermissionCustomProps(string displayName, string description, string categoryName,
            string subCategoryName) {
            DisplayName = displayName;
            Description = description;
            CategoryName = categoryName;
            SubCategoryName = subCategoryName;
        }

        public string DisplayName { get; }
        public string Description { get; }
        public string CategoryName { get; }
        public string SubCategoryName { get; }
    }
}