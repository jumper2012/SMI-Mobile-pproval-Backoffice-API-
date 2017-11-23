using System;
using System.Globalization;
using System.Linq.Expressions;
using JrzAsp.Lib.QueryableUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoUtils {

        static ChronoUtils() {
            Self = new ChronoUtils();
        }

        public static ChronoUtils Self { get; }

        public bool TryBuildSearchConditionExpression<TModel>(
            out Expression<Func<TModel, bool>> searchCondition, string keyword,
            Expression<Func<TModel, DateTime?>> dateTimePropExpression, bool dateTimePropIsUtc) {

            var dtpe = dateTimePropExpression;

            var hasValExp = Expression.PropertyOrField(dtpe.Body, nameof(Nullable<DateTime>.HasValue));
            var valExp = Expression.PropertyOrField(dtpe.Body, nameof(Nullable<DateTime>.Value));

            var hasValExpLamb = Expression.Lambda<Func<TModel, bool>>(hasValExp, dtpe.Parameters);
            var valExpLamb = Expression.Lambda<Func<TModel, DateTime>>(valExp, dtpe.Parameters);

            if (TryBuildSearchConditionExpression(out var scond, keyword, valExpLamb, dateTimePropIsUtc)
            ) {
                searchCondition = PredicateBuilder.True<TModel>();
                searchCondition = searchCondition.And(hasValExpLamb);
                searchCondition = searchCondition.And(scond);
                return true;
            }
            searchCondition = PredicateBuilder.False<TModel>();
            return false;
        }

        public bool TryBuildSearchConditionExpression<TModel>(out Expression<Func<TModel, bool>> searchCondition,
            string keyword, Expression<Func<TModel, DateTime>> dateTimePropExpression, bool dateTimePropIsUtc) {
            var inf = SearchDateTimeIn(keyword);
            if (!inf.FoundDate && !inf.FoundTime && !inf.FoundInteger) {
                searchCondition = PredicateBuilder.False<TModel>();
                return false;
            }
            searchCondition = PredicateBuilder.False<TModel>();
            var dtpe = dateTimePropExpression;
            var isUtc = dateTimePropIsUtc;

            if (inf.FoundDate) {
                var cond = PredicateBuilder.True<TModel>();
                var yrExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Year));
                var moExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Month));
                var dyExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Day));

                var yrEqExp = Expression.Equal(yrExp, Expression.Constant(inf.TheDateTime.Year));
                var moEqExp = Expression.Equal(moExp, Expression.Constant(inf.TheDateTime.Month));
                var dyEqExp = Expression.Equal(dyExp, Expression.Constant(inf.TheDateTime.Day));

                var yrEqExpLamb = Expression.Lambda<Func<TModel, bool>>(yrEqExp, dtpe.Parameters);
                var moEqExpLamb = Expression.Lambda<Func<TModel, bool>>(moEqExp, dtpe.Parameters);
                var dyEqExpLamb = Expression.Lambda<Func<TModel, bool>>(dyEqExp, dtpe.Parameters);

                cond = cond.And(yrEqExpLamb);
                cond = cond.And(moEqExpLamb);
                cond = cond.And(dyEqExpLamb);

                searchCondition = searchCondition.Or(cond);

                if (isUtc) {
                    var dtLoc = DateTime.SpecifyKind(inf.TheDateTime, DateTimeKind.Utc).ToLocalTime();
                    yrEqExp = Expression.Equal(yrExp, Expression.Constant(dtLoc.Year));
                    moEqExp = Expression.Equal(moExp, Expression.Constant(dtLoc.Month));
                    dyEqExp = Expression.Equal(dyExp, Expression.Constant(dtLoc.Day));

                    yrEqExpLamb = Expression.Lambda<Func<TModel, bool>>(yrEqExp, dtpe.Parameters);
                    moEqExpLamb = Expression.Lambda<Func<TModel, bool>>(moEqExp, dtpe.Parameters);
                    dyEqExpLamb = Expression.Lambda<Func<TModel, bool>>(dyEqExp, dtpe.Parameters);

                    var condLoc = PredicateBuilder.True<TModel>();
                    condLoc = condLoc.And(yrEqExpLamb);
                    condLoc = condLoc.And(moEqExpLamb);
                    condLoc = condLoc.And(dyEqExpLamb);

                    searchCondition = searchCondition.Or(condLoc);
                }

            }
            if (inf.FoundTime) {
                var hhExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Hour));
                var mmExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Minute));

                var hhEqExp = Expression.Equal(hhExp, Expression.Constant(inf.TheDateTime.Hour));
                var mmEqExp = Expression.Equal(mmExp, Expression.Constant(inf.TheDateTime.Minute));

                var hhEqExpLamb = Expression.Lambda<Func<TModel, bool>>(hhEqExp, dtpe.Parameters);
                var mmEqExpLamb = Expression.Lambda<Func<TModel, bool>>(mmEqExp, dtpe.Parameters);

                var cond = PredicateBuilder.True<TModel>();
                cond = cond.And(hhEqExpLamb);
                cond = cond.And(mmEqExpLamb);

                searchCondition = searchCondition.Or(cond);
                if (isUtc) {
                    var dtLoc = DateTime.SpecifyKind(inf.TheDateTime, DateTimeKind.Utc).ToLocalTime();
                    hhEqExp = Expression.Equal(hhExp, Expression.Constant(dtLoc.Hour));
                    mmEqExp = Expression.Equal(mmExp, Expression.Constant(dtLoc.Minute));

                    hhEqExpLamb = Expression.Lambda<Func<TModel, bool>>(hhEqExp, dtpe.Parameters);
                    mmEqExpLamb = Expression.Lambda<Func<TModel, bool>>(mmEqExp, dtpe.Parameters);

                    var condLoc = PredicateBuilder.True<TModel>();
                    condLoc = condLoc.And(hhEqExpLamb);
                    condLoc = condLoc.And(mmEqExpLamb);

                    searchCondition = searchCondition.Or(condLoc);
                }
            }
            if (inf.FoundInteger) {
                var yrExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Year));
                var moExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Month));
                var dyExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Day));
                var hhExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Hour));
                var mmExp = Expression.PropertyOrField(dtpe.Body, nameof(DateTime.Minute));

                var yrEqExp = Expression.Equal(yrExp, Expression.Constant(inf.TheInteger));
                var moEqExp = Expression.Equal(moExp, Expression.Constant(inf.TheInteger));
                var dyEqExp = Expression.Equal(dyExp, Expression.Constant(inf.TheInteger));
                var hhEqExp = Expression.Equal(hhExp, Expression.Constant(inf.TheInteger));
                var mmEqExp = Expression.Equal(mmExp, Expression.Constant(inf.TheInteger));

                var cond = PredicateBuilder.False<TModel>();
                foreach (var binExp in new[] {yrEqExp, moEqExp, dyEqExp, hhEqExp, mmEqExp}) {
                    var binExpLamb = Expression.Lambda<Func<TModel, bool>>(binExp, dtpe.Parameters);
                    cond = cond.Or(binExpLamb);
                }

                searchCondition = searchCondition.Or(cond);
            }
            return true;
        }

        public ChronoSearchInfo SearchDateTimeIn(string keyword) {
            var foundDate = false;
            var foundTime = false;
            var foundInteger = false;
            var dt = DateTime.Now;
            var intgr = 0;
            if (DateTime.TryParseExact(keyword, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt) || DateTime.TryParseExact(keyword, "yyyy/MM/ddTHH:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt) || DateTime.TryParseExact(keyword, "dd-MM-yyyyTHH:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt) || DateTime.TryParseExact(keyword, "dd/MM/yyyyTHH:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt) || DateTime.TryParseExact(keyword, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt) || DateTime.TryParseExact(keyword, "yyyy/MM/ddTHH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt) || DateTime.TryParseExact(keyword, "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt) || DateTime.TryParseExact(keyword, "dd/MM/yyyyTHH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dt)) {
                foundDate = true;
                foundTime = true;
            } else if (DateTime.TryParseExact(keyword, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                           out dt) || DateTime.TryParseExact(keyword, "yyyy/MM/dd", CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out dt) || DateTime.TryParseExact(keyword, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out dt) || DateTime.TryParseExact(keyword, "dd/MM/yyyy",
                           CultureInfo.InvariantCulture, DateTimeStyles.None,
                           out dt)) {
                foundDate = true;
            } else if (DateTime.TryParseExact(keyword, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                           out dt) ||
                       DateTime.TryParseExact(keyword, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None,
                           out dt)) {
                foundTime = true;
            } else if (int.TryParse(keyword, out intgr)) {
                foundInteger = true;
            }
            return new ChronoSearchInfo(foundDate, foundTime, foundInteger, dt, intgr);
        }
    }
}