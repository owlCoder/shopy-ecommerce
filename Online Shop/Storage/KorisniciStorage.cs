using Online_Shop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Hosting;

namespace Online_Shop.Storage
{
    public class KorisniciStorage
    {
        public static Dictionary<string, Korisnik> Korisnici { get; set; }
        public static string KorisniciPath = HostingEnvironment.MapPath(@"~/App_Data/korisnici.json");

        public KorisniciStorage()
        {
            // Prazan konstruktor
        }

        public static void UcitajKorisnike()
        {
            // ucitavanje korisnika iz datoteke - ako ona postoji
            if (File.Exists(KorisniciPath))
            {
                try
                {
                    // ucitavanje svih korisnika iz json datoteke
                    Korisnici = JsonConvert.DeserializeObject<Dictionary<string, Korisnik>>(File.ReadAllText(KorisniciPath));

                    // izloguj sve korisnike - server se restartovao
                    foreach (Korisnik k in Korisnici.Values)
                    {
                        if (k.IsLoggedIn)
                        {
                            k.IsLoggedIn = false;
                        }
                    }
                }
                catch 
                {
                    // ne postoji datoteka - pa se kreira prazan recnik
                    Korisnici = new Dictionary<string, Korisnik>();
                }
            }
            else
            {
                // ne postoji datoteka - pa se kreira prazan recnik
                Korisnici = new Dictionary<string, Korisnik>();
            }
        }

        // Metoda koja proverava da li postoji korisnik sa trazenim username
        public static bool ProveriPostojiUsername(string username)
        {
            // ako postoji i nije obrisan
            return (Korisnici.TryGetValue(username, out Korisnik pronadjen) && !pronadjen.IsDeleted);
        }

        // Dobavi referencu korisnika iz baze podataka
        public static Korisnik GetKorisnik(string username)
        {
            return Korisnici.TryGetValue(username, out Korisnik korisnik) ? korisnik : null;
        }

        // Dodaje novog korisnika i cuva u json nakon uspesne registracije
        public static void AzurirajKorisnikeUBazi()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Korisnici);
                File.WriteAllText(KorisniciPath, json);
            }
            catch { }
        }

        // Iz recnika svih korisnika vraca listu korisnika koji nisu obrisani
        public static List<AuthKorisnik> GetKorisnici()
        {
            List<AuthKorisnik> korisnici = new List<AuthKorisnik>();
            string trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]).KorisnickoIme;

            foreach(Korisnik k in Korisnici.Values)
            {
                // ne prikazuju se administratori, niti LOGICKI obrisani korisnici niti trenutno ulogovan korisnik
                if(k.IsDeleted == false && k.Uloga != ULOGA.Administrator && !k.KorisnickoIme.Equals(trenutni))
                {
                    AuthKorisnik ak = new AuthKorisnik
                    {
                        KorisnickoIme = k.KorisnickoIme,
                        Ime = k.Ime,
                        Prezime = k.Prezime,
                        Pol = k.Pol,
                        Email = k.Email,
                        DatumRodjenja = k.DatumRodjenja,
                        Uloga = k.Uloga,
                    };

                    korisnici.Add(ak);
                }    
            }

            return korisnici;
        }
    }
}