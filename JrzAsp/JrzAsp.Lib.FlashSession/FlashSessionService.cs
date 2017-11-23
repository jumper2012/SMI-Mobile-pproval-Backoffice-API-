using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JrzAsp.Lib.FlashSession {
    public static class FlashSessionService {

        private static HttpSessionStateBase Session => TheHttpContext?.Session;

        private static HttpContextBase TheHttpContext {
            get {
                if (HttpContext.Current == null) {
                    return null;
                }
                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        private static string TempDataKey {
            get {
                if (Session == null) return $"{typeof(FlashSessionService).FullName}:TempData";
                return $"{typeof(FlashSessionService).FullName}:TempData_{Session.SessionID}";
            }
        }

        public static void AddMessage(string message) {
            if (Session == null) return;
            if (Session[TempDataKey] as ICollection<string> == null) Session[TempDataKey] = new List<string>();
            (Session[TempDataKey] as ICollection<string>).Add(message);
        }

        public static void RemoveMessage(int index) {
            var fms = GetMessages().ToList();
            if (index < 0 || index >= fms.Count) return;
            var runIdx = 0;
            var newFms = new List<string>();
            foreach (var fm in fms) {
                if (runIdx != index) newFms.Add(fm);
                runIdx++;
            }
            SetMessages(newFms);
        }

        public static bool HasMessage() {
            return GetMessages().Any();
        }

        public static ICollection<string> GetAndClearMessages() {
            var fms = GetMessages().ToList();
            ClearMessages();
            return fms;
        }

        public static ICollection<string> GetMessages() {
            if (Session != null) {
                if (Session[TempDataKey] as ICollection<string> != null) {
                    return (ICollection<string>) Session[TempDataKey];
                }
                Session[TempDataKey] = new List<string>();
                return (ICollection<string>) Session[TempDataKey];
            }
            return new List<string>();
        }

        public static void SetMessages(IEnumerable<string> messages) {
            if (Session == null) return;
            ClearMessages();
            var fms = new List<string>(messages);
            Session[TempDataKey] = fms;
        }

        public static void ClearMessages() {
            Session?.Remove(TempDataKey);
        }
    }
}