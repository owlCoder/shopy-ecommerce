using Newtonsoft.Json;
using Online_Shop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI.WebControls;

namespace Online_Shop.Storage
{
    public class KorisniciStorage
    {
        public static List<Korisnik> Korisnici { get; set; }
        public static string KorisniciPath = HostingEnvironment.MapPath(@"~/App_Data/korisnici.json");

        public KorisniciStorage()
        {
            // Prazan konstruktor - zbog serijalizacije
        }

        public static void UcitajKorisnike()
        {
            // ucitavanje korisnika iz datoteke - ako ona postoji
            if (File.Exists(KorisniciPath))
            {
                try
                {
                    Korisnici = new List<Korisnik>();

                    // ucitavanje svih korisnika iz json datoteke
                    Korisnici = JsonConvert.DeserializeObject<List<Korisnik>>(File.ReadAllText(KorisniciPath));

                    // izloguj sve korisnike - server se restartovao
                    foreach (Korisnik k in Korisnici)
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
                    Korisnici = new List<Korisnik>();
                }
            }
            else
            {
                // ne postoji datoteka - pa se kreira prazan recnik
                Korisnici = new List<Korisnik>();
            }
        }

        // Metoda koja proverava da li postoji korisnik sa trazenim username
        public static bool ProveriPostojiUsername(string username)
        {
            // ako postoji i nije obrisan
            Korisnik pronadjen = Korisnici.FirstOrDefault(p => p.KorisnickoIme.Equals(username));
            return (pronadjen != null && !pronadjen.IsDeleted);
        }

        // Metoda koja vraca trazenog korisnika
        public static Korisnik KorisnikPoId(string username)
        {
            // ako postoji i nije obrisan
            return Korisnici.FirstOrDefault(p => p.KorisnickoIme.Equals(username) && p.IsDeleted == false);
        }

        // Dobavi referencu korisnika iz baze podataka
        public static Korisnik GetKorisnik(string username)
        {
            return Korisnici.FirstOrDefault(p => p.KorisnickoIme.Equals(username));
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

            foreach (Korisnik k in Korisnici)
            {
                // ne prikazuju se administratori, niti LOGICKI obrisani korisnici niti trenutno ulogovan korisnik
                if (k.IsDeleted == false && k.Uloga != ULOGA.Administrator && !k.KorisnickoIme.Equals(trenutni))
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

        public static bool LogickoBrisanje(string id)
        {
            Korisnik pronadjen = Korisnici.FirstOrDefault(p => p.KorisnickoIme.Equals(id));

            if (pronadjen != null)
            {

                // ako je bio kupac vrati sve sa porudzbina na stanje, porudzbine ponisti itd
                // ako je bio prodavac...
                if (pronadjen.Uloga == ULOGA.Kupac)
                {
                    // brisu se sve recenzije kupca koje je ostavio
                    List<Recenzija> recenzije = RecenzijeStorage.Recenzije.FindAll(p => p.Recenzent.KorisnickoIme.Equals(id) && p.IsDeleted == false);
                    
                    foreach(Recenzija recenzija in recenzije)
                    {
                        recenzija.IsDeleted = true; // logicko brisanje recenzije
                    }

                    // sve porudzbine vezane za kupca se sada trebaju obrisati
                    // ako su AKTIVNE onda se kolicina iz proizvoda iz te porudzbine vraca na stanje liste SVIH proizvoda
                    List<Porudzbina> porudzbine = PorudzbineStorage.Porudzbine.FindAll(p => p.Kupac.KorisnickoIme.Equals(id) && p.IsDeleted == false);

                    foreach (Porudzbina porudzbina in porudzbine)
                    {
                        if (porudzbina.Status == STATUS.AKTIVNA)
                        {
                            // proizvod kome treba da se vrati kolicina nakon brisanja porudzbine
                            Proizvod proizvod = ProizvodiStorage.Proizvodi.FirstOrDefault(p => p.Id == porudzbina.Proizvod.Id && p.IsDeleted == false);
                            proizvod.Kolicina += porudzbina.Kolicina; // vrati onu kolicinu koju je kupac porucio
                        }

                        porudzbina.IsDeleted = true; // brisanje porudzbine
                    }
                }
                else if (pronadjen.Uloga == ULOGA.Prodavac)
                {
                    // kod logickog brisanja prodavca
                    // brisu se svi njegovi proizvodi, a uz same proizvode i sve one kolekcije gde se ti proizvodi nalaze
                    // pronadji sve AKTIVNE porudzbine u kojima se taj proizvod nalazi, i sve te porudzbine obrisati
                    // POTREBNO JE U SVIM LISTAMA KOD KUPCA OMILJENI PROIZVODI UKLONITI TE PROIZVODE OD PRODAVCA KOJI SE BRISE
                    List<Proizvod> objavljeni_proizvodi = pronadjen.ObjavljeniProizvodi;

                    // samo ako je neki proizvod i objavio pristupa se kaskadnom logickom brisanju
                    if (objavljeni_proizvodi.Count > 0)
                    {
                        List<Korisnik> kupci = Korisnici.FindAll(p => p.IsDeleted == false && p.Uloga == ULOGA.Kupac);
                        // pronaci sve kupce koji imaju neki od proizvoda u svojim omiljenim proizvodima

                        foreach (Korisnik korisnik in kupci)
                        {
                            List<Proizvod> omiljeni = korisnik.OmiljenjiProizvodi.FindAll(p => p.IsDeleted == false);
                            List<Porudzbina> porudzbine = korisnik.Porudzbine.FindAll(p => p.IsDeleted == false);

                            foreach (Proizvod tmp in objavljeni_proizvodi)
                            {
                                foreach (Proizvod kp in omiljeni)
                                {
                                    if (kp.Id == tmp.Id)
                                    {
                                        kp.IsDeleted = true; // logicko brisanje iz liste omiljenih proizvoda
                                    }
                                }

                                // posto se brisu i svi proizvodi kupca, ne treba obrisane porudzbine kao i njihove kolicine
                                // vracati na stanje u magacinu
                                foreach (Porudzbina p in porudzbine)
                                {
                                    if (p.Proizvod.Id == tmp.Id)
                                    {
                                        p.IsDeleted = true; // brisanje porudzbine koja sadrzi dati proizvod
                                    }
                                }

                                // sve recenzije koje sadrze dati proizvod takodje obrisati - proizvod vise ne postoji
                                List<Recenzija> recenzije = RecenzijeStorage.Recenzije.FindAll(p => p.IsDeleted == false && p.Proizvod.Id == tmp.Id);

                                foreach(Recenzija recenzija in recenzije)
                                {
                                    recenzija.IsDeleted = true; // logicko brisanje recenzija koje su objavljene za taj proizvod
                                }
                            }
                        }

                        // obrisati iz liste svih proizvoda, one proizvode koji pripadaju prodavcu koji se brise
                        List<Proizvod> svi = ProizvodiStorage.Proizvodi.FindAll(p => p.IsDeleted == false);

                        foreach (Proizvod za_brisanje in objavljeni_proizvodi)
                        {
                            // svaki proizvod uvek ima unique id u listi svih proizvoda pa je uvek samo jedan
                            Proizvod proizvod = svi.FirstOrDefault(p => p.Id == za_brisanje.Id);

                            if (proizvod != null)
                            {
                                proizvod.IsDeleted = true; // logicko brisanje proizvoda
                            }
                        }
                    }
                }

                // logicko brisanje korisnika
                pronadjen.IsDeleted = true;

                // azuriranje podataka u svim bazama podataka
                AzurirajKorisnikeUBazi(); // ostaje upisan u fajlu ali sa IsDeleted na true
                PorudzbineStorage.AzurirajPorudzbineUBazi();
                ProizvodiStorage.AzurirajProizvodeUBazi();
                RecenzijeStorage.AzurirajRecenzijeUBazi();

                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<AuthKorisnik> PretragaKorisnika(SearchRequest zahtev, int uloga)
        {
            List<AuthKorisnik> pretrazeni = new List<AuthKorisnik>();
            List<Korisnik> temp = Korisnici.FindAll(p => p.IsDeleted == false && p.Uloga != ULOGA.Administrator);

            if (!zahtev.Ime.Equals("")) // uneo je zahtev po imenu
            {
                temp = temp.FindAll(p => p.Ime.Contains(zahtev.Ime));
            }

            // pretraga po ostalim kriterijumima
            if (!zahtev.Prezime.Equals("")) // uneo je zahtev po prezimenu
            {
                temp = temp.FindAll(p => p.Prezime.Contains(zahtev.Prezime));
            }

            DateTime invalid_date = new DateTime(1900, 01, 01);
            if (!zahtev.Od.Equals(invalid_date) && !zahtev.Do.Equals(invalid_date)) // uneo je zahtev po datumu
            {
                temp = temp.FindAll(p => (p.DatumRodjenja >= zahtev.Od && p.DatumRodjenja <= zahtev.Do));
            }

            // pretraga po ulozi
            if (uloga != -1)
            {
                ULOGA ul = uloga == 0 ? ULOGA.Kupac : ULOGA.Prodavac;
                temp = temp.FindAll(p => p.Uloga.Equals(ul));
            }

            foreach (Korisnik k in temp)
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

                pretrazeni.Add(ak);
            }

            return pretrazeni;
        }

        // Metoda za sortiranje po kriterijumu
        // 0 - default, 1 - ime asc, 2 - ime desc, 3 - dat rodj asc, 4 - dat rodj desc, 5 - uloga asc, 6 - uloga desc
        public static List<AuthKorisnik> GetSorterd(string id)
        {
            // svi korisnici koji nisu obrisani, koji nisu admini i nisu trenutni
            string trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]).KorisnickoIme;
            List<Korisnik> korisnici = Korisnici.FindAll(p => p.IsDeleted == false && p.Uloga != ULOGA.Administrator && !p.KorisnickoIme.Equals(trenutni));
            List<AuthKorisnik> sortirani = new List<AuthKorisnik>();

            // kopiranje liste korisnika u novu sortiranu
            foreach (Korisnik k in korisnici)
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

                sortirani.Add(ak);
            }

            if (id.Equals("1"))
            {
                sortirani = sortirani.OrderBy(p => p.Ime).ToList();
            }
            else if (id.Equals("2"))
            {
                sortirani = sortirani.OrderByDescending(p => p.Ime).ToList();
            }
            if (id.Equals("3"))
            {
                sortirani = sortirani.OrderBy(p => p.DatumRodjenja).ToList();
            }
            else if (id.Equals("4"))
            {
                sortirani = sortirani.OrderByDescending(p => p.DatumRodjenja).ToList();
            }
            if (id.Equals("5"))
            {
                sortirani = sortirani.OrderBy(p => p.Uloga).ToList();
            }
            else if (id.Equals("6"))
            {
                sortirani = sortirani.OrderByDescending(p => p.Uloga).ToList();
            }

            return sortirani;
        }
    }
}