namespace JrzAsp.Lib.ProtoCms {
    /// <summary>
    ///     All interfaces that inherit from this will be considered by <see cref="AutoDependencyBinder" /> for
    ///     automatic dependency registration. The registration will use <see cref="DependencyScope.PerRequest" />.
    /// </summary>
    public interface IPerRequestDependency { }
}