using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PR55_2020_Danijel_Jovanovic.Models
{
    public class Korisnik
    {
        public string KorisnickoIme { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Pol { get; set; }
        public string Email { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public ULOGA Uloga { get; set; }
        public List<Porudzbina> Porudzbine { get; set; }
        public List<Proizvod> OmiljenjiProizvodi { get; set; }
        public List<Proizvod> ObjavljeniProizvodi { get; set; }


    }
}