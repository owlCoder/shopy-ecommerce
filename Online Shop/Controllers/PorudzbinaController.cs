using Online_Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/orders")]
    public class PorudzbinaController : ApiController
    {
        [HttpPost]
        [Route("KreiranjePorudzbine")]
        public string KreirajPorudzbinu(SingleIdRequest zahtev)
        {
            // prima se Id proizvoda koji se narucuje kao i kolicina koja je trazena
        }
    }
}
