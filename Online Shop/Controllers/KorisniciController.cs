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
    [RoutePrefix("api/users")]
    public class KorisniciController : ApiController
    {
        // Kao povratnu vrednost vrati listu svih korisnika koji nisu obrisani
        [HttpGet]
        [Route("ListaKorisnika")]
        public string ListaKorisnika()
        {
            return JsonConvert.SerializeObject(KorisniciStorage.GetKorisnici());
        }

        // Metoda za brisanje korisnika
        [HttpPost]
        [Route("BrisanjeKorisnika")]
        public string ObrisiKorisnika(string id)
        {
            return JsonConvert.SerializeObject(id);
        }
    }
}
