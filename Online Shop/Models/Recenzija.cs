using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Shop.Models
{
    public class Recenzija
    {
        public Proizvod Proizvod { get; set; }
        public Korisnik Recenzent { get; set; }
        public string Naslov { get; set; }
        public string SadrzajRecenzije { get; set; }
        public string Slika { get; set; }
        public bool Odobrena { get; set; }
        public bool IsDeleted { get; set; }

    }
}