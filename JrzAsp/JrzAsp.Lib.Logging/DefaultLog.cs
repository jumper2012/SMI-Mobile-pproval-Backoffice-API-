using System;

namespace JrzAsp.Lib.Logging {
    internal class DefaultLog : ILog {
        private readonly log4net.ILog _realLog;

        public DefaultLog(log4net.ILog realLog) {
            _realLog = realLog;
        }

        public void Debug(object message) {
            _realLog.Debug(message);
        }

        public void Debug(object message, Exception exception) {
            _realLog.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args) {
            _realLog.DebugFormat(format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args) {
            _realLog.DebugFormat(provider, format, args);
        }

        public void Info(object message) {
            _realLog.Info(message);
        }

        public void Info(object message, Exception exception) {
            _realLog.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args) {
            _realLog.InfoFormat(format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args) {
            _realLog.InfoFormat(provider, format, args);
        }

        public void Warn(object message) {
            _realLog.Warn(message);
        }

        public void Warn(object message, Exception exception) {
            _realLog.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args) {
            _realLog.WarnFormat(format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args) {
            _realLog.WarnFormat(provider, format, args);
        }

        public void Error(object message) {
            _realLog.Error(message);
        }

        public void Error(object message, Exception exception) {
            _realLog.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args) {
            _realLog.ErrorFormat(format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {
            _realLog.ErrorFormat(provider, format, args);
        }

        public void Fatal(object message) {
            _realLog.Fatal(message);
        }

        public void Fatal(object message, Exception exception) {
            _realLog.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args) {
            _realLog.FatalFormat(format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args) {
            _realLog.FatalFormat(provider, format, args);
        }
    }
}