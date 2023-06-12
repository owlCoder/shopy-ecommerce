using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthUserController : ApiController
    {
        [HttpPost]
        [Route("Registracija")]
        public string Registracija([FromBody] KorisnikRegistracija zahtev)
        {
            if (!ModelState.IsValid)
            {
                return JsonConvert.SerializeObject("Podaci koje ste uneli u formi nisu validni!");
            }
            else
            {
                // uneti podaci su okej uneti - provera da li postoji korisnik sa unetim korisnickim imenom
                if (KorisniciStorage.ProveriPostojiUsername(zahtev.KorisnickoIme))
                {
                    // postoji vec, vratiti gresku
                    return JsonConvert.SerializeObject("Korisnik sa unetim korisničkim imenom postoji!");
                }
                else
                {
                    Korisnik novi = new Korisnik(zahtev.KorisnickoIme, zahtev.Lozinka, zahtev.Ime, zahtev.Prezime, zahtev.Pol, zahtev.Email, zahtev.DatumRodjenja);
                    novi.IsLoggedIn = true;
                    KorisniciStorage.Korisnici.Add(zahtev.KorisnickoIme, novi);
                    KorisniciStorage.AzurirajKorisnikeUBazi();

                    // auto login registrovanog korisnika - cuvanje po http sesiji
                    HttpContext.Current.Session["korisnik"] = novi;

                    return JsonConvert.SerializeObject("OK");
                }
            }
        }

        [HttpGet]
        [Route("Ulogovan")]
        public string Ulogovan()
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["korisnik"];

            if (korisnik != null && korisnik.IsLoggedIn)
                return JsonConvert.SerializeObject("OK");
            else
                return JsonConvert.SerializeObject("Nije ulogovan!");

        }
    }
}