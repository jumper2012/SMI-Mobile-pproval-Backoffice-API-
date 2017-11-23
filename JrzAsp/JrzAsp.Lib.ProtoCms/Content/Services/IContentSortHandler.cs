using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentSortHandler : IContentHandler {
        IQueryable<ProtoContent> HandleSort(IQueryable<ProtoContent> currentQuery,
            string fieldName,
            bool descending,
            ContentType contentType,
            out bool callNextHandler);
    }
}