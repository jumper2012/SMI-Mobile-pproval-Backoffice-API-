namespace JrzAsp.Lib.ProtoCms.FileSys {
    public class FileExplorerMoveResult {
        public FileExplorerMoveResult(FileExplorerOperationResult fromBefore, FileExplorerOperationResult toAfter) {
            FromBefore = fromBefore;
            ToAfter = toAfter;
        }

        public FileExplorerOperationResult FromBefore { get; }
        public FileExplorerOperationResult ToAfter { get; }
    }
}