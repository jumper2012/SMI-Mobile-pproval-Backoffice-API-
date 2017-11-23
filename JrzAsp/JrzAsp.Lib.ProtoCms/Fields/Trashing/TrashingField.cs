using System;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Trashing {
    public class TrashingField : ContentField {
        private DateTime? _trashedUtc;

        public override ContentFieldSpec __FieldSpec =>
            new ContentFieldSpec(typeof(TrashingFieldFinder), typeof(TrashingFieldModifier),
                typeof(TrashingFieldConfiguration), 1);

        public DateTime? TrashedUtc {
            get => _trashedUtc;
            set {
                _trashedUtc = value;
                if (_trashedUtc != null) _trashedUtc = DateTime.SpecifyKind(_trashedUtc.Value, DateTimeKind.Utc);
            }
        }

        public bool IsTrashed {
            get {
                var now = DateTime.UtcNow;
                return TrashedUtc.HasValue && TrashedUtc.Value <= now;
            }
        }

        public DateTime? TrashedAt => TrashedUtc?.ToLocalTime();
    }
}