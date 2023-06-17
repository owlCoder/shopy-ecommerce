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

        public static object GetDostupniProizvodi()
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
    }
}