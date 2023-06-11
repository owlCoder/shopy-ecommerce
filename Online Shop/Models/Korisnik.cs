using System.Collections.Generic;
using System;

namespace Online_Shop.Models
{
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
            Porudzbine = porudzbine;
            OmiljenjiProizvodi = omiljenjiProizvodi;
            ObjavljeniProizvodi = objavljeniProizvodi;
            IsDeleted = isDeleted;
            IsLoggedIn = isLoggedIn;
        }
    }
}