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
        public string ObrisiKorisnika(SingleIdRequest id)
        {
            if(KorisniciStorage.LogickoBrisanje(id.Id))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Korisnik sa ID '" + id.Id + "' uspešno obrisan!" });
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 9, Poruka = "Došlo je do greške prilikom brisanja!" });
            }
        }
        
        // Metoda za pretragu korisnika
        [HttpPost]
        [Route("PretragaKorisnika")]
        public string PretragaKorisnika(SearchRequest zahtev)
        {
            if(ModelState.IsValid)
            {
                int uloga;
                if (zahtev.Uloga.Equals("Kupac"))
                    uloga = 0;
                else if (zahtev.Uloga.Equals("Prodavac"))
                    uloga = 1;
                else
                    uloga = -1;

                // pretraga po kriterijumu
                return JsonConvert.SerializeObject(KorisniciStorage.PretragaKorisnika(zahtev, uloga));
            }
            else
            {
                return JsonConvert.SerializeObject(new List<AuthKorisnik>());
            }
        }
    }
}
