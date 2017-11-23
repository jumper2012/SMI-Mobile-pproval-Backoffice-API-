using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JrzAsp.Lib.ProtoCms.Core.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Models {
    public class ProtoContent : IHaveTimestamps {

        public const string DATE_FORMAT_FOR_DISPLAY = "ddd, dd-MMM-yyyy HH:mm:ss (K)";

        private ICollection<ProtoField> _contentFields;

        private DateTime _createdUtc;
        private bool _createdUtcKindNeedRefresh = true;

        private DateTime _updatedUtc;
        private bool _updatedUtcKindNeedRefresh = true;

        public ProtoContent() {
            Id = Guid.NewGuid().ToString();
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = CreatedUtc;
        }

        [Key]
        [MaxLength(128)]
        public string Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string ContentTypeId { get; set; }

        [InverseProperty(nameof(ProtoField.Content))]
        public virtual ICollection<ProtoField> ContentFields {
            get => _contentFields ?? (_contentFields = new List<ProtoField>());
            set => _contentFields = value;
        }

        [MaxLength(128)]
        public string CreatedByUserId { get; set; }

        [MaxLength(256)]
        public string CreatedByUserName { get; set; }

        [MaxLength(256)]
        public string CreatedByUserDisplayName { get; set; }

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