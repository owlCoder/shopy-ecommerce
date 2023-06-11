using System.Web.Http;
using System.Web.Http.Results;

namespace Online_Shop.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet, Route("")]
        public RedirectResult Index()
        {
            return Redirect(Request.RequestUri.AbsoluteUri + "index.html");
        }
    }
}
