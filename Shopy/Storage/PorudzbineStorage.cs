using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Online_Shop.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Online_Shop.Storage
{
    public class PorudzbineStorage
    {
        public static List<Porudzbina> Porudzbine { get; set; }
        public static string PorudzbinePath = HostingEnvironment.MapPath(@"~/App_Data/porudzbine.json");

        public PorudzbineStorage()
        {
            // Prazan konstruktor - zbog serijalizacije
        }

        // Metoda koja iscitava sve porudzbine iz json datoteke
        public static void UcitajPorudzbine()
        {
            // ucitavanje porudzbina iz datoteke - ako ona postoji
            if (File.Exists(PorudzbinePath))
            {
                try
                {
                    Porudzbine = new List<Porudzbina>();

                    // ucitavanje svih porudzbina iz json datoteke
                    Porudzbine = JsonConvert.DeserializeObject<List<Porudzbina>>(File.ReadAllText(PorudzbinePath), new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy", Culture = System.Globalization.CultureInfo.InvariantCulture });

                    // za svaku porudzbinu proizvod
                    foreach (Proizvod pr in ProizvodiStorage.Proizvodi)
                    {
                        // taj proizvod dodaj u porudzbinu kojoj pripada
                        foreach (int pid in pr.PID)
                        {
                            int porudzbina = Porudzbine.FindIndex(p => p.Id == pid);

                            if (porudzbina != -1)
                            {
                                int korisnikp = KorisniciStorage.Korisnici.FindIndex(p => p.Id.Equals(Porudzbine[porudzbina].Kupac));

                                if(korisnikp != -1)
                                {
                                    Porudzbine[porudzbina].Proizvod = pr; // proizvod u porudzbini
                                    KorisniciStorage.Korisnici[korisnikp].Porudzbine.Add(Porudzbine[porudzbina]);
                                }
                            }
                        }
                    }

                }
                catch
                {
                    // ne postoji datoteka - pa se kreira prazna kolekcija
                    Porudzbine = new List<Porudzbina>();
                }
            }
            else
            {
                // ne postoji datoteka - pa se kreira prazna kolekcija
                Porudzbine = new List<Porudzbina>();
            }
        }

        // Metoda za azuriranje proizvoda u json datoteci
        public static void AzurirajPorudzbineUBazi()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Porudzbine, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy", Culture = System.Globalization.CultureInfo.InvariantCulture });
                File.WriteAllText(PorudzbinePath, json);
            }
            catch { }
        }

        // Metoda koja vraca trazeni proizvod po Id
        public static Porudzbina GetPorudzbinaById(int id)
        {
            return Porudzbine.FirstOrDefault(p => p.Id == id);
        }

        // Metoda koja vraca listu svih porudzbina koje nisu obrisane i pripadaju kupcu
        public static List<Porudzbina> PorudzbineKupac()
        {
            string id = ((Korisnik)HttpContext.Current.Session["korisnik"]).Id;
            return Porudzbine.FindAll(p => p.Kupac.Equals(id));
        }
    }
}