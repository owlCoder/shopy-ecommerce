using System;

namespace WebProjekat.Models
{
    public class Administrator
    {
        public string KorisnickoIme { get; set; }
        public string Sifra { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRodjenja { get; set; }

        public Administrator(string korisnickoIme, string sifra, string ime, string prezime, DateTime datumRodjenja)
        {
            KorisnickoIme = korisnickoIme;
            Sifra = sifra;
            Ime = ime;
            Prezime = prezime;
            DatumRodjenja = datumRodjenja;
        }

        public Administrator() { }
    }

}