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
                    Korisnici = JsonConvert.DeserializeObject<Dictionary<string, Korisnik>>(KorisniciPath);
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
            return Korisnici.ContainsKey(username);
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
    }
}