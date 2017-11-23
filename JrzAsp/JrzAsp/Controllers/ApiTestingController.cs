using System.Collections.Generic;
using System.Web.Http;

namespace JrzAsp.Controllers {
    [RoutePrefix("api/testing")]
    public class ApiTestingController : ApiController {
        // GET api/<controller>
        [HttpGet]
        [Route("")]
        public IEnumerable<string> Get() {
            return new[] {"value1-BOO!", "value2-BOO!"};
        }
    }
}