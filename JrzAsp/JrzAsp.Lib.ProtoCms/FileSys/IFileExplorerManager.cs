namespace JrzAsp.Lib.ProtoCms.FileSys {
    public interface IFileExplorerManager : IPerRequestDependency {
        IFileExplorerHandler[] Handlers { get; }
        IFileExplorerHandler GetHandler();
    }
}