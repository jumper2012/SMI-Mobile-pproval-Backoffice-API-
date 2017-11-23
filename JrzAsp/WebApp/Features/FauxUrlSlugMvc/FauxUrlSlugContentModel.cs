using JrzAsp.Lib.ProtoCms.Content.Models;

namespace WebApp.Features.FauxUrlSlugMvc {
    public class FauxUrlSlugContentModel {
        public FauxUrlSlugContentModel(ContentType contentType, ProtoContent contentObject, dynamic content) {
            ContentType = contentType;
            ContentObject = contentObject;
            Content = content;
        }

        public ContentType ContentType { get; }
        public ProtoContent ContentObject { get; }
        public dynamic Content { get; }
    }
}