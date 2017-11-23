using System;
using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.CodeRunner {
    public sealed class CodeRunResult {

        private CodeRunResult() {
            ErrorMessages = new string[0];
            Exceptions = new Exception[0];
        }

        public bool Success {
            get {
                if (ErrorMessages == null && Exceptions == null) return true;
                if (ErrorMessages?.Length > 0) return false;
                if (Exceptions?.Length > 0) return false;
                return true;
            }
        }

        public string[] ErrorMessages { get; private set; }
        public Exception[] Exceptions { get; private set; }

        public object Data { get; private set; }

        public static CodeRunResult SuccessResult(object data = null) {
            return new CodeRunResult {Data = data};
        }

        public static CodeRunResult ErrorResult(string message, object data = null) {
            if (string.IsNullOrWhiteSpace(message)) return SuccessResult(data);
            var msgs = new[] {message};
            return new CodeRunResult {
                ErrorMessages = msgs,
                Data = data
            };
        }

        public static CodeRunResult ErrorResult(IEnumerable<string> messages, object data = null) {
            var msgs = messages?.ToArray();
            if (msgs == null || msgs.Length == 0) return SuccessResult(data);

            return new CodeRunResult {
                ErrorMessages = msgs,
                Data = data
            };
        }

        public static CodeRunResult ErrorResult(Exception exception, object data = null) {
            if (exception == null) return SuccessResult(data);
            var excs = new[] {exception};

            return new CodeRunResult {
                Exceptions = excs,
                Data = data
            };
        }

        public static CodeRunResult ErrorResult(IEnumerable<Exception> exceptions, object data = null) {
            var excs = exceptions?.ToArray();
            if (excs == null || excs.Length == 0) return SuccessResult(data);

            return new CodeRunResult {
                Exceptions = excs,
                Data = data
            };
        }

        public static CodeRunResult ErrorResult(IEnumerable<string> messages, IEnumerable<Exception> exceptions,
            object data = null) {
            var msgs = messages?.ToArray();
            var excs = exceptions?.ToArray();

            var cr = new CodeRunResult {
                Data = data
            };

            if (msgs != null && msgs.Length > 0) cr.ErrorMessages = msgs;
            if (excs != null && excs.Length > 0) cr.Exceptions = excs;

            return cr;
        }
    }
}