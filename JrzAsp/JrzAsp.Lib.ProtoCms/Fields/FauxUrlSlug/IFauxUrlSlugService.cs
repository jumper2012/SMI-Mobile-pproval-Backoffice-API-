using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug {
    public interface IFauxUrlSlugService : IPerRequestDependency {
        ProtoContent[] FindContentByFauxUrlSlug(string fauxUrlSlug);
        bool IsContentMatchFauxUrlSlug(ProtoContent content, string fauxUrlSlug);
    }
}