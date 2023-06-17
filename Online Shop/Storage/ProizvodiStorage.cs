using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Online_Shop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Online_Shop.Storage
{
    public class ProizvodiStorage
    {
        public static List<Proizvod> Proizvodi { get; set; }
        public static string ProizvodiPath = HostingEnvironment.MapPath(@"~/App_Data/proizvodi.json");

        public ProizvodiStorage() 
        {
            // Prazan konstruktor - zbog serijalizacije
        }

        // Metoda koja iscitava sve proizvode iz json datoteke
        public static void UcitajProizvode()
        {
            // ucitavanje proizvoda iz datoteke - ako ona postoji
            if (File.Exists(ProizvodiPath))
            {
                try
                {
                    Proizvodi = new List<Proizvod>();

                    // ucitavanje svih proizvoda iz json datoteke
                    Proizvodi = JsonConvert.DeserializeObject<List<Proizvod>>(File.ReadAllText(ProizvodiPath));

                    // azuriranje status proizvoda nakon citanja
                    foreach (Proizvod p in Proizvodi)
                    {
                        if (p.Kolicina <= 0.0)
                        {
                            p.Status = false; // nije dostupan
                        }
                        else
                        {
                            p.Status = true; // na stanju je vise od jednog proizvoda
                        }
                    }
                }
                catch
                {
                    // ne postoji datoteka - pa se kreira prazna kolekcija
                    Proizvodi = new List<Proizvod>();
                }
            }
            else
            {
                // ne postoji datoteka - pa se kreira prazna kolekcija
                Proizvodi = new List<Proizvod>();
            }
        }

        // Metoda za azuriranje proizvoda u json datoteci
        public static void AzurirajProizvodeUBazi()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Proizvodi);
                File.WriteAllText(ProizvodiPath, json);
            }
            catch { }
        }

        // Metoda koja vraca trazeni proizvod po Id
        public Proizvod GetProizvodById(int id)
        {
            return Proizvodi.FirstOrDefault(p => p.Id == id);
        }

        // Metoda za dodavanje proizvoda
        public static bool DodajProizvod(string naziv, double cena, double kolicina, string opis, string slika, string grad)
        {
            // Proizvod treba dodati i u listu proizvoda prodavca i u globalnu listu proizvoda
            // string slika je sada PUTANJA SLIKE NA SERVERU
            Proizvod novi = new Proizvod(naziv, cena, kolicina, opis, slika, grad);

            Korisnik trenutni = (Korisnik)HttpContext.Current.Session["korisnik"];

            if(trenutni != null)
            {
                // Dodavanje proizvoda u listu
                Proizvodi.Add(novi);

                // Dodavanje proizvoda u listu objavljenih proizvoda
                if(trenutni.ObjavljeniProizvodi == null) trenutni.ObjavljeniProizvodi = new List<Proizvod>();

                trenutni.ObjavljeniProizvodi.Add(novi);

                // Azurirati json i za korisnike i za proizvode
                KorisniciStorage.AzurirajKorisnikeUBazi();
                AzurirajProizvodeUBazi();

                return true;
            }
            else
            {
                return false;
            }
        }

        // Metoda koja vraca sve proizvode koji pripadaju trenutno ulogovanom korisniku
        public static List<Proizvod> GetProizvodiPerUser()
        {
            Korisnik trenutni = (Korisnik)HttpContext.Current.Session["korisnik"];

            if(trenutni != null && trenutni.ObjavljeniProizvodi != null && trenutni.ObjavljeniProizvodi.Count > 0)
            {
                return trenutni.ObjavljeniProizvodi.FindAll(p => p.IsDeleted == false);
            }
            else
            {
                return new List<Proizvod>();
            }
        }

        // Metoda koja vraca sve dostupne proizvode za trenutno prijavljenog korisnika
        public static List<Proizvod> GetDostupniProizvodi()
        {
            Korisnik trenutni = (Korisnik)HttpContext.Current.Session["korisnik"];

            if (trenutni != null && trenutni.ObjavljeniProizvodi != null && trenutni.ObjavljeniProizvodi.Count > 0)
            {
                return trenutni.ObjavljeniProizvodi.FindAll(p => p.IsDeleted == false && p.Status);
            }
            else
            {
                return new List<Proizvod>();
            }
        }

        // Metoda koja dodatno sortira proizvode koji su dostupni/svi proizvodi
        public static List<Proizvod> SortirajPoKriterijumu(string kriterijum, List<Proizvod> proizvodi)
        {
            if (proizvodi == null || proizvodi.Count == 0)
            {
                return new List<Proizvod>();
            }
            else
            {
                // lista nije prazna, sortirani po kriterijumu
                // 1 - naziv asc, 2 - naziv desc, 3 - cena asc, 4 - cena desc, 5 - datum ogl asc, 6 - datum ogl desc
                if(kriterijum.Equals("1"))
                {
                    return proizvodi.OrderBy(p => p.Naziv).ToList();
                }
                else if (kriterijum.Equals("2"))
                {
                    return proizvodi.OrderByDescending(p => p.Naziv).ToList();
                }
                else if (kriterijum.Equals("3"))
                {
                    return proizvodi.OrderBy(p => p.Cena).ToList();
                }
                else if (kriterijum.Equals("4"))
                {
                    return proizvodi.OrderByDescending(p => p.Naziv).ToList();
                }
                else if (kriterijum.Equals("5"))
                {
                    return proizvodi.OrderBy(p => p.DatumPostavljanjaProizvoda).ToList();
                }
                else if (kriterijum.Equals("6"))
                {
                    return proizvodi.OrderByDescending(p => p.DatumPostavljanjaProizvoda).ToList();
                }
                else
                {
                    return proizvodi;
                }
            }
        }
    
        // Metoda koja brise proizvode iz kolekcije svih proizvoda, i listi proizvoda koji se nalaze listi korisnika
        public static bool DeleteProizvod(int id)
        {
            // Obrisi sve proizvode u bazi, kao i korisnike (prodavac koji je kreirao taj proizvod,
            // i kupce koji su taj proizvod dodali u porudzbine i/ili omiljeni)

            // prvo prolazimo kroz listu svih proizvoda i logicki brisemo proizvod
            List<Proizvod> za_brisanje = Proizvodi.FindAll(p => p.Id == id);

            foreach(Proizvod proizvod in za_brisanje)
            {
                if (proizvod.IsDeleted == false)
                {
                    // nije jos uvek obrisan
                    // obrisemo ga logicki
                    proizvod.IsDeleted = true;
                }
            }

            // Azuriranje u json fajlovima, novo izmenjenih entiteta
            AzurirajProizvodeUBazi();
            KorisniciStorage.AzurirajKorisnikeUBazi();

            return true;
        }
    }
}