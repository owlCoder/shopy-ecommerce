using Online_Shop.Storage;
using System;

namespace Online_Shop.Models
{
    public class Porudzbina
    {
        public int Id { get; set; }
        public Proizvod Proizvod { get; set; }
        public double Kolicina { get; set; }
        public Korisnik Kupac { get; set; }
        public DateTime DatumPorudzbine { get; set; }
        public STATUS Status { get; set; }
        public bool IsDeleted { get; set; }

        public Porudzbina() 
        {
            // Prazan konstruktor
        }

        // Konstruktor za json serijalizator
        public Porudzbina(int id, Proizvod proizvod, double kolicina, Korisnik kupac, DateTime datumPorudzbine, STATUS status, bool isDeleted)
        {
            Id = id;
            Proizvod = proizvod;
            Kolicina = kolicina;
            Kupac = kupac;
            DatumPorudzbine = datumPorudzbine;
            Status = status;
            IsDeleted = isDeleted;
        }

        // Konstruktor za kreiranje nove porudzbine
        public Porudzbina(Proizvod proizvod, double kolicina, Korisnik kupac)
        {
            Id = PorudzbineStorage.Porudzbine.Count + 1;
            Proizvod = proizvod;
            Kolicina = kolicina;
            Kupac = kupac;
            DatumPorudzbine = DateTime.Now;
            Status = STATUS.AKTIVNA;
            IsDeleted = false;
        }
    }
}