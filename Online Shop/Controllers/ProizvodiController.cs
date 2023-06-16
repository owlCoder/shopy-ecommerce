using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/products")]
    public class ProizvodiController : ApiController
    {
        [HttpPost]
        [Route("DodavanjeProizvoda")]
        public string DodajProizvod(ProizvodAddRequest zahtev)
        {
            if(ModelState.IsValid)
            {
                ProizvodiStorage.DodajProizvod(zahtev.Naziv, zahtev.Cena, zahtev.Kolicina, zahtev.Opis, zahtev.Slika, zahtev.Grad);
                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Uneti podaci nisu validni!" });
            }    
        }
    }
}
