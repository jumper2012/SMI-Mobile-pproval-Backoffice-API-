using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public interface IContentFieldFinder : IPerRequestDependency {
        ContentFieldColumn[] Columns { get; }

        ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition);

        Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition);

        IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName, bool isDescending,
            ContentFieldDefinition fieldDefinition);
    }
}