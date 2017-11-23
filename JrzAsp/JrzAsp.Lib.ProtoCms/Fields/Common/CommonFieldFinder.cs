using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Fields.Chrono;
using JrzAsp.Lib.ProtoCms.User.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Common {
    public class CommonFieldFinder : IContentFieldFinder {

        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(nameof(CommonField.CreatedByUserDisplayName),
                def => "Created By",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedByUserDisplayName)
                },
                (field, def) => field.DirectCastTo<CommonField>().CreatedByUserDisplayName,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedByUserDisplayName)
                }),
            new ContentFieldColumn(nameof(CommonField.CreatedByUserName),
                def => "Creator UserName",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedByUserName)
                },
                (field, def) => field.DirectCastTo<CommonField>().CreatedByUserName,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedByUserName)
                }),
            new ContentFieldColumn(nameof(CommonField.CreatedByUserId),
                def => "Creator Id",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedByUserId)
                },
                (field, def) => field.DirectCastTo<CommonField>().CreatedByUserId,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedByUserId)
                }),
            new ContentFieldColumn(nameof(CommonField.CreatedUtc),
                def => "Created UTC",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedUtc
                        .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat))
                },
                (field, def) => field.DirectCastTo<CommonField>().CreatedUtc
                    .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedUtc
                        .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat))
                }),
            new ContentFieldColumn(nameof(CommonField.CreatedAt),
                def => "Created At",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedAt
                        .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat))
                },
                (field, def) => field.DirectCastTo<CommonField>().CreatedAt
                    .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().CreatedAt
                        .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat))
                }),
            new ContentFieldColumn(nameof(CommonField.UpdatedUtc),
                def => "Updated UTC",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().UpdatedUtc
                        .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat))
                },
                (field, def) => field.DirectCastTo<CommonField>().UpdatedUtc
                    .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().UpdatedUtc
                        .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat))
                }),
            new ContentFieldColumn(nameof(CommonField.UpdatedAt),
                def => "Updated At",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().UpdatedAt
                        .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat))
                },
                (field, def) => field.DirectCastTo<CommonField>().UpdatedAt
                    .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat),
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().UpdatedAt
                        .ToString(def.Config.DirectCastTo<CommonFieldConfiguration>().DateTimeFormat))
                }),
            new ContentFieldColumn(nameof(CommonField.ContentTypeId),
                def => "Content Type Id",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().ContentTypeId)
                },
                (field, def) => field.DirectCastTo<CommonField>().ContentTypeId,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().ContentTypeId)
                }),
            new ContentFieldColumn(nameof(CommonField.Id),
                def => "Id",
                def => true,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().Id)
                },
                (field, def) => field.DirectCastTo<CommonField>().Id,
                (field, def) => new VueComponentDefinition[] {
                    new VueHtmlWidget(field.DirectCastTo<CommonField>().Id)
                })
        };

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var model = new CommonField {
                Id = content.Id,
                ContentTypeId = content.ContentTypeId,
                CreatedUtc = content.CreatedUtc,
                UpdatedUtc = content.UpdatedUtc,
                CreatedByUserId = content.CreatedByUserId,
                CreatedByUserName = content.CreatedByUserName,
                CreatedByUserDisplayName = content.CreatedByUserDisplayName
            };
            return model;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            var pred = PredicateBuilder.False<ProtoContent>();
            foreach (var kw in splittedKeywords) {
                pred = pred.Or(x => x.Id == kw || x.ContentTypeId == kw);
                if (ChronoUtils.Self.TryBuildSearchConditionExpression<ProtoContent>(out var sccond, kw,
                    x => x.CreatedUtc, true)) {
                    pred = pred.Or(sccond);
                }
                if (ChronoUtils.Self.TryBuildSearchConditionExpression<ProtoContent>(out var sucond, kw,
                    x => x.UpdatedUtc, true)) {
                    pred = pred.Or(sucond);
                }
            }
            return pred;
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            var q = currentQuery;
            var idfn = $"{fieldDefinition.FieldName}.{nameof(CommonField.Id)}";
            var ctfn = $"{fieldDefinition.FieldName}.{nameof(CommonField.ContentTypeId)}";
            var crfn = $"{fieldDefinition.FieldName}.{nameof(CommonField.CreatedUtc)}";
            var upfn = $"{fieldDefinition.FieldName}.{nameof(CommonField.UpdatedUtc)}";
            var cuidfn = $"{fieldDefinition.FieldName}.{nameof(CommonField.CreatedByUserId)}";
            var cunmfn = $"{fieldDefinition.FieldName}.{nameof(CommonField.CreatedByUserName)}";
            var cudnfn = $"{fieldDefinition.FieldName}.{nameof(CommonField.CreatedByUserDisplayName)}";
            if (sortFieldName == idfn) {
                q = q.AddOrderBy(x => x.Id, isDescending);
            } else if (sortFieldName == ctfn) {
                q = q.AddOrderBy(x => x.ContentTypeId, isDescending);
            } else if (sortFieldName == crfn) {
                q = q.AddOrderBy(x => x.CreatedUtc, isDescending);
            } else if (sortFieldName == upfn) {
                q = q.AddOrderBy(x => x.UpdatedUtc, isDescending);
            } else if (sortFieldName == cuidfn) {
                q = q.AddOrderBy(x => x.CreatedByUserId, isDescending);
            } else if (sortFieldName == cunmfn) {
                q = q.AddOrderBy(x => x.CreatedByUserName, isDescending);
            } else if (sortFieldName == cudnfn) {
                q = q.AddOrderBy(x => x.CreatedByUserDisplayName, isDescending);
            }
            return q;
        }
    }
}