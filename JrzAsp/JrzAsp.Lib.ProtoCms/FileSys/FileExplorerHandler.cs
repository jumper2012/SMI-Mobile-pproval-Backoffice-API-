using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace JrzAsp.Lib.ProtoCms.FileSys {
    public class FileExplorerHandler : IFileExplorerHandler {
        public string BasePath => HostingEnvironment.MapPath(ProtoCmsAppSettings.FileExplorerBasePath);
        public decimal Priority => 0;

        public FileExplorerItem[] List(string path) {
            var realPath = GetRealPath(path);
            if (!Directory.Exists(realPath)) {
                throw new HttpException(404, $"ProtoCMS: directory doesn't exists '{path}'.");
            }
            var feItems = new List<FileExplorerItem>();
            var dirs = Directory.EnumerateDirectories(realPath).ToArray();
            foreach (var dir in dirs) {
                var webPath = GetWebPath(dir);
                var dlPath = GetDownloadPath(dir);
                var name = GetItemName(webPath);
                feItems.Add(new FileExplorerItem(name, webPath, dlPath, true, 0));
            }
            var fils = Directory.EnumerateFiles(realPath).ToArray();
            foreach (var fil in fils) {
                var webPath = GetWebPath(fil);
                var dlPath = GetDownloadPath(fil);
                var name = GetItemName(webPath);
                var size = GetFileSize(fil);
                feItems.Add(new FileExplorerItem(name, webPath, dlPath, false, size));
            }
            return feItems.ToArray();
        }

        public FileExplorerOperationResult CreateDirectory(string path) {
            var realPath = GetRealPath(path);
            if (Directory.Exists(realPath)) {
                throw new HttpException(400, $"ProtoCMS: directory '{path}' already exists.");
            }
            Directory.CreateDirectory(realPath);
            var webPath = GetWebPath(realPath);
            var dlPath = GetDownloadPath(realPath);
            var name = GetItemName(webPath);
            var result = new FileExplorerOperationResult(realPath, webPath, name, dlPath, true, null);
            return result;
        }

        public FileExplorerOperationResult CreateFile(string path, Stream fileStream) {
            var realPath = GetRealPath(path);
            var lastDot = realPath.LastIndexOf('.');
            var fileExtWithDot = lastDot == -1 ? "" : realPath.Substring(lastDot);
            if (fileExtWithDot.Length == 0) {
                throw new HttpException(400, $"ProtoCMS: file must have extension.");
            }
            var containingDir = Path.GetDirectoryName(realPath);
            if (!Directory.Exists(containingDir)) {
                var containingDirWebPath = GetWebPath(containingDir);
                throw new HttpException(400, $"ProtoCMS: directory doesn't exists '{containingDirWebPath}'.");
            }
            var realPathWithoutFileExt = lastDot == -1 ? realPath : realPath.Substring(0, lastDot);
            while (File.Exists(realPath)) {
                realPathWithoutFileExt = $"{realPathWithoutFileExt}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                realPath = realPathWithoutFileExt + fileExtWithDot;
            }

            var totalBytesToRead = fileStream.Length;
            if (totalBytesToRead == 0) {
                throw new HttpException(400, $"ProtoCMS: can't create file with empty content.");
            }
            using (var fs = File.OpenWrite(realPath)) {
                var buffer = new byte[1024];
                while (totalBytesToRead > 0) {
                    var readCount = fileStream.Read(buffer, 0, 1024);
                    totalBytesToRead -= readCount;
                    fs.Write(buffer, 0, readCount);
                }
            }
            var webPath = GetWebPath(realPath);
            var dlPath = GetDownloadPath(realPath);
            var name = GetItemName(webPath);
            var result = new FileExplorerOperationResult(realPath, webPath, name, dlPath, false, null);
            return result;
        }
        
        public FileExplorerOperationResult[] Delete(string[] paths) {
            var results = new List<FileExplorerOperationResult>();
            foreach (var path in paths) {
                var realPath = GetRealPath(path);
                var isDir = false;
                var hasDel = false;
                if (Directory.Exists(realPath)) {
                    hasDel = true;
                    isDir = true;
                    Directory.Delete(realPath, true);
                } else if (File.Exists(realPath)) {
                    hasDel = true;
                    File.Delete(realPath);
                }
                if (hasDel) {
                    var webPath = GetWebPath(realPath);
                    var dlPath = GetDownloadPath(realPath);
                    var name = GetItemName(webPath);
                    var result = new FileExplorerOperationResult(realPath, webPath, name, dlPath, isDir, null);
                    results.Add(result);
                }
            }
            return results.ToArray();
        }

        public FileExplorerMoveResult[] CopyToDir(string[] paths, string targetDir) {
            var results = new List<FileExplorerMoveResult>();
            var tgtBasePath = GetRealPath(targetDir);
            foreach (var path in paths) {
                var srcRealPath = GetRealPath(path);
                var srcWebPath = GetWebPath(srcRealPath);
                var srcDlPath = GetDownloadPath(srcRealPath);
                var srcName = GetItemName(srcWebPath);
                var tgtRealPath = Path.Combine(tgtBasePath, srcName);
                var tgtWebPath = GetWebPath(tgtRealPath);
                var tgtDlPath = GetDownloadPath(tgtRealPath);
                var tgtName = GetItemName(tgtWebPath);
                if (Directory.Exists(srcRealPath)) {
                    var bef = new FileExplorerOperationResult(srcRealPath, srcWebPath, srcName, srcDlPath, true, null);
                    var dirCopyErrs = new List<string>();
                    if (!Directory.Exists(tgtRealPath)) {
                        var allSrcFiles = Directory.EnumerateFiles(srcRealPath, "*.*", SearchOption.AllDirectories)
                            .ToArray();
                        foreach (var srcFileRealPath in allSrcFiles) {
                            var srcFileWebPath = GetWebPath(srcFileRealPath);
                            var srcFileName = GetItemName(srcFileWebPath);
                            var tgtFileRealPath = Path.Combine(tgtRealPath, srcFileName);
                            var tgtFileWebPath = GetWebPath(tgtFileRealPath);
                            if (!File.Exists(tgtFileRealPath)) {
                                var tgtFileContainingDir = Path.GetDirectoryName(tgtFileRealPath);
                                if (tgtFileContainingDir != null && !Directory.Exists(tgtFileContainingDir)) {
                                    Directory.CreateDirectory(tgtFileContainingDir);
                                }
                                File.Copy(srcFileRealPath, tgtFileRealPath);
                            } else {
                                dirCopyErrs.Add($"ProtoCMS: file '{tgtFileWebPath}' already exists.");
                            }
                        }
                        var aft = new FileExplorerOperationResult(tgtRealPath, tgtWebPath, tgtName, tgtDlPath, true,
                            dirCopyErrs.Count == 0 ? null : dirCopyErrs.ToArray());
                        var result = new FileExplorerMoveResult(bef, aft);
                        results.Add(result);
                    } else {
                        var aft = new FileExplorerOperationResult(tgtRealPath, tgtWebPath, tgtName, tgtDlPath, true,
                            new[] {
                                $"ProtoCMS: directory '{tgtWebPath}' already exists."
                            });
                        var result = new FileExplorerMoveResult(bef, aft);
                        results.Add(result);
                    }
                } else if (File.Exists(srcRealPath)) {
                    var bef = new FileExplorerOperationResult(srcRealPath, srcWebPath, srcName, srcDlPath, false, null);
                    if (!File.Exists(tgtRealPath)) {
                        var tgtFileContainingDir = Path.GetDirectoryName(tgtRealPath);
                        if (tgtFileContainingDir != null && !Directory.Exists(tgtFileContainingDir)) {
                            Directory.CreateDirectory(tgtFileContainingDir);
                        } else {
                            File.Copy(srcRealPath, tgtRealPath);
                        }
                        var aft = new FileExplorerOperationResult(tgtRealPath, tgtWebPath, tgtName, tgtDlPath, false,
                            null);
                        var result = new FileExplorerMoveResult(bef, aft);
                        results.Add(result);
                    } else {
                        var aft = new FileExplorerOperationResult(tgtRealPath, tgtWebPath, tgtName, tgtDlPath, false,
                            new[] {
                                $"ProtoCMS: file '{tgtWebPath}' already exists."
                            });
                        var result = new FileExplorerMoveResult(bef, aft);
                        results.Add(result);
                    }
                }
            }
            return results.ToArray();
        }

        public FileExplorerMoveResult Rename(string path, string newPath) {
            var srcRealPath = GetRealPath(path);
            var srcWebPath = GetWebPath(srcRealPath);
            var srcDlPath = GetDownloadPath(srcRealPath);
            var srcName = GetItemName(srcWebPath);
            var tgtRealPath = GetRealPath(newPath);
            var tgtWebPath = GetWebPath(tgtRealPath);
            var tgtDlPath = GetDownloadPath(tgtRealPath);
            var tgtName = GetItemName(tgtWebPath);
            if (Directory.Exists(srcRealPath)) {
                var bef = new FileExplorerOperationResult(srcRealPath, srcWebPath, srcName, srcDlPath, true, null);
                if (!Directory.Exists(tgtRealPath)) {
                    var tgtDirContainer = Path.GetDirectoryName(tgtRealPath);
                    if (tgtDirContainer != null && !Directory.Exists(tgtDirContainer)) {
                        Directory.CreateDirectory(tgtDirContainer);
                    }
                    Directory.Move(srcRealPath, tgtRealPath);
                    var aft = new FileExplorerOperationResult(tgtRealPath, tgtWebPath, tgtName, tgtDlPath, true, null);
                    var result = new FileExplorerMoveResult(bef, aft);
                    return result;
                } else {
                    var aft = new FileExplorerOperationResult(tgtRealPath, tgtWebPath, tgtName, tgtDlPath, true, new[] {
                        $"ProtoCMS: directory '{tgtWebPath}' already exists."
                    });
                    var result = new FileExplorerMoveResult(bef, aft);
                    return result;
                }
            }
            if (File.Exists(srcRealPath)) {
                var bef = new FileExplorerOperationResult(srcRealPath, srcWebPath, srcName, srcDlPath, false, null);
                if (!File.Exists(tgtRealPath)) {
                    var tgtFileContainingDir = Path.GetDirectoryName(tgtRealPath);
                    if (tgtFileContainingDir != null && !Directory.Exists(tgtFileContainingDir)) {
                        Directory.CreateDirectory(tgtFileContainingDir);
                    }
                    File.Move(srcRealPath, tgtRealPath);
                    var aft = new FileExplorerOperationResult(tgtRealPath, tgtWebPath, tgtName, tgtDlPath, false, null);
                    var result = new FileExplorerMoveResult(bef, aft);
                    return result;
                } else {
                    var aft = new FileExplorerOperationResult(tgtRealPath, tgtWebPath, tgtName, tgtDlPath, false, new[] {
                        $"ProtoCMS: file '{tgtWebPath}' already exists."
                    });
                    var result = new FileExplorerMoveResult(bef, aft);
                    return result;
                }
            }
            return new FileExplorerMoveResult(
                new FileExplorerOperationResult(srcRealPath, srcWebPath, srcName, srcDlPath, false, null),
                new FileExplorerOperationResult(srcRealPath, srcWebPath, srcName, srcDlPath, false, new[] {
                    $"ProtoCMS: file or directory '{srcWebPath}' doesn't exists."
                })
            );
        }

        public FileExplorerItem PathInfo(string path) {
            var realPath = GetRealPath(path);
            var webPath = GetWebPath(realPath);
            var downloadPath = GetDownloadPath(realPath);
            var name = GetItemName(webPath);
            if (Directory.Exists(realPath)) {
                return new FileExplorerItem(name, webPath, downloadPath, true, 0);
            }
            if (File.Exists(realPath)) {
                var fileSize = GetFileSize(realPath);
                return new FileExplorerItem(name, webPath, downloadPath, false, fileSize);
            } 
            return new FileExplorerItem(name, webPath, downloadPath, false, -1);
        }

        public string GetRealPath(string path) {
            var pathCleaned = string.IsNullOrWhiteSpace(path)
                ? null
                : path.Trim().Replace("/", "\\").Trim('\\');

            var baseDirPath = pathCleaned == null ? BasePath : Path.Combine(BasePath, pathCleaned);
            return baseDirPath;
        }

        public long GetFileSize(string fileRealPath) {
            var fileInf = new FileInfo(fileRealPath);
            var size = fileInf.Length;
            return size;
        }

        public string GetItemName(string webPath) {
            var lastSlash = webPath.LastIndexOf('/');
            var name = webPath.Substring(lastSlash + 1);
            return name;
        }

        public string GetDownloadPath(string realPath) {
            var webRootPath = HostingEnvironment.MapPath("~");
            return realPath.Replace(webRootPath, "").Replace('\\', '/').TrimEnd('/');
        }

        public string GetWebPath(string realPath) {
            return realPath.Replace(BasePath, "").Replace('\\', '/').TrimEnd('/');
        }
    }
}