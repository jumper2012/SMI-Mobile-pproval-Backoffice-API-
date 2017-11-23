using System;
using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ExceptionUtilities {
    public class MultiException : Exception {

        public MultiException(IEnumerable<string> messages) {
            Messages = messages?.ToArray() ?? new string[0];
        }

        public MultiException(IEnumerable<Exception> exceptions) {
            Exceptions = exceptions?.ToArray() ?? new Exception[0];
        }

        public MultiException(IEnumerable<string> messages, IEnumerable<Exception> exceptions,
            bool addExceptionsMessagesFirstToArray = false) : this(exceptions) {
            var msgs = messages?.ToList() ?? new List<string>();
            if (addExceptionsMessagesFirstToArray) {
                var msgsTmp = Exceptions.Select(e => e.Message).ToList();
                msgsTmp.AddRange(msgs);
                msgs = msgsTmp;
            }
            Messages = msgs.ToArray();
        }

        public string[] Messages { get; }
        public Exception[] Exceptions { get; }
    }
}