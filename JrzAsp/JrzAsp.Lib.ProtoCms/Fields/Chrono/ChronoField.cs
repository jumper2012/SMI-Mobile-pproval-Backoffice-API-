using System;
using JrzAsp.Lib.TypeUtilities;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug;
using JrzAsp.Lib.ProtoCms.Fields.SimpleField;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoField : SimpleContentField<DateTime?>, IFauxUrlSlugGenerator {

        public override ContentFieldSpec __FieldSpec =>
            new ContentFieldSpec(typeof(ChronoFieldFinder), typeof(ChronoFieldModifier),
                typeof(ChronoFieldConfiguration));

        public DateTime? ValAsUtcDateTime =>
            Val.HasValue ? DateTime.SpecifyKind(Val.Value, DateTimeKind.Utc) : (DateTime?) null;

        public DateTime? ValAsLocalDateTime =>
            Val.HasValue ? DateTime.SpecifyKind(Val.Value, DateTimeKind.Local) : (DateTime?) null;

        public string GenerateFauxUrlSlugPart(string param, ContentFieldDefinition fieldDef) {
            if (!Val.HasValue) return null;
            var fcfg = fieldDef.Config.DirectCastTo<ChronoFieldConfiguration>();
            var dt = fcfg.Kind == DateTimeKind.Utc ? ValAsUtcDateTime.Value : ValAsLocalDateTime.Value;
            switch (param) {
                case "year":
                    return dt.ToString("yyyy");
                case "month":
                    return dt.ToString("MM");
                case "date":
                    return dt.ToString("dd");
                case "hour":
                    return dt.ToString("HH");
                case "minute":
                    return dt.ToString("mm");
                case "second":
                    return dt.ToString("ss");
            }
            return dt.ToString("yyyy-MM-dd-HH-mm-ss");
        }
    }
}