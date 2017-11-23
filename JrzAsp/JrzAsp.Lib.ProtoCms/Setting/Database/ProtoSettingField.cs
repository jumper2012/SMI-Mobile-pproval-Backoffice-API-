using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.Setting.Database {
    public class ProtoSettingField : IHaveTimestamps {

        private DateTime _createdUtc;
        private bool _createdUtcKindNeedRefresh = true;

        private DateTime _updatedUtc;
        private bool _updatedUtcKindNeedRefresh = true;

        public ProtoSettingField() {
            Id = Guid.NewGuid().ToString();
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = DateTime.UtcNow;
        }

        [Key]
        [Required]
        [MaxLength(128)]
        public string Id { get; set; }

        [Required]
        [MaxLength(128)]
        [Index("IX_ProtoSettingField_SettingId")]
        public string SettingId { get; set; }

        [Required]
        public string FieldName { get; set; }

        public string StringValue { get; set; }
        public decimal? NumberValue { get; set; }
        public bool? BooleanValue { get; set; }
        public DateTime? DateTimeValue { get; set; }

        [Required]
        public DateTime CreatedUtc {
            get {
                if (_createdUtcKindNeedRefresh) {
                    _createdUtc = DateTime.SpecifyKind(_createdUtc, DateTimeKind.Utc);
                    _createdUtcKindNeedRefresh = false;
                }
                return _createdUtc;
            }
            set {
                _createdUtc = value;
                _createdUtcKindNeedRefresh = true;
            }
        }

        [Required]
        public DateTime UpdatedUtc {
            get {
                if (_updatedUtcKindNeedRefresh) {
                    _updatedUtc = DateTime.SpecifyKind(_updatedUtc, DateTimeKind.Utc);
                    _updatedUtcKindNeedRefresh = false;
                }
                return _updatedUtc;
            }
            set {
                _updatedUtc = value;
                _updatedUtcKindNeedRefresh = true;
            }
        }
    }
}