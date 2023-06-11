using System;

namespace Online_Shop.Models
{
    public class Porudzbina
    {
        public Proizvod Proizvod { get; set; }
        public double Kolicina { get; set; }
        public Korisnik Kupac { get; set; }
        public DateTime DatumPorudzbine { get; set; }
        public STATUS Status { get; set; }

        public bool IsDeleted { get; set; }
    }
}