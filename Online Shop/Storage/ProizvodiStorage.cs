﻿using Newtonsoft.Json;
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

        // 
    }
}