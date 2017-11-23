using System;

namespace JrzAsp.Lib.CodeRunner {
    /// <summary>
    ///     Run code and handle exceptions and return the code running result
    /// </summary>
    public static class CodeRunnerService {
        public static CodeRunResult RunCode(Action code) {
            try {
                code();
                return CodeRunResult.SuccessResult();
            } catch (Exception exc) {
                return CodeRunResult.ErrorResult(exc);
            }
        }

        public static CodeRunResult RunCode(Action code, Action<Exception> onCodeError) {
            try {
                code();
                return CodeRunResult.SuccessResult();
            } catch (Exception exc) {
                onCodeError(exc);
                return CodeRunResult.ErrorResult(exc);
            }
        }

        public static CodeRunResult RunCode<TErrorResult>(Action code, Func<Exception, TErrorResult> onCodeError) {
            try {
                code();
                return CodeRunResult.SuccessResult();
            } catch (Exception exc) {
                var errResult = onCodeError(exc);
                return CodeRunResult.ErrorResult(exc, errResult);
            }
        }

        public static CodeRunResult RunCode<TCodeResult>(Func<TCodeResult> code) {
            try {
                var result = code();
                return CodeRunResult.SuccessResult(result);
            } catch (Exception exc) {
                return CodeRunResult.ErrorResult(exc);
            }
        }

        public static CodeRunResult RunCode<TCodeResult>(Func<TCodeResult> code, Action<Exception> onCodeError) {
            try {
                var result = code();
                return CodeRunResult.SuccessResult(result);
            } catch (Exception exc) {
                onCodeError(exc);
                return CodeRunResult.ErrorResult(exc);
            }
        }

        public static CodeRunResult RunCode<TCodeResult, TErrorResult>(Func<TCodeResult> code,
            Func<Exception, TErrorResult> onCodeError) {
            try {
                var result = code();
                return CodeRunResult.SuccessResult(result);
            } catch (Exception exc) {
                var errResult = onCodeError(exc);
                return CodeRunResult.ErrorResult(exc, errResult);
            }
        }
    }
}