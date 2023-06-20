using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Online_Shop.Models
{
    [JsonObject(IsReference = true)]
    public class Korisnik
    {
        public string KorisnickoIme { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Pol { get; set; }
        public string Email { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public ULOGA Uloga { get; set; }
        public List<Porudzbina> Porudzbine { get; set; }
        public List<Proizvod> OmiljenjiProizvodi { get; set; }
        public List<Proizvod> ObjavljeniProizvodi { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLoggedIn { get; set; }

        public Korisnik()
        {
            // inicijalno korisnik nije ulogovan
            IsLoggedIn = false;
        }

        // Konstruktor za ucitavanje podataka iz liste
        [JsonConstructor]
        public Korisnik(string korisnickoIme, string lozinka, string ime, string prezime, string pol, string email,
                        DateTime datumRodjenja, ULOGA uloga, List<Porudzbina> porudzbine, List<Proizvod> omiljenjiProizvodi,
                        List<Proizvod> objavljeniProizvodi, bool isDeleted, bool isLoggedIn)
        {
            KorisnickoIme = korisnickoIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Email = email;
            DatumRodjenja = datumRodjenja;
            Uloga = uloga;
            Porudzbine = null;
            OmiljenjiProizvodi = null;
            ObjavljeniProizvodi = null;
            IsDeleted = isDeleted;
            IsLoggedIn = isLoggedIn;
        }

        // Kreiranje kupca - nakon registracije
        public Korisnik(string korisnickoIme, string lozinka, string ime, string prezime, string pol, string email, DateTime datumRodjenja)
        {
            // podrazumevano nije obrisan, ima praznu listu porudzbina itd - tek se registrovao
            OmiljenjiProizvodi = new List<Proizvod>();
            Porudzbine = new List<Porudzbina>();
            IsDeleted = false;
            IsLoggedIn = true; // ulogovan je cim se registrovao

            // samo prodavac ima listu objavljenih proizvoda
            ObjavljeniProizvodi = null;

            // ostali podaci se podesavaju onima iz forme
            KorisnickoIme = korisnickoIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Email = email;
            DatumRodjenja = datumRodjenja;
            Uloga = ULOGA.Kupac; // podrazumevano je kupac
        }
    }
}