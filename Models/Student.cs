using System;
using System.Collections.Generic;

namespace WebProjekat.Models
{
    public class Student
    {
        public string KorisnickoIme { get; set; }
        public string BrojIndeksa { get; set; }
        public string Sifra { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string Email { get; set; }
        public List<Ispit> PrijavljeniIspiti { get; set; } = new List<Ispit>();
        public List<Ispit> PolozeniIspiti { get; set; } = new List<Ispit>();
        public List<Ispit> NepolozeniIspiti { get; set; } = new List<Ispit>();

        public Student(string korisnickoIme, string brojIndeksa, string sifra, string ime, string prezime,
                       DateTime datumRodjenja, string email, List<Ispit> prijavljeniIspiti, List<Ispit> polozeniIspiti,
                       List<Ispit> nepolozeniIspiti)
        {
            KorisnickoIme = korisnickoIme;
            BrojIndeksa = brojIndeksa;
            Sifra = sifra;
            Ime = ime;
            Prezime = prezime;
            DatumRodjenja = datumRodjenja;
            Email = email;
            PrijavljeniIspiti = prijavljeniIspiti;
            PolozeniIspiti = polozeniIspiti;
            NepolozeniIspiti = nepolozeniIspiti;
        }

        public Student() { }
    }

}