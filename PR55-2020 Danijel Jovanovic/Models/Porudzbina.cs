using System;

namespace PR55_2020_Danijel_Jovanovic.Models
{
    public class Porudzbina
    {
        public Proizvod Proizvod { get; set; }
        public double Kolicina { get; set; }
        public Korisnik Kupac { get; set; }
        public DateTime DatumPorudzbine { get; set; }
        public STATUS Status { get; set; }
    }
}