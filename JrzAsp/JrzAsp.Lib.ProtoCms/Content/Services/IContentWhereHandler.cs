using System;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentWhereHandler : IContentHandler {
        Expression<Func<ProtoContent, bool>> HandleWhere(ContentWhereCondition condition,
            object param, ContentType contentType, out bool callNextHandler);
    }
}