using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Number {
    public class NumberFieldRangeValidatorConfiguration : ContentFieldValidatorConfiguration {
        private string _maxValueErrorMessage;
        private string _maxValueInclusiveErrorMessage;
        private string _minValueErrorMessage;
        private string _minValueInclusiveErrorMessage;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public bool MinValueIsInclusive { get; set; } = true;
        public bool MaxValueIsInclusive { get; set; } = true;
        public string MinValueErrorMessage {
            get {
                if (_minValueErrorMessage == null) {
                    _minValueErrorMessage = "Field '{0}' minimum value should be {1}.";
                }
                return _minValueErrorMessage;
            }
            set => _minValueErrorMessage = value;
        }
        public string MaxValueErrorMessage {
            get {
                if (_maxValueErrorMessage == null) {
                    _maxValueErrorMessage = "Field '{0}' maximum value should be {1}.";
                }
                return _maxValueErrorMessage;
            }
            set => _maxValueErrorMessage = value;
        }
        public string MinValueInclusiveErrorMessage {
            get {
                if (_minValueInclusiveErrorMessage == null) {
                    _minValueInclusiveErrorMessage = "Field '{0}' minimum value should be {1} (inclusive).";
                }
                return _minValueInclusiveErrorMessage;
            }
            set => _minValueInclusiveErrorMessage = value;
        }
        public string MaxValueInclusiveErrorMessage {
            get {
                if (_maxValueInclusiveErrorMessage == null) {
                    _maxValueInclusiveErrorMessage = "Field '{0}' maximum value should be {1} (inclusive).";
                }
                return _maxValueInclusiveErrorMessage;
            }
            set => _maxValueInclusiveErrorMessage = value;
        }
    }
}