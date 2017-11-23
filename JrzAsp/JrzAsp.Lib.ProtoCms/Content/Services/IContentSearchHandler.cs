using System;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentSearchHandler : IContentHandler {
        Expression<Func<ProtoContent, bool>> HandleSearch(string keywords, string[] splittedKeywords,
            ContentType contentType, out bool callNextHandler);
    }
}