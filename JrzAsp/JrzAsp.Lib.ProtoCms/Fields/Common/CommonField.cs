using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug;
using JrzAsp.Lib.ProtoCms.User.Services;

namespace JrzAsp.Lib.ProtoCms.Fields.Common {
    public class CommonField : ContentField, IFauxUrlSlugGenerator {

        private DateTime _createdUtc;
        private DateTime _updatedUtc;

        public override ContentFieldSpec __FieldSpec =>
            new ContentFieldSpec(typeof(CommonFieldFinder), typeof(CommonFieldModifier),
                typeof(CommonFieldConfiguration), 1);

        public string CreatedByUserId { get; set; }

        public string CreatedByUserName { get; set; }

        public string CreatedByUserDisplayName { get; set; }

        public DateTime CreatedUtc {
            get => DateTime.SpecifyKind(_createdUtc, DateTimeKind.Utc);
            set => _createdUtc = value;
        }
        public DateTime UpdatedUtc {
            get => DateTime.SpecifyKind(_updatedUtc, DateTimeKind.Utc);
            set => _updatedUtc = value;
        }
        public string ContentTypeId { get; set; }
        public string Id { get; set; }
        public DateTime CreatedAt => CreatedUtc.ToLocalTime();
        public DateTime UpdatedAt => UpdatedUtc.ToLocalTime();

        public string GenerateFauxUrlSlugPart(string param, ContentFieldDefinition fieldDef) {
            switch (param) {
                case "year":
                    return CreatedAt.ToString("yyyy");
                case "month":
                    return CreatedAt.ToString("MM");
                case "date":
                    return CreatedAt.ToString("dd");
                case "hour":
                    return CreatedAt.ToString("HH");
                case "minute":
                    return CreatedAt.ToString("mm");
                case "second":
                    return CreatedAt.ToString("ss");
            }
            return CreatedAt.ToString("yyyy-MM-dd-HH-mm-ss");
        }

    }
}