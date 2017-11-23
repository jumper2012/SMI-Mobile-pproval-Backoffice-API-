using System;

namespace JrzAsp.Lib.ProtoCms.Fields.Chrono {
    public class ChronoSearchInfo {
        public ChronoSearchInfo(bool foundDate, bool foundTime, bool foundInteger, DateTime theDateTime,
            int theInteger) {
            FoundDate = foundDate;
            FoundTime = foundTime;
            FoundInteger = foundInteger;
            TheDateTime = theDateTime;
            TheInteger = theInteger;
        }

        public bool FoundDate { get; }
        public bool FoundTime { get; }
        public bool FoundInteger { get; }
        public DateTime TheDateTime { get; }
        public int TheInteger { get; }
    }
}