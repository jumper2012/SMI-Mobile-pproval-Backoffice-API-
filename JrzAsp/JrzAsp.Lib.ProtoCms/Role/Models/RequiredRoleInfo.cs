namespace JrzAsp.Lib.ProtoCms.Role.Models {
    public sealed class RequiredRoleInfo {

        public static readonly RequiredRoleInfo SuperAdmin = new RequiredRoleInfo {
            Name = "SuperAdmin",
            Description = "SuperAdmins automatically have all permissions."
        };

        public static readonly RequiredRoleInfo Authenticated = new RequiredRoleInfo {
            Name = "Authenticated",
            Description = "Represents users that have logged in."
        };

        public static readonly RequiredRoleInfo Guest = new RequiredRoleInfo {
            Name = "Guest",
            Description = "Represents users that have not logged in."
        };

        internal RequiredRoleInfo() { }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
    }
}