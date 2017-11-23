using System;
using System.Linq;
using System.Linq.Expressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.Publishing;
using JrzAsp.Lib.ProtoCms.Fields.Trashing;
using JrzAsp.Lib.QueryableUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public class StandardContentWhereHandler : IContentWhereHandler {
        public string[] HandledContentTypeIds => new[] {ContentType.ANY_CONTENT_TYPE_ID};
        public decimal Priority => 0;

        private readonly PublishingFieldTableFilterHandler _pubHdlr;
        private readonly TrashingFieldTableFilterHandler _traHdlr;

        public StandardContentWhereHandler(PublishingFieldTableFilterHandler pubHdlr, TrashingFieldTableFilterHandler traHdlr) {
            _pubHdlr = pubHdlr;
            _traHdlr = traHdlr;
        }

        public Expression<Func<ProtoContent, bool>> HandleWhere(ContentWhereCondition condition, object param,
            ContentType contentType,
            out bool callNextHandler) {
            callNextHandler = true;
            var pf = new PublishingFieldTableFilterForm();
            var tf = new TrashingFieldTableFilterForm();
            var ok = false;
            if (condition.Is(StandardContentWhereConditionsProvider.IS_PUBLISHED_AND_NOT_TRASHED)) {
                pf.IsPublished = true;
                tf.IsTrashed = false;
                ok = true;
            } else if (condition.Is(StandardContentWhereConditionsProvider.IS_PUBLISHED_AND_TRASHED)) {
                pf.IsPublished = true;
                tf.IsTrashed = true;
                ok = true;
            } else if (condition.Is(StandardContentWhereConditionsProvider.IS_NOT_PUBLISHED_AND_NOT_TRASHED)) {
                pf.IsPublished = false;
                tf.IsTrashed = false;
                ok = true;
            } else if (condition.Is(StandardContentWhereConditionsProvider.IS_NOT_PUBLISHED_AND_TRASHED)) {
                pf.IsPublished = false;
                tf.IsTrashed = true;
                ok = true;
            }
            if (!ok) return null;
            var pred = PredicateBuilder.True<ProtoContent>();

            var pubCond = ContentFinder.DefinedWhereConditions.First(x =>
                x.Is(PublishingFieldWhereConditionsProvider.IS_PUBLISHED_WHERE_CONDITION_NAME));
            var pubPred = _pubHdlr.HandleWhere(pubCond, pf, contentType, out var pubCnh);

            var traCond = ContentFinder.DefinedWhereConditions.First(x =>
                x.Is(TrashingFieldWhereConditionsProvider.IS_TRASHED_WHERE_CONDITION_NAME));
            var traPred = _traHdlr.HandleWhere(traCond, tf, contentType, out var traCnh);

            pred = pred.And(pubPred);
            pred = pred.And(traPred);

            return pred;
        }
    }
}