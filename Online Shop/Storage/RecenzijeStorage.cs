using Newtonsoft.Json;
using Online_Shop.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Online_Shop.Storage
{
    public class RecenzijeStorage
    {
        public static List<Recenzija> Recenzije { get; set; }
        public static string RecenzijePath = HostingEnvironment.MapPath(@"~/App_Data/recenzije.json");

        public RecenzijeStorage()
        {
            // Prazan konstruktor - zbog serijalizacije
        }

        // Metoda koja iscitava sve recenzije iz json datoteke
        public static void UcitajRecenzije()
        {
            // ucitavanje porudzbina iz datoteke - ako ona postoji
            if (File.Exists(RecenzijePath))
            {
                try
                {
                    Recenzije = new List<Recenzija>();

                    // ucitavanje svih porudzbina iz json datoteke
                    Recenzije = JsonConvert.DeserializeObject<List<Recenzija>>(File.ReadAllText(RecenzijePath));
                }
                catch
                {
                    // ne postoji datoteka - pa se kreira prazna kolekcija
                    Recenzije = new List<Recenzija>();
                }
            }
            else
            {
                // ne postoji datoteka - pa se kreira prazna kolekcija
                Recenzije = new List<Recenzija>();
            }
        }

        // Metoda za azuriranje proizvoda u json datoteci
        public static void AzurirajRecenzijeUBazi()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Recenzije);
                File.WriteAllText(RecenzijePath, json);
            }
            catch { }
        }

        // Metoda koja vraca trazenu recenziju po Id
        public Recenzija GetRecenzijaById(int id)
        {
            return Recenzije.FirstOrDefault(p => p.Id == id);
        }
    }
}