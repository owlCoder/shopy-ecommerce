using System.Collections.Generic;
using System;

namespace Online_Shop.Models
{
    public class Proizvod
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public double Kolicina { get; set; }
        public string Opis { get; set; }
        public string Slika { get; set; }
        public DateTime DatumPostavljanjaProizvoda { get; set; }
        public string Grad { get; set; }
        public List<Recenzija> Recenzija { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}