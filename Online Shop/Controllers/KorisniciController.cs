using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
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

        // Metoda za registraciju novog prodavca
        [HttpPost]
        [Route("RegistracijaProdavca")]
        public string Registracija(KorisnikRegistracija zahtev)
        {
            if (!ModelState.IsValid)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 1, Poruka = "Podaci koje ste uneli u formi nisu validni!" });
            }
            else
            {
                // uneti podaci su okej uneti - provera da li postoji korisnik sa unetim korisnickim imenom
                if (KorisniciStorage.ProveriPostojiUsername(zahtev.KorisnickoIme))
                {
                    // postoji vec, vratiti gresku
                    return JsonConvert.SerializeObject(new Response { Kod = 2, Poruka = "Korisnik sa unetim korisničkim imenom postoji!" });
                }
                else
                {
                    var sifra = new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(zahtev.Lozinka));
                    var sha1 = new ASCIIEncoding().GetString(sifra);

                    Korisnik novi = new Korisnik(zahtev.KorisnickoIme, sha1, zahtev.Ime, zahtev.Prezime, zahtev.Pol, zahtev.Email, zahtev.DatumRodjenja, ULOGA.Prodavac, null, null, new List<Proizvod>(), false, false);
                    novi.IsLoggedIn = true;
                    KorisniciStorage.Korisnici.Add(novi);
                    KorisniciStorage.AzurirajKorisnikeUBazi();

                    return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
                }
            }
        }

        // Metoda za pribavljanje konkretnog korisnika
        [HttpPost]
        [Route("GetKorisnikById")]
        public string KorisnikPoId(SingleIdRequest id)
        {
            Korisnik k = KorisniciStorage.KorisnikPoId(id.Id);

            if(k == null)
            {
                return JsonConvert.SerializeObject(new AuthKorisnik { KorisnickoIme = ""});
            }
            else
            {
                return JsonConvert.SerializeObject(k);
            }
        }

        // Metoda za azuriranje profila
        [HttpPost]
        [Route("AzuriranjeProfilaKorisnika")]
        public string AzuriranjeProfila(KorisnikIzmenaProfila zahtev)
        {
            if (!ModelState.IsValid)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 1, Poruka = "Niste popunili pravilno formu!" });
            }
            else
            {
                string staro_korisnicko_ime = zahtev.Id;
                Korisnik korisnik = KorisniciStorage.KorisnikPoId(zahtev.Id);
                bool provera;

                // korisnik nije hteo da menja korisnicko ime - sto je i okej
                if (staro_korisnicko_ime.Equals(zahtev.KorisnickoIme))
                {
                    provera = true;
                }
                else
                {
                    // nisu ista korisnicka imena - provera da li je korisnicko ime zauzeto
                    provera = !KorisniciStorage.ProveriPostojiUsername(zahtev.KorisnickoIme);
                }

                // uneti podaci su okej uneti - provera da ne postoji korisnik sa unetim korisnickim imenom
                if (korisnik != null && provera)
                {
                    Korisnik tren = korisnik;

                    if (tren != null)
                    {
                        tren.KorisnickoIme = zahtev.KorisnickoIme;
                        tren.Ime = zahtev.Ime;
                        tren.Prezime = zahtev.Prezime;
                        tren.Pol = zahtev.Pol;
                        tren.Email = zahtev.Email;
                        tren.DatumRodjenja = zahtev.DatumRodjenja;

                        // azuriranje "baze podataka"
                        KorisniciStorage.AzurirajKorisnikeUBazi();

                        return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new Response { Kod = 7, Poruka = "Korisnik je izbrisan iz baze podataka!" });
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new Response { Kod = 8, Poruka = "Korisničko ime je zauzeto!" });
                }
            }
        }

        // Metoda za sortiranje korisnika
        [HttpPost]
        [Route("SortiranjeKorisnika")]
        public string SortirajKorisnike(SingleIdRequest id)
        {
            return JsonConvert.SerializeObject(KorisniciStorage.GetSorterd(id.Id));
        }
    }
}
