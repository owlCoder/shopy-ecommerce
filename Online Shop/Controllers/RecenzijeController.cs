﻿using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Models.Requests;
using Online_Shop.Storage;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/reviews")]
    public class RecenzijeController : ApiController
    {
        // Metoda koja vraca sve recenzije za dati proizvod
        [HttpPost]
        [Route("RecenzijeZaProizvodPoId")]
        public string RecenzijePoProizvodu(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (!ModelState.IsValid || int.TryParse(zahtev.Id, out int id))
            {
                return JsonConvert.SerializeObject(new List<Recenzija>());
            }
            else
            {
                return JsonConvert.SerializeObject(RecenzijeStorage.RecenzijePoProizvodu(id));
            }
        }

        // Metoda koja vraca sve recenzije od datog korisnika po korisnickom imenu
        // stranica za kupce, moje recenzije
        [HttpPost]
        [Route("RecenzijePoKupcu")]
        public string RecenzijeKupac(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (!ModelState.IsValid)
            {
                return JsonConvert.SerializeObject(new List<Recenzija>());
            }
            else
            {
                return JsonConvert.SerializeObject(RecenzijeStorage.RecenzijePoKorisniku(zahtev.Id));
            }
        }

        // Metoda za dodavanje porudzbine
        [HttpPost]
        [Route("DodavanjeRecenzije")]
        public string DodajRecenziju(PorudzbinaAddRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            // recenziju moze da dodaje samo kupac


        }
    }
}
