﻿using Newtonsoft.Json;
using Online_Shop.Models;
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

                        // kupci omiljeni
                        int korisnik;
                        foreach(string uname in p.FID)
                        {
                            korisnik = KorisniciStorage.Korisnici.IndexOf(KorisniciStorage.Korisnici.Find(x => x.KorisnickoIme.Equals(uname)));
                            if (korisnik != -1) KorisniciStorage.Korisnici[korisnik].OmiljenjiProizvodi.Add(Proizvodi[Proizvodi.FindIndex(x => x.Id == p.Id)]);
                        }

                        // prodavci oblavljeni
                        korisnik = KorisniciStorage.Korisnici.IndexOf(KorisniciStorage.Korisnici.Find(x => x.KorisnickoIme.Equals(p.KID)));
                        if (korisnik != -1) KorisniciStorage.Korisnici[korisnik].ObjavljeniProizvodi.Add(Proizvodi[Proizvodi.FindIndex(x => x.Id == p.Id)]);
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
                string json = JsonConvert.SerializeObject(Proizvodi, Formatting.Indented);
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

            if (trenutni != null)
            {
                novi.KID = trenutni.Id;
                // Dodavanje proizvoda u listu
                Proizvodi.Add(novi);

                // Dodavanje proizvoda u listu objavljenih proizvoda
                if (trenutni.ObjavljeniProizvodi == null) trenutni.ObjavljeniProizvodi = new List<Proizvod>();

                int index = KorisniciStorage.Korisnici.FindIndex(p => p.KorisnickoIme.Equals(trenutni.KorisnickoIme));
                if(index != -1)
                {
                    int pi = Proizvodi.FindIndex(p => p.Id == novi.Id);
                    int ki = KorisniciStorage.Korisnici.FindIndex(p => p.KorisnickoIme.Equals(trenutni.KorisnickoIme));
                    if(pi != -1 && ki != -1)
                        KorisniciStorage.Korisnici[ki].ObjavljeniProizvodi.Add(Proizvodi[pi]);
                }

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

            if (trenutni != null && trenutni.ObjavljeniProizvodi != null && trenutni.ObjavljeniProizvodi.Count > 0)
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
                if (kriterijum.Equals("1"))
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
                    return proizvodi.OrderByDescending(p => p.Cena).ToList();
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
            // proizvod nije obrisan
            List<Proizvod> za_brisanje = Proizvodi.FindAll(p => p.Id == id && p.IsDeleted == false);

            if (za_brisanje.Count == 0)
            {
                return false; // proizvod je vec obrisan, nema daljeg brisanja 
            }

            // da li proizvod brise prodavac, jer ako prodavac brise onda je potrebno proveriti i status proizvoda
            // specifikacija: proizvodi koji nisu dostupni ne mogu biti obrisani niti izmenjeni
            if (((Korisnik)HttpContext.Current.Session["korisnik"]).Uloga == ULOGA.Prodavac &&
                za_brisanje.FindAll(p => p.Status == false).Count > 0)
            {
                return false; // proizvod koji nije dostupan pokusava izmeniti prodavac
            }

            // ipak postoji proizvod za brisanje u listi svih proizvoda
            foreach (Proizvod proizvod in za_brisanje)
            {
                if (proizvod.IsDeleted == false)
                {
                    // nije jos uvek obrisan
                    // obrisemo ga logicki
                    proizvod.IsDeleted = true;
                }
            }

            // recenzije za dati proizvod vise nije moguce ni postaviti ni menjati, pa se logicki brisu
            List<Recenzija> recenzije = RecenzijeStorage.Recenzije.FindAll(p => p.Proizvod.Id == id && !p.IsDeleted);

            foreach(Recenzija recenzija in recenzije)
            {
                recenzija.IsDeleted = true; // brisanje recenzije, nezavisno od statusa (odobrena/odbijena)
            }

            // Azuriranje u json fajlovima, novo izmenjenih entiteta
            AzurirajProizvodeUBazi();
            KorisniciStorage.AzurirajKorisnikeUBazi();
            PorudzbineStorage.AzurirajPorudzbineUBazi();
            RecenzijeStorage.AzurirajRecenzijeUBazi();

            return true;
        }

        // Metoda koja vraca sve proizvode koji nisu obrisani
        public static List<Proizvod> GetSviProizvodi()
        {
            return Proizvodi.FindAll(p => p.IsDeleted == false);
        }

        // Metoda koja vraca sve proizvode koji nisu obrisani i koji su dostupni
        public static List<Proizvod> GetDostupniSviProizvodi()
        {
            return GetSviProizvodi().FindAll(p => p.Status == true);
        }

        public static Proizvod GetProizvodPoId(string id)
        {
            if (!int.TryParse(id, out int idp))
            {
                return new Proizvod() { Id = 0 }; // ne postoji proizvod
            }
            else
            {
                // proizvod sa trazenim id i da nije obrisan
                Proizvod proizvod = Proizvodi.Find(p => p.IsDeleted == false && p.Id == idp);

                if (proizvod == null)
                {
                    return new Proizvod() { Id = 0 }; // ne postoji proizvod
                }
                else
                {
                    return proizvod;
                }
            }
        }

        // Metoda koja vraca dostupan proizvod
        public static Proizvod GetProizvodPoIdDostupan(string id)
        {
            if (!int.TryParse(id, out int idp))
            {
                return new Proizvod() { Id = 0 }; // ne postoji proizvod
            }
            else
            {
                // proizvod sa trazenim id i da nije obrisan
                Proizvod proizvod = Proizvodi.Find(p => p.IsDeleted == false && p.Id == idp && p.Status == true);

                if (proizvod == null)
                {
                    return new Proizvod() { Id = 0 }; // ne postoji proizvod
                }
                else
                {
                    return proizvod;
                }
            }
        }

        // Metoda za azuriranje proizvoda
        public static bool AzuriranjeProizvoda(string id_str, string naziv, double cena, double kolicina, string opis, string slika, string grad)
        {
            // nije unet validan broj tj id koji je broj
            if (!int.TryParse(id_str, out int id))
            {
                return false;
            }

            // Azuriraj sve proizvode u bazi, kao i korisnike (prodavac koji je kreirao taj proizvod,
            // i kupce koji su taj proizvod dodali u AKTIVNE porudzbine i/ili omiljeni)

            // prvo prolazimo kroz listu svih proizvoda i azuriramo proizvod
            // proizvod nije obrisan
            List<Proizvod> za_izmenu = Proizvodi.FindAll(p => p.Id == id && p.IsDeleted == false);

            if (za_izmenu.Count == 0)
            {
                return false; // proizvod je vec obrisan, nema daljeg azuriranja 
            }

            // da li proizvod menja prodavac, jer ako prodavac menja onda je potrebno proveriti i status proizvoda
            // specifikacija: proizvodi koji nisu dostupni ne mogu biti obrisani niti izmenjeni
            if (((Korisnik)HttpContext.Current.Session["korisnik"]).Uloga == ULOGA.Prodavac &&
                za_izmenu.FindAll(p => p.Status == false).Count > 0)
            {
                return false; // proizvod koji nije dostupan pokusava izmeniti prodavac
            }

            // ipak postoji proizvod za izmenu u listi svih proizvoda
            foreach (Proizvod pr in za_izmenu)
            {
                int index = Proizvodi.FindIndex(p => p.Id == pr.Id);

                if(index != -1)
                {
                    Proizvodi[index].Naziv = naziv;
                    Proizvodi[index].Cena = cena;
                    Proizvodi[index].Kolicina = kolicina;
                    Proizvodi[index].Opis = opis;
                    Proizvodi[index].Slika = slika;
                    Proizvodi[index].Grad = grad;
                }
                // azuriranje podataka o proizvodu
                
            }

            // Azuriranje u json fajlovima, novo izmenjenih entiteta
            AzurirajProizvodeUBazi();
            KorisniciStorage.AzurirajKorisnikeUBazi();
            PorudzbineStorage.AzurirajPorudzbineUBazi();
            RecenzijeStorage.AzurirajRecenzijeUBazi();

            return za_izmenu.Count > 0;
        }
    }
}