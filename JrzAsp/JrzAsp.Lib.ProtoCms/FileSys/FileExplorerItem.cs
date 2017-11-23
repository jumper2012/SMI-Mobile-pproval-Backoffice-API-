namespace JrzAsp.Lib.ProtoCms.FileSys {
    public class FileExplorerItem {
        public FileExplorerItem(string name, string webPath, string downloadPath, bool isDirectory, long sizeInBytes) {
            Name = name;
            WebPath = webPath;
            DownloadPath = downloadPath;
            IsDirectory = isDirectory;
            SizeInBytes = sizeInBytes;
        }
        
        public string Name { get; }
        public string WebPath { get; }
        public string DownloadPath { get; }
        public bool IsDirectory { get; }

        /// <summary>
        ///     Only important if <see cref="IsDirectory" /> is false
        /// </summary>
        public long SizeInBytes { get; }
    }
}