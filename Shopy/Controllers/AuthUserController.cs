﻿using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Storage;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthUserController : ApiController
    {
        [HttpPost]
        [Route("Registracija")]
        public string Registracija(KorisnikRegistracija zahtev)
        {
            // Samo neprijavljeni korisnici se ne mogu registrovati
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni != null && trenutni.IsLoggedIn)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Već ste autentifikovani na platformi!" });
            }

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

                    Korisnik novi = new Korisnik(zahtev.KorisnickoIme, sha1, zahtev.Ime, zahtev.Prezime, zahtev.Pol, zahtev.Email, zahtev.DatumRodjenja);
                    novi.IsLoggedIn = true;
                    KorisniciStorage.Korisnici.Add(novi);
                    KorisniciStorage.AzurirajKorisnikeUBazi();

                    // auto login registrovanog korisnika - cuvanje po http sesiji
                    HttpContext.Current.Session["korisnik"] = novi;

                    return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
                }
            }
        }

        [HttpGet]
        [Route("Ulogovan")]
        public string Ulogovan()
            {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["korisnik"];

            if (korisnik != null && !korisnik.IsDeleted && korisnik.IsLoggedIn)
                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
            else
            {
                OdjavaKorisnika();
                return JsonConvert.SerializeObject(new Response { Kod = 3, Poruka = "Nije ulogovan!" });
            }
        }

        [HttpGet]
        [Route("AuthKorisnik")]
        public string TrenutnoUlogovanAuthPodaci()
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["korisnik"];

            if (korisnik != null && korisnik.IsLoggedIn)
            {
                AuthKorisnik novi = new AuthKorisnik
                {
                    KorisnickoIme = korisnik.KorisnickoIme,
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Pol = korisnik.Pol,
                    Email = korisnik.Email,
                    DatumRodjenja = korisnik.DatumRodjenja,
                    Uloga = korisnik.Uloga,
                    Porudzbine = korisnik.Porudzbine,
                    OmiljenjiProizvodi = korisnik.OmiljenjiProizvodi,
                    ObjavljeniProizvodi = korisnik.ObjavljeniProizvodi,
                    IsDeleted = korisnik.IsDeleted,
                    IsLoggedIn = korisnik.IsLoggedIn
                };

                // proveri kasnije zbog liste porudzbina!
                return JsonConvert.SerializeObject(novi);
            }
            else
                return JsonConvert.SerializeObject(new AuthKorisnik { KorisnickoIme = "" });
        }

        [HttpPost]
        [Route("Prijava")]
        public string PrijavaNaPlatformu(KorisnikLogin zahtev)
        {
            // Samo neprijavljeni korisnici se ne mogu prijaviti
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni != null && trenutni.IsLoggedIn)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Već ste autentifikovani na platformi!" });
            }

            if (!ModelState.IsValid)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 1, Poruka = "Pre prijave na platformu potrebno je da se registrujete!" });
            }
            else
            {
                // uneti podaci su okej uneti - provera da li postoji korisnik sa unetim korisnickim imenom
                if (KorisniciStorage.ProveriPostojiUsername(zahtev.KorisnickoIme))
                {
                    // proveri da li se lozinke poklapaju
                    var sifra = new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(zahtev.Lozinka));
                    var sha1 = new ASCIIEncoding().GetString(sifra);

                    Korisnik tren = KorisniciStorage.GetKorisnik(zahtev.KorisnickoIme);

                    if (tren.Lozinka.Equals(sha1))
                    {
                        tren.IsLoggedIn = true;
                        HttpContext.Current.Session["korisnik"] = tren;

                        // postoji i lozinka je okej
                        return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
                    }
                    else
                    {
                        // lozinka nije tacka
                        // postoji i lozinka je okej
                        return JsonConvert.SerializeObject(new Response { Kod = 6, Poruka = "Lozinka koju ste uneli nije validna!" });
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new Response { Kod = 5, Poruka = "Pre prijave na platformu potrebno je da se registrujete!" });
                }
            }
        }

        [HttpGet]
        [Route("Odjava")]
        public string OdjavaKorisnika()
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["korisnik"];

            if (korisnik != null && korisnik.IsLoggedIn)
            {
                // odjava korisnika sa sesije
                HttpContext.Current.Session["korisnik"] = null;

                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
            }
            else if(korisnik != null && !korisnik.IsLoggedIn && korisnik.IsDeleted)
            {
                // odjava korisnika sa sesije
                HttpContext.Current.Session["korisnik"] = null;

                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
            }
            else
                return JsonConvert.SerializeObject(new Response { Kod = 3, Poruka = "Nije ulogovan!" });
        }

        // Metoda za azuriranje korisnickog profila
        [HttpPost]
        [Route("AzuriranjeProfila")]
        public string AzuriranjeProfila(KorisnikRegistracija zahtev)
        {
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni != null && !trenutni.IsLoggedIn)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi!" });
            }

            if(zahtev.DatumRodjenja.Equals(new DateTime(0001, 1, 1)))
            {
                zahtev.DatumRodjenja = trenutni.DatumRodjenja;
                ModelState.Clear();
            }

            if (!ModelState.IsValid)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 1, Poruka = "Niste popunili pravilno formu!" });
            }
            else
            {
                Korisnik korisnik = (Korisnik)HttpContext.Current.Session["korisnik"];
                string staro_korisnicko_ime = korisnik.KorisnickoIme;
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
                    Korisnik tren = KorisniciStorage.GetKorisnik(korisnik.KorisnickoIme);

                    if (tren != null)
                    {
                        tren.KorisnickoIme = zahtev.KorisnickoIme;
                        tren.Ime = zahtev.Ime;
                        tren.Prezime = zahtev.Prezime;
                        tren.Pol = zahtev.Pol;
                        tren.Email = zahtev.Email;
                        tren.DatumRodjenja = zahtev.DatumRodjenja;

                        // Azuriranje korisnika
                        HttpContext.Current.Session["korisnik"] = tren;

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

        [HttpPost]
        [Route("AzuriranjeLozinke")]
        public string AzuriranjeLozinke(PromenaLozinke zahtev)
        {
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi!" });
            }

            if (!ModelState.IsValid)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 1, Poruka = "Niste popunili pravilno formu!" });
            }
            else
            {
                Korisnik korisnik = (Korisnik)HttpContext.Current.Session["korisnik"];

                // uneti podaci su okej uneti - provera da li postoji korisnik sa unetim korisnickim imenom
                if (korisnik != null && KorisniciStorage.ProveriPostojiUsername(korisnik.KorisnickoIme))
                {
                    Korisnik tren = KorisniciStorage.GetKorisnik(korisnik.KorisnickoIme);

                    // proveri da li se lozinke poklapaju
                    var stara_sifra = new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(zahtev.StaraLozinka));
                    var nova_sifra = new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(zahtev.NovaLozinka));
                    var sha1_stara = new ASCIIEncoding().GetString(stara_sifra);
                    var sha1_nova = new ASCIIEncoding().GetString(nova_sifra);

                    if (tren != null && tren.Lozinka.Equals(sha1_stara))
                    {
                        tren.Lozinka = sha1_nova;
                        KorisniciStorage.AzurirajKorisnikeUBazi();

                        return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new Response { Kod = 7, Poruka = "Stara lozinka nije tačna!" });
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new Response { Kod = 8, Poruka = "Potrebno je da se ponovo ulogujete!" });
                }
            }
        }
    }
}