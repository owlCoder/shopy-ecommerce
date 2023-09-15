using System;
using System.Collections.Generic;

namespace WebProjekat.Models
{
    public class Profesor
    {
        public string KorisnickoIme { get; set; }
        public string Sifra { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string Email { get; set; }
        public List<string> Predmeti { get; set; }
        public List<Ispit> Ispiti { get; set; } = new List<Ispit>();

        public Profesor(string korisnickoIme, string sifra, string ime, string prezime, DateTime datumRodjenja,
                        string email, List<string> predmeti, List<Ispit> ispiti)
        {
            KorisnickoIme = korisnickoIme;
            Sifra = sifra;
            Ime = ime;
            Prezime = prezime;
            DatumRodjenja = datumRodjenja;
            Email = email;
            Predmeti = predmeti;
            Ispiti = ispiti;
        }
        public Profesor() { }
    }
}