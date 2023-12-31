﻿using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Storage;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/products")]
    public class ProizvodiController : ApiController
    {
        // Metoda za dodavanje proizvoda
        [HttpPost]
        [Route("DodavanjeProizvoda")]
        public string DodajProizvod(ProizvodAddRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Prodavac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (ModelState.IsValid)
            {
                bool uspesno = ProizvodiStorage.DodajProizvod(zahtev.Naziv, zahtev.Cena, zahtev.Kolicina, zahtev.Opis, zahtev.Slika, zahtev.Grad);

                if (uspesno)
                    return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
                else
                    return JsonConvert.SerializeObject(new Response { Kod = 15, Poruka = "Došlo je do greške prilikom dodavanja proizvoda!" });
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Uneti podaci nisu validni!" });
            }
        }

        // Metoda za brisanje proizvoda
        // proizvodi koji nisu dostupni (povuceni su) ne mogu biti obrisani niti izmenjeni
        [HttpPost]
        [Route("BrisanjeProizvoda")]
        public string ObrisiProizvod(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || (trenutni.Uloga != ULOGA.Administrator && trenutni.Uloga != ULOGA.Prodavac))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (int.TryParse(zahtev.Id, out int idp) && ProizvodiStorage.DeleteProizvod(idp))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Proizvod uspešno obrisan iz liste proizvoda." });
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 13, Poruka = "Proizvod nije moguće uspešno obrisati iz liste svih proizvoda. Proverite da li je proizvod na stanju!" });
            }
        }

        // Metoda za azuriranje proizvoda
        // propagacija izmene se odvija svuda gde se proizvod nalazi
        // lista svih proizvoda, liste omiljenih proizvoda kod kupaca, lista aktivnih porudzbina kod kupaca
        // lista 
        [HttpPost]
        [Route("AzuriranjeProizvoda")]
        public string AzurirajProizvod(ProizvodEditRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || (trenutni.Uloga != ULOGA.Administrator && trenutni.Uloga != ULOGA.Prodavac))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (ModelState.IsValid)
            {
                bool uspesno = ProizvodiStorage.AzuriranjeProizvoda(zahtev.Id, zahtev.Naziv, zahtev.Cena, zahtev.Kolicina, zahtev.Opis, zahtev.Slika, zahtev.Grad);

                if (uspesno)
                    return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
                else
                    return JsonConvert.SerializeObject(new Response { Kod = 15, Poruka = "Došlo je do greške prilikom ažuriranja proizvoda!" });
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Uneti podaci nisu validni!" });
            }
        }

        // Metoda za prikazivanje svih proizvoda
        [HttpGet]
        [Route("ListaProizvoda")]
        public string PrikazObjavljenihProizvoda()
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Prodavac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            return JsonConvert.SerializeObject(ProizvodiStorage.GetProizvodiPerUser());
        }

        // Metoda za prikaz dostupnih proizvoda i filtriranje po korisniku
        [HttpPost]
        [Route("ListaFiltriranihProizvoda")]
        public string PrikazDostupnihProizvoda(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Prodavac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            string[] request = zahtev.Id.Split(';');
            List<Proizvod> proizvodi;

            if (request[0].Equals("0"))
            {
                proizvodi = ProizvodiStorage.GetProizvodiPerUser();
            }
            else
            {
                proizvodi = ProizvodiStorage.GetDostupniProizvodi();
            }

            // filtiranje po drugom kriterijumu - ako nije 0 - podrazumevano
            if (request[1].Equals("0") || request[1].Equals(""))
            {
                return JsonConvert.SerializeObject(proizvodi);
            }
            else
            {
                proizvodi = ProizvodiStorage.SortirajPoKriterijumu(request[1], proizvodi);
                return JsonConvert.SerializeObject(proizvodi);
            }
        }

        // Metoda za prikaz dostupnih proizvoda i filtriranje u CELOM SISTEMU za POCETNU STRANICU
        [HttpPost]
        [Route("ListaFiltriranihProizvodaPocetna")]
        public string PrikazSvihDostupnihProizvoda(FilterProductRequest zahtev)
        {
            if (ModelState.IsValid)
            {
                List<Proizvod> proizvodi = ProizvodiStorage.Proizvodi.FindAll(p => p.IsDeleted == false && p.Status == true);

                if (!zahtev.Naziv.Equals("")) // unet je kriterijum za naziv
                {
                    proizvodi = proizvodi.FindAll(p => p.Naziv.ToLower().Contains(zahtev.Naziv.ToLower()));
                }

                if (!zahtev.Grad.Equals("")) // unet je kriterijum za grad
                {
                    proizvodi = proizvodi.FindAll(p => p.Grad.ToLower().Contains(zahtev.Grad.ToLower()));
                }

                if (zahtev.MinCena != -1.0 && zahtev.MaxCena != -1.0)
                {
                    proizvodi = proizvodi.FindAll(p => p.Cena >= zahtev.MinCena && p.Cena <= zahtev.MaxCena);
                }

                // filtiranje po drugom kriterijumu - ako nije 0 - podrazumevano
                if (zahtev.Sortiranje.Equals("0") || zahtev.Sortiranje.Equals(""))
                {
                    return JsonConvert.SerializeObject(proizvodi);
                }
                else
                {
                    proizvodi = ProizvodiStorage.SortirajPoKriterijumu(zahtev.Sortiranje, proizvodi);
                    return JsonConvert.SerializeObject(proizvodi);
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new List<Proizvod>());
            }
        }

        // Metoda za prikaz dostupnih proizvoda i filtriranje u CELOM SISTEMU
        [HttpPost]
        [Route("ListaFiltriranihSvihProizvoda")]
        public string PrikazSvihDostupnihProizvoda(SingleIdRequest zahtev)
        {
            string[] request = zahtev.Id.Split(';');
            List<Proizvod> proizvodi;

            if (request[0].Equals("0"))
            {
                proizvodi = ProizvodiStorage.GetSviProizvodi();
            }
            else
            {
                proizvodi = ProizvodiStorage.GetDostupniSviProizvodi();
            }

            // filtiranje po drugom kriterijumu - ako nije 0 - podrazumevano
            if (request[1].Equals("0") || request[1].Equals(""))
            {
                return JsonConvert.SerializeObject(proizvodi);
            }
            else
            {
                proizvodi = ProizvodiStorage.SortirajPoKriterijumu(request[1], proizvodi);
                return JsonConvert.SerializeObject(proizvodi);
            }
        }

        // Metoda za prikaz svih proizvoda koji nisu obrisani i koji su dostupni
        [HttpGet]
        [Route("SviProizvodiPocetna")]
        public string PrikazSvihProizvoda()
        {
            return JsonConvert.SerializeObject(ProizvodiStorage.Proizvodi.FindAll(p => p.IsDeleted == false && p.Status == true));
        }

        [HttpGet]
        [Route("SviProizvodi")]
        public string PrikazSvihProizvodaAdministracija()
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Administrator)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            return JsonConvert.SerializeObject(ProizvodiStorage.Proizvodi.FindAll(p => p.IsDeleted == false));
        }

        // Metoda za pribvljanje informacija o proizvodu za izmenu
        [HttpPost]
        [Route("ProizvodPoId")]
        public string GetProizvodById(SingleIdRequest zahtev)
        {
            return JsonConvert.SerializeObject(ProizvodiStorage.GetProizvodPoId(zahtev.Id));
        }

        // Metoda za pribvljanje informacija o proizvodu za prikaz na Proizvod stranici
        [HttpPost]
        [Route("DostupanProizvodPoId")]
        public string GetProizvodByIdDostupan(SingleIdRequest zahtev)
        {
            return JsonConvert.SerializeObject(ProizvodiStorage.GetProizvodPoIdDostupan(zahtev.Id));
        }
    }
}
