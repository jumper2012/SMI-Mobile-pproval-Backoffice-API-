using System.Web.Mvc;

namespace JrzAsp.Lib.CodeRunner.MvcHelpers {
    public static class ControllerModelStateErrorExtensions {

        public static void AddModelStateError(this Controller ctrl, CodeRunResult crr) {
            ctrl.ModelState.AddModelError(crr);
        }

        public static void AddModelError(this ModelStateDictionary modelState, CodeRunResult crr) {
            if (crr == null) return;
            foreach (var err in crr.ErrorMessages) modelState.AddModelError("", err);
            foreach (var err in crr.Exceptions) {
                modelState.AddModelError("", err);
                modelState.AddModelError("", err.Message);
            }
        }
    }
}