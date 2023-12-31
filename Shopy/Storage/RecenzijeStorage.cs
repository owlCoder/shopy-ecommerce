﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
                    Recenzije = JsonConvert.DeserializeObject<List<Recenzija>>(File.ReadAllText(RecenzijePath), new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy", Culture = System.Globalization.CultureInfo.InvariantCulture });

                    foreach (Proizvod pr in ProizvodiStorage.Proizvodi)
                    {
                        // taj proizvod dodaj u porudzbinu kojoj pripada
                        foreach (int rid in pr.RID)
                        {
                            int recenzija = Recenzije.FindIndex(p => p.Id == rid);
                            if (recenzija != -1)
                            {
                                Recenzija rec = Recenzije[recenzija];
                                int korisnik = KorisniciStorage.Korisnici.FindIndex(p => p.Id.Equals(rec.KOID.ToString()));
                                int porudzbina = PorudzbineStorage.Porudzbine.FindIndex(p => p.Id == rec.POID);

                                if (korisnik != -1 && porudzbina != -1)
                                {
                                    Recenzije[recenzija].Proizvod = pr; // proizvod u porudzbini
                                    Recenzije[recenzija].Recenzent = KorisniciStorage.Korisnici[korisnik]; // recenzent porudzbine
                                }
                            }
                        }
                    }
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
                string json = JsonConvert.SerializeObject(Recenzije, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy", Culture = System.Globalization.CultureInfo.InvariantCulture });
                File.WriteAllText(RecenzijePath, json);
            }
            catch { }
        }

        // Metoda koja vraca trazenu recenziju po Id
        public Recenzija GetRecenzijaById(int id)
        {
            return Recenzije.FirstOrDefault(p => p.Id == id);
        }

        // Metoda koja vraca recenzije od datog korisnika
        public static List<Recenzija> RecenzijePoProizvodu(int proizvod_id)
        {
            // vracaju se samo ne obrisane, odobrene recenzije i za dati proizvod
            // return Recenzije.FindAll(p => p.IsDeleted == false && p.Odobrena == true && p.Proizvod.Id == proizvod_id);
            // proizvod u sebi ima listu recenzija - brza pretraga nego lista svih recenzije
            Proizvod trazeni = ProizvodiStorage.Proizvodi.FirstOrDefault(p => p.Id == proizvod_id);

            if (trazeni == null)
            {
                return new List<Recenzija>();
            }
            else
            {
                // sve recenzije koje su odobrene i nisu obrisane
                return Recenzije.FindAll(p => p.IsDeleted == false && p.Status == STATUS_RECENZIJE.ODOBRENA && p.Proizvod.Id == proizvod_id);
            }
        }

        // Metoda koja vraca sve recenzije za dato korisnicko ime
        public static List<Recenzija> RecenzijePoKorisniku(string korisnickoIme)
        {
            // vracaju se samo ne obrisane, sve recenzije i od datog korisnika
            return Recenzije.FindAll(p => p.IsDeleted == false && p.Recenzent.Id.Equals(korisnickoIme));
        }
    }
}