using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.Core.Permissions;
using JrzAsp.Lib.ProtoCms.FileSys.Permissions;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;
using JrzAsp.Lib.ProtoCms.WebApi.Security;
using Newtonsoft.Json.Linq;

namespace JrzAsp.Lib.ProtoCms.FileSys.API {
    [AuthorizeByProtoPermissions(AccessCmsPermission.PERMISSION_ID)]
    [RoutePrefixProtoCms("file-explorer")]
    public class FileExplorerApiController : BaseProtoApiController {
        public const string PATH_INPUT_HEADER = "X-ProtoCms-FileExplorer-Path";
        public const string PATH_TARGET_INPUT_HEADER = "X-ProtoCms-FileExplorer-PathTarget";
        private readonly IFileExplorerManager _fileMgr;

        public FileExplorerApiController(IFileExplorerManager fileMgr) {
            _fileMgr = fileMgr;
        }

        /// <summary>
        ///     Get path details info for a directory or file, the path input must be provided via HTTP request header
        ///     'X-ProtoCms-FileExplorer-Path'
        /// </summary>
        /// <returns></returns>
        [Route("path-info")]
        [HttpGet]
        [ResponseType(typeof(FileExplorerItem))]
        public IHttpActionResult PathInfo() {
            try {
                if (!Request.Headers.Contains(PATH_INPUT_HEADER)) {
                    throw new HttpException(400,
                        $"ProtoCMS: file explorer path-info requires path param in request header '{PATH_INPUT_HEADER}'.");
                }
                var path = Request.Headers.GetValues(PATH_INPUT_HEADER).FirstOrDefault();
                var handler = _fileMgr.GetHandler();
                var result = handler.PathInfo(path);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }
        
        /// <summary>
        ///     List directory, the directory path input must be provided via HTTP request header
        ///     'X-ProtoCms-FileExplorer-Path'
        /// </summary>
        /// <returns></returns>
        [Route("list")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<FileExplorerItem>))]
        public IHttpActionResult ListDir() {
            try {
                if (!Request.Headers.Contains(PATH_INPUT_HEADER)) {
                    throw new HttpException(400,
                        $"ProtoCMS: file explorer list requires path param in request header '{PATH_INPUT_HEADER}'.");
                }
                var path = Request.Headers.GetValues(PATH_INPUT_HEADER).FirstOrDefault();
                var handler = _fileMgr.GetHandler();
                var items = handler.List(path);
                return JsonProto(items);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        /// <summary>
        ///     Create directory, the directory path input must be provided via HTTP request header
        ///     'X-ProtoCms-FileExplorer-Path'
        /// </summary>
        /// <returns></returns>
        [AuthorizeByProtoPermissions(ManageFileExplorerPermission.PERMISSION_ID)]
        [Route("create-dir")]
        [HttpPost]
        [ResponseType(typeof(FileExplorerOperationResult))]
        public IHttpActionResult CreateDir() {
            try {
                if (!Request.Headers.Contains(PATH_INPUT_HEADER)) {
                    throw new HttpException(400,
                        $"ProtoCMS: file explorer create-dir requires path param in request header '{PATH_INPUT_HEADER}'.");
                }
                var path = Request.Headers.GetValues(PATH_INPUT_HEADER).FirstOrDefault();
                var handler = _fileMgr.GetHandler();
                var result = handler.CreateDirectory(path);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        /// <summary>
        ///     Create/upload file, the target file path input must be provided via HTTP request header
        ///     'X-ProtoCms-FileExplorer-Path'
        /// </summary>
        /// <returns></returns>
        [AuthorizeByProtoPermissions(ManageFileExplorerPermission.PERMISSION_ID)]
        [Route("create-file")]
        [HttpPost]
        [ResponseType(typeof(FileExplorerOperationResult))]
        public IHttpActionResult CreateFile() {
            try {
                if (!Request.Headers.Contains(PATH_INPUT_HEADER)) {
                    throw new HttpException(400,
                        $"ProtoCMS: file explorer create-file requires path param in request header '{PATH_INPUT_HEADER}'.");
                }
                var path = Request.Headers.GetValues(PATH_INPUT_HEADER).FirstOrDefault();
                var handler = _fileMgr.GetHandler();
                var stream = Request.Content.ReadAsStreamAsync().Result;
                var result = handler.CreateFile(path, stream);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        /// <summary>
        ///     Upload file. This API is designed to be used by HTML form, and accept multipart/form-data.
        ///     Field 'file' should contains the file, and field 'targetPath' the target save path of the file into the
        ///     website.
        /// </summary>
        /// <returns></returns>
        [AuthorizeByProtoPermissions(ManageFileExplorerPermission.PERMISSION_ID)]
        [Route("upload-file")]
        [HttpPost]
        [ResponseType(typeof(FileExplorerOperationResult))]
        public IHttpActionResult UploadFile() {
            try {
                if (!Request.Content.IsMimeMultipartContent()) {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                var stream = Request.Content.ReadAsStreamAsync().Result;
                var parser = new HttpMultipartParser(stream, "file");
                if (!parser.Success) throw new HttpException(400, $"ProtoCMS: can't parse upload file request.");
                var targetPath = HttpUtility.UrlDecode(parser.Parameters["targetPath"]);
                var fileStream = new MemoryStream(parser.FileContents);
                var handler = _fileMgr.GetHandler();
                var result = handler.CreateFile(targetPath, fileStream);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        /// <summary>
        ///     Delete files/directories, the paths to delete must be provided using HTTP header
        ///     'X-ProtoCms-FileExplorer-Path' containing a JSON array of strings
        /// </summary>
        /// <returns></returns>
        [AuthorizeByProtoPermissions(ManageFileExplorerPermission.PERMISSION_ID)]
        [Route("delete")]
        [HttpPost]
        [ResponseType(typeof(IEnumerable<FileExplorerOperationResult>))]
        public IHttpActionResult Delete() {
            try {
                if (!Request.Headers.Contains(PATH_INPUT_HEADER)) {
                    throw new HttpException(400,
                        $"ProtoCMS: file explorer delete requires paths param in request header '{PATH_INPUT_HEADER}'.");
                }
                var pathsJsonArrStr = Request.Headers.GetValues(PATH_INPUT_HEADER).FirstOrDefault();
                var paths = JArray.Parse(pathsJsonArrStr).Select(x => x.ToString()).ToArray();
                var handler = _fileMgr.GetHandler();
                var result = handler.Delete(paths.ToArray());
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        /// <summary>
        ///     Copy files/directories, the paths to move must be provided using HTTP header
        ///     'X-ProtoCms-FileExplorer-Path' containing a JSON array of strings. The target directory of copy
        ///     destination must be provided using HTTP header 'X-ProtoCms-FileExplorer-PathTarget' containing
        ///     a string.
        /// </summary>
        /// <returns></returns>
        [AuthorizeByProtoPermissions(ManageFileExplorerPermission.PERMISSION_ID)]
        [Route("copy-to-dir")]
        [HttpPost]
        [ResponseType(typeof(IEnumerable<FileExplorerMoveResult>))]
        public IHttpActionResult CopyToDir() {
            try {
                if (!Request.Headers.Contains(PATH_INPUT_HEADER)) {
                    throw new HttpException(400,
                        $"ProtoCMS: file explorer copy-to-dir requires paths param in request header '{PATH_INPUT_HEADER}'.");
                }
                if (!Request.Headers.Contains(PATH_TARGET_INPUT_HEADER)) {
                    throw new HttpException(400,
                        $"ProtoCMS: file explorer copy-to-dir requires target path param in request header '{PATH_TARGET_INPUT_HEADER}'.");
                }
                var pathsJsonArrStr = Request.Headers.GetValues(PATH_INPUT_HEADER).FirstOrDefault();
                var paths = JArray.Parse(pathsJsonArrStr).Select(x => x.ToString()).ToArray();
                var targetPath = Request.Headers.GetValues(PATH_TARGET_INPUT_HEADER).FirstOrDefault();
                var handler = _fileMgr.GetHandler();
                var result = handler.CopyToDir(paths.ToArray(), targetPath);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        /// <summary>
        ///     Rename a file/a directory, the path to rename must be provided using HTTP header
        ///     'X-ProtoCms-FileExplorer-Path' containing a string. The target path of rename
        ///     destination must be provided using HTTP header 'X-ProtoCms-FileExplorer-PathTarget' containing
        ///     a string.
        /// </summary>
        /// <returns></returns>
        [AuthorizeByProtoPermissions(ManageFileExplorerPermission.PERMISSION_ID)]
        [Route("rename")]
        [HttpPost]
        [ResponseType(typeof(FileExplorerMoveResult))]
        public IHttpActionResult Rename() {
            try {
                if (!Request.Headers.Contains(PATH_INPUT_HEADER)) {
                    throw new HttpException(400,
                        $"ProtoCMS: file explorer rename requires path param in request header '{PATH_INPUT_HEADER}'.");
                }
                if (!Request.Headers.Contains(PATH_TARGET_INPUT_HEADER)) {
                    throw new HttpException(400,
                        $"ProtoCMS: file explorer rename requires target path param in request header '{PATH_TARGET_INPUT_HEADER}'.");
                }
                var path = Request.Headers.GetValues(PATH_INPUT_HEADER).FirstOrDefault();
                var targetPath = Request.Headers.GetValues(PATH_TARGET_INPUT_HEADER).FirstOrDefault();
                var handler = _fileMgr.GetHandler();
                var result = handler.Rename(path, targetPath);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }
    }
}