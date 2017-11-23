using System.Web.Mvc;
using JrzAsp.Lib.FlashSession;
using JrzAsp.Models;

namespace JrzAsp.Controllers {
    public class CaptchaFieldTestController : Controller {
        [HttpGet]
        public ActionResult Index() {
            var mdl = new CaptchaFieldTestModel();
            return View(mdl);
        }

        [HttpPost]
        public ActionResult Index(CaptchaFieldTestModel mdl) {
            if (ModelState.IsValid) {
                FlashSessionService.AddMessage("Captcha is Valid!");
                return RedirectToAction("Index");
            }
            return View(mdl);
        }
    }
}