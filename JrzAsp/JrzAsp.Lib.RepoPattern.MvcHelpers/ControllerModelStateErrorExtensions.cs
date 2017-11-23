using System.Web.Mvc;

namespace JrzAsp.Lib.RepoPattern.MvcHelpers {
    public static class ControllerModelStateErrorExtensions {
        public static void AddModelStateError(this Controller ctrl, FurtherValidationResult furValRes) {
            ctrl.ModelState.AddModelError(furValRes);
        }

        public static void AddModelError(this ModelStateDictionary modelState, FurtherValidationResult furValRes) {
            if (furValRes == null) return;
            foreach (var field in furValRes.Errors.Keys)
            foreach (var err in furValRes.Errors[field]) modelState.AddModelError(field, err);
        }
    }
}