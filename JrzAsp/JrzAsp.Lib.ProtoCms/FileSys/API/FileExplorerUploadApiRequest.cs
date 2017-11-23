using System.ComponentModel.DataAnnotations;
using System.Web;

namespace JrzAsp.Lib.ProtoCms.FileSys.API {
    public class FileExplorerUploadApiRequest {
        [Required]
        public HttpPostedFileBase File { get; set; }

        [Required]
        public string TargetPath { get; set; }
    }
}