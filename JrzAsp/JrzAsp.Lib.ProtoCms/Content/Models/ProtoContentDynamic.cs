using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ProtoContentDynamic : DynamicObject {

        public ProtoContentDynamic(ProtoContent contentObject) {
            ContentObject = contentObject;
            ContentType = ContentType.DefinedTypes.First(x => x.Id == ContentObject.ContentTypeId);
        }

        [JsonIgnore]
        public ProtoContent ContentObject { get; }

        [JsonIgnore]
        public ContentType ContentType { get; }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            foreach (var fd in ContentType.Fields) {
                if (binder.Name != fd.FieldName && (!binder.IgnoreCase || !string.Equals(binder.Name, fd.FieldName,
                                                        StringComparison.InvariantCultureIgnoreCase))) {
                    continue;
                }
                var finder = fd.FieldFinder();
                result = finder.GetModel(ContentObject, fd);
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames() {
            return ContentType.Fields.Select(fd => fd.FieldName);
        }
    }
}