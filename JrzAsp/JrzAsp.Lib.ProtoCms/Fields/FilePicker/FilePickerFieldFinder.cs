using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.FileSys;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.FilePicker {
    public class FilePickerFieldFinder : IContentFieldFinder {
        private readonly IFileExplorerManager _fileMgr;
        private readonly IProtoCmsMainUrlsProvider _urlProv;

        public FilePickerFieldFinder(IFileExplorerManager fileMgr, IProtoCmsMainUrlsProvider urlProv) {
            _fileMgr = fileMgr;
            _urlProv = urlProv;
        }

        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(
                nameof(FilePickerField.Val),
                cfd => cfd.Config.Label ?? cfd.FieldName,
                cfd => true,
                CellValueForVal,
                SummarizedValueForVal,
                FullPreviewValueForVal
            ),
            new ContentFieldColumn(
                nameof(FilePickerField.DownloadPaths),
                cfd => $"{cfd.Config.Label ?? cfd.FieldName} (Download)",
                cfd => false,
                CellValueForDownload,
                SummarizedValueForDownload,
                FullPreviewValueForDownload
            ),
            new ContentFieldColumn(
                nameof(FilePickerField.WebPaths),
                cfd => $"{cfd.Config.Label ?? cfd.FieldName} (Web)",
                cfd => false,
                CellValueForWeb,
                SummarizedValueForWeb,
                FullPreviewValueForWeb
            ),
            new ContentFieldColumn(
                nameof(FilePickerField.RealPaths),
                cfd => $"{cfd.Config.Label ?? cfd.FieldName} (Real)",
                cfd => false,
                CellValueForReal,
                SummarizedValueForReal,
                FullPreviewValueForReal
            )
        };

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var fcfg = fieldDefinition.Config.DirectCastTo<FilePickerFieldConfiguration>();
            var mdl = new FilePickerField();
            var fnPrefix = $"{fieldDefinition.FieldName}.{nameof(FilePickerField.Val)}.";
            if (!fcfg.IsMultiSelect) {
                var cf = content.ContentFields.FirstOrDefault(x => x.FieldName.StartsWith(fnPrefix));
                if (cf?.StringValue != null) {
                    mdl.Val = new[] {
                        cf.StringValue
                    };
                }
            } else {
                var cfs = content.ContentFields.Where(
                    x => x.FieldName.StartsWith(fnPrefix)).ToArray();
                var vals = new List<Tuple<int, string>>();
                foreach (var cf in cfs) {
                    var idxStr = cf.FieldName.Replace(fnPrefix, "");
                    if (!int.TryParse(idxStr, out var idx)) continue;
                    if (cf.StringValue != null) {
                        vals.Add(Tuple.Create(idx, cf.StringValue));
                    }
                }
                mdl.Val = vals.OrderBy(x => x.Item1).Select(x => x.Item2).ToArray();
            }
            mdl.UpdateRelatedPaths(_fileMgr.GetHandler());
            return mdl;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            var fnPrefix = $"{fieldDefinition.FieldName}.{nameof(FilePickerField.Val)}.";
            return x => x.ContentFields.Any(cf => splittedKeywords.Any(k =>
                cf.FieldName.StartsWith(fnPrefix) && cf.StringValue.Contains(k)));
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            var fnPrefix = $"{fieldDefinition.FieldName}.{nameof(FilePickerField.Val)}.";
            return currentQuery.AddOrderBy(x =>
                x.ContentFields.FirstOrDefault(cf => cf.FieldName.StartsWith(fnPrefix)).StringValue);
        }

        private VueComponentDefinition[] FullPreviewValueForVal(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            var vues = new List<VueComponentDefinition>();
            var fi = contentField.DirectCastTo<FilePickerField>();
            var idx = 0;
            if (fi.Val.Length != fi.DownloadPaths.Length) {
                fi.UpdateRelatedPaths(_fileMgr.GetHandler());
            }
            foreach (var val in fi.Val) {
                vues.Add(new VueHtmlWidget($"<li><a href='{_urlProv.GetBaseWebsiteContentUrl()}{fi.DownloadPaths[idx]}' target='_blank'>{val}</a></li>"));
                idx++;
            }
            return vues.ToArray();
        }

        private string SummarizedValueForVal(ContentField contentField, ContentFieldDefinition contentFieldDefinition) {
            var fi = contentField.DirectCastTo<FilePickerField>();
            var fcfg = contentFieldDefinition.Config.DirectCastTo<FilePickerFieldConfiguration>();
            if (fcfg.IsMultiSelect) {
                return $"{fi.Val.Length} file{(fi.Val.Length > 1 ? "s" : "")}";
            }
            return fi.Val.FirstOrDefault();
        }

        private VueComponentDefinition[] CellValueForVal(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            return FullPreviewValueForVal(contentField, contentFieldDefinition);
        }

        private VueComponentDefinition[] FullPreviewValueForDownload(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            var vues = new List<VueComponentDefinition>();
            var fi = contentField.DirectCastTo<FilePickerField>();
            var idx = 0;
            if (fi.Val.Length != fi.DownloadPaths.Length) {
                fi.UpdateRelatedPaths(_fileMgr.GetHandler());
            }
            foreach (var val in fi.DownloadPaths) {
                vues.Add(new VueHtmlWidget($"<li><a href='{_urlProv.GetBaseWebsiteContentUrl()}{fi.DownloadPaths[idx]}' target='_blank'>{val}</a></li>"));
                idx++;
            }
            return vues.ToArray();
        }

        private string SummarizedValueForDownload(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            var fi = contentField.DirectCastTo<FilePickerField>();
            var fcfg = contentFieldDefinition.Config.DirectCastTo<FilePickerFieldConfiguration>();
            if (fcfg.IsMultiSelect) {
                return $"{fi.DownloadPaths.Length} file{(fi.DownloadPaths.Length > 1 ? "s" : "")}";
            }
            return fi.DownloadPaths.FirstOrDefault();
        }

        private VueComponentDefinition[] CellValueForDownload(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            return FullPreviewValueForDownload(contentField, contentFieldDefinition);
        }

        private VueComponentDefinition[] FullPreviewValueForWeb(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            var vues = new List<VueComponentDefinition>();
            var fi = contentField.DirectCastTo<FilePickerField>();
            var idx = 0;
            if (fi.Val.Length != fi.DownloadPaths.Length) {
                fi.UpdateRelatedPaths(_fileMgr.GetHandler());
            }
            foreach (var val in fi.WebPaths) {
                vues.Add(new VueHtmlWidget($"<li><a href='{_urlProv.GetBaseWebsiteContentUrl()}{fi.DownloadPaths[idx]}' target='_blank'>{val}</a></li>"));
                idx++;
            }
            return vues.ToArray();
        }

        private string SummarizedValueForWeb(ContentField contentField, ContentFieldDefinition contentFieldDefinition) {
            var fi = contentField.DirectCastTo<FilePickerField>();
            var fcfg = contentFieldDefinition.Config.DirectCastTo<FilePickerFieldConfiguration>();
            if (fcfg.IsMultiSelect) {
                return $"{fi.WebPaths.Length} file{(fi.WebPaths.Length > 1 ? "s" : "")}";
            }
            return fi.WebPaths.FirstOrDefault();
        }

        private VueComponentDefinition[] CellValueForWeb(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            return FullPreviewValueForWeb(contentField, contentFieldDefinition);
        }

        private VueComponentDefinition[] FullPreviewValueForReal(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            var vues = new List<VueComponentDefinition>();
            var fi = contentField.DirectCastTo<FilePickerField>();
            var idx = 0;
            if (fi.Val.Length != fi.DownloadPaths.Length) {
                fi.UpdateRelatedPaths(_fileMgr.GetHandler());
            }
            foreach (var val in fi.RealPaths) {
                vues.Add(new VueHtmlWidget($"<li><a href='{_urlProv.GetBaseWebsiteContentUrl()}{fi.DownloadPaths[idx]}' target='_blank'>{val}</a></li>"));
                idx++;
            }
            return vues.ToArray();
        }

        private string SummarizedValueForReal(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            var fi = contentField.DirectCastTo<FilePickerField>();
            var fcfg = contentFieldDefinition.Config.DirectCastTo<FilePickerFieldConfiguration>();
            if (fcfg.IsMultiSelect) {
                return $"{fi.RealPaths.Length} file{(fi.RealPaths.Length > 1 ? "s" : "")}";
            }
            return fi.RealPaths.FirstOrDefault();
        }

        private VueComponentDefinition[] CellValueForReal(ContentField contentField,
            ContentFieldDefinition contentFieldDefinition) {
            return FullPreviewValueForReal(contentField, contentFieldDefinition);
        }
    }
}