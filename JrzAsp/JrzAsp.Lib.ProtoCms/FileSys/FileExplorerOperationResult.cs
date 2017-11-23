namespace JrzAsp.Lib.ProtoCms.FileSys {
    public class FileExplorerOperationResult {
        public FileExplorerOperationResult(string fullSysPath, string webPath, string name, string downloadPath, bool isDirectory, string[] errors) {
            FullSysPath = fullSysPath;
            WebPath = webPath;
            Name = name;
            Errors = errors ?? new string[0];
            DownloadPath = downloadPath;
            IsDirectory = isDirectory;
        }
        public string FullSysPath { get; }
        public string WebPath { get; }
        public string Name { get; }
        public string DownloadPath { get; }
        public bool IsDirectory { get; }
        public string[] Errors { get; }
    }
}