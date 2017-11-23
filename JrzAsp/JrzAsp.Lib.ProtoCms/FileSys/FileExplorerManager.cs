using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ProtoCms.FileSys {
    public class FileExplorerManager : IFileExplorerManager {
        public FileExplorerManager(IEnumerable<IFileExplorerHandler> handlers) {
            Handlers = handlers.OrderBy(x => x.Priority).ToArray();
        }

        public IFileExplorerHandler[] Handlers { get; }

        public IFileExplorerHandler GetHandler() {
            return Handlers.FirstOrDefault();
        }
    }
}