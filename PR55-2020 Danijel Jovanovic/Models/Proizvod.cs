using System;
using System.Collections.Generic;

namespace PR55_2020_Danijel_Jovanovic.Models
{
    public class Proizvod
    {
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public double Kolicina { get; set; }
        public string Opis { get; set; }
        public string Slika { get; set; }
        public DateTime DatumPostavljanjaProizvoda { get; set; }
        public string Grad { get; set; }
        public List<Recenzija> Recenzija { get; set; }
        public bool Status { get; set; }
    }
}