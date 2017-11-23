using System;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ContentSelectFieldOptionsHandlerParam {
        private string[] _contentTypeIds;
        private Tuple<string, bool>[] _sortInfos;
        private Tuple<string, object>[] _whereConditions;
        public string[] ContentTypeIds {
            get {
                if (_contentTypeIds != null) return _contentTypeIds;
                _contentTypeIds = new string[0];
                return _contentTypeIds;
            }
            set => _contentTypeIds = value;
        }
        /// <summary>
        ///     Array of tuples of where condition name and where param.
        ///     This works only if <see cref="ContentTypeIds"/> contains only 1 content type id.
        /// </summary>
        public Tuple<string, object>[] WhereConditions {
            get {
                if (_whereConditions != null) return _whereConditions;
                _whereConditions = new Tuple<string, object>[0];
                return _whereConditions;
            }
            set => _whereConditions = value;
        }
        /// <summary>
        ///     Array of tuples of field name and is descending bool.
        ///     This works only if <see cref="ContentTypeIds"/> contains only 1 content type id.
        /// </summary>
        public Tuple<string, bool>[] SortInfos {
            get {
                if (_sortInfos != null) return _sortInfos;
                _sortInfos = new Tuple<string, bool>[0];
                return _sortInfos;
            }
            set => _sortInfos = value;
        }
    }
}