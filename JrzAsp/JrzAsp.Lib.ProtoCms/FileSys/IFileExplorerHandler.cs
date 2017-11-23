using System.IO;

namespace JrzAsp.Lib.ProtoCms.FileSys {
    public interface IFileExplorerHandler : IPerRequestDependency {
        decimal Priority { get; }

        FileExplorerItem[] List(string path);

        FileExplorerOperationResult CreateDirectory(string path);

        FileExplorerOperationResult CreateFile(string path, Stream fileStream);

        FileExplorerOperationResult[] Delete(string[] paths);

        FileExplorerMoveResult[] CopyToDir(string[] paths, string targetDir);

        FileExplorerMoveResult Rename(string path, string newPath);

        FileExplorerItem PathInfo(string path);

        string GetRealPath(string path);
        string GetWebPath(string realPath);
        string GetDownloadPath(string realPath);
        string GetItemName(string webPath);
        long GetFileSize(string fileRealPath);
    }
}