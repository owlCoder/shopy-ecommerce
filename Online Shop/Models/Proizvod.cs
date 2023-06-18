using System.Collections.Generic;
using System;
using Online_Shop.Storage;

namespace Online_Shop.Models
{
    public class Proizvod
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public double Kolicina { get { return Kolicina; } set { if(Kolicina != value) { Kolicina = value; Status = Kolicina > 0.0; } } }
        public string Opis { get; set; }
        public string Slika { get; set; }
        public DateTime DatumPostavljanjaProizvoda { get; set; }
        public string Grad { get; set; }
        public List<Recenzija> Recenzija { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }

        // Prazan konstruktor zbog serijalizacije
        public Proizvod() 
        {
            
        }

        // Kreiranje tek dodatog proizvoda
        public Proizvod(string naziv, double cena, double kolicina, string opis, string slika, string grad)
        {
            Id = ProizvodiStorage.Proizvodi.Count + 1;
            Naziv = naziv;
            Cena = cena;
            Kolicina = kolicina;
            Opis = opis;
            Slika = slika;
            DatumPostavljanjaProizvoda = DateTime.Now;
            Grad = grad;
            Recenzija = new List<Recenzija>();
            Status = (kolicina > 0.0 ? true : false);
            IsDeleted = false;
        }

        // Konstruktor za ucitavanje iz json fajla
        public Proizvod(int id, string naziv, double cena, double kolicina, string opis, string slika, DateTime datumPostavljanjaProizvoda, string grad, List<Recenzija> recenzija, bool status, bool isDeleted)
        {
            Id = id;
            Naziv = naziv;
            Cena = cena;
            Kolicina = kolicina;
            Opis = opis;
            Slika = slika;
            DatumPostavljanjaProizvoda = datumPostavljanjaProizvoda;
            Grad = grad;
            Recenzija = recenzija;
            Status = status;
            IsDeleted = isDeleted;
        }
    }
}