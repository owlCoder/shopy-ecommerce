﻿using Newtonsoft.Json;
using Online_Shop.Storage;
using System;
using System.Collections.Generic;

namespace Online_Shop.Models
{
    public class Proizvod
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public double Cena { get; set; }
        private double kolicina;
        public string Opis { get; set; }
        public string Slika { get; set; }
        public DateTime DatumPostavljanjaProizvoda { get; set; }
        public string Grad { get; set; }
        public List<Recenzija> Recenzija { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public List<string> FID { get; set; } = new List<string>();
        public string KID { get; set; }
        public List<int> PID { get; set; } = new List<int>();
        public List<int> RID { get; set; } = new List<int>();
        public double Kolicina { get { return kolicina; } set { if (kolicina != value) { kolicina = value; Status = kolicina > 0.0; } } }

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
        [JsonConstructor]
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
            Recenzija = new List<Recenzija>();
            Status = status;
            IsDeleted = isDeleted;
        }
    }
}