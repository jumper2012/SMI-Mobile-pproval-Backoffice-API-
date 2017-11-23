using System;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug;

namespace JrzAsp.Lib.ProtoCms.Fields.Publishing {
    public class PublishingField : ContentField, IFauxUrlSlugGenerator {
        private DateTime? _publishedUtc;
        private DateTime? _unpublishedUtc;

        public PublishingField() {
            PublishedUtc = DateTime.Now.ToUniversalTime();
            UnpublishedUtc = null;
        }

        public override ContentFieldSpec __FieldSpec =>
            new ContentFieldSpec(typeof(PublishingFieldFinder), typeof(PublishingFieldModifier),
                typeof(PublishingFieldConfiguration), 1);

        public DateTime? PublishedUtc {
            get => _publishedUtc;
            set {
                _publishedUtc = value;
                if (_publishedUtc != null) _publishedUtc = DateTime.SpecifyKind(_publishedUtc.Value, DateTimeKind.Utc);
            }
        }
        public DateTime? UnpublishedUtc {
            get => _unpublishedUtc;
            set {
                _unpublishedUtc = value;
                if (_unpublishedUtc != null) {
                    _unpublishedUtc = DateTime.SpecifyKind(_unpublishedUtc.Value, DateTimeKind.Utc);
                }
            }
        }

        public bool IsPublished {
            get {
                var now = DateTime.UtcNow;
                return PublishedUtc.HasValue && PublishedUtc.Value <= now &&
                       (!UnpublishedUtc.HasValue || UnpublishedUtc.Value > now);
            }
        }

        public bool IsDraft => !IsPublished;

        public DateTime? PublishedAt => PublishedUtc?.ToLocalTime();
        public DateTime? UnpublishedAt => UnpublishedUtc?.ToLocalTime();

        public string GenerateFauxUrlSlugPart(string param, ContentFieldDefinition fieldDef) {
            if (!PublishedAt.HasValue) return null;
            var dt = PublishedAt.Value;
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