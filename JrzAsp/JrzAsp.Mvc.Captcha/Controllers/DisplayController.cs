using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using JrzAsp.Mvc.Captcha.Services;

namespace JrzAsp.Mvc.Captcha.Controllers {
    public class DisplayController : Controller {
        public ICaptchaService CapSvc => ModuleDependencyResolver.GetCaptchaService();

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public FileStreamResult Index(string id = null) {
            var captcha = CapSvc.GetCaptcha(id, true);
            var imageStream = new MemoryStream();
            captcha.ImageData.Save(imageStream, ImageFormat.Png);
            imageStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(imageStream, "image/png");
        }
    }
}