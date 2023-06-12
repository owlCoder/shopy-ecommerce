using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Shop.Models
{
    public class AuthKorisnik
    {
        public string KorisnickoIme { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Pol { get; set; }
        public string Email { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public ULOGA Uloga { get; set; }
        public List<Porudzbina> Porudzbine { get; set; }
        public List<Proizvod> OmiljenjiProizvodi { get; set; }
        public List<Proizvod> ObjavljeniProizvodi { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}