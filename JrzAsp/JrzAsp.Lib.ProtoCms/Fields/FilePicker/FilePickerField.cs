using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.SimpleField;
using JrzAsp.Lib.ProtoCms.FileSys;

namespace JrzAsp.Lib.ProtoCms.Fields.FilePicker {
    public class FilePickerField : SimpleContentField<string[]> {
        private string[] _downloadPaths;

        private string[] _realPaths;

        private string[] _val;

        private string[] _webPaths;
        public override string[] Val {
            get {
                if (_val != null) return _val;
                _val = new string[0];
                return _val;
            }
            set => _val = value;
        }

        public override ContentFieldSpec __FieldSpec => new ContentFieldSpec(
            typeof(FilePickerFieldFinder),
            typeof(FilePickerFieldModifier),
            typeof(FilePickerFieldConfiguration)
        );

        public string[] RealPaths {
            get {
                if (_realPaths != null) return _realPaths;
                _realPaths = new string[0];
                return _realPaths;
            }
            set => _realPaths = value;
        }
        public string[] WebPaths {
            get {
                if (_webPaths != null) return _webPaths;
                _webPaths = new string[0];
                return _webPaths;
            }
            set => _webPaths = value;
        }
        public string[] DownloadPaths {
            get {
                if (_downloadPaths != null) return _downloadPaths;
                _downloadPaths = new string[0];
                return _downloadPaths;
            }
            set => _downloadPaths = value;
        }

        public void UpdateRelatedPaths(IFileExplorerHandler fileHandler) {
            var realPaths = new List<string>();
            var webPaths = new List<string>();
            var dlPaths = new List<string>();

            foreach (var val in Val) {
                var realPath = fileHandler.GetRealPath(val);
                var webPath = fileHandler.GetWebPath(realPath);
                var dlPath = fileHandler.GetDownloadPath(realPath);
                realPaths.Add(realPath);
                webPaths.Add(webPath);
                dlPaths.Add(dlPath);
            }

            RealPaths = realPaths.ToArray();
            WebPaths = webPaths.ToArray();
            DownloadPaths = dlPaths.ToArray();
        }
    }
}