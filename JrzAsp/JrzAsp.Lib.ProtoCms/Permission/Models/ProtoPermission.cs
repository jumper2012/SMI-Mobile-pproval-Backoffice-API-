namespace JrzAsp.Lib.ProtoCms.Permission.Models {
    /// <summary>
    ///     Defines ProtoCMS permissions
    /// </summary>
    public abstract class ProtoPermission {
        /// <summary>
        ///     Unique id of the permission
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        ///     Permission name to display
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        ///     Description of this permission
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        ///     Permission category name
        /// </summary>
        public abstract string CategoryName { get; }

        /// <summary>
        ///     Permission sub-category name
        /// </summary>
        public abstract string SubCategoryName { get; }

        /// <summary>
        ///     If true, then all authenticated users will have this permission
        /// </summary>
        public abstract bool AuthenticatedUserHasThisByDefault { get; }

        /// <summary>
        ///     If true, then all unauthenticated users will have this permission
        /// </summary>
        public abstract bool GuestUserHasThisByDefault { get; }
    }
}