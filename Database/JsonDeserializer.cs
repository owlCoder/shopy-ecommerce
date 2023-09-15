using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using WebProjekat.Controllers;
using WebProjekat.Models;

namespace WebProjekat.Database
{
    public class JsonDeserializer
    {
        public static void Deserialize()
        {
            if (File.Exists(HomeController.AppData + "administratori.json"))
            {
                using (StreamReader sr = new StreamReader(HomeController.AppData + "administratori.json"))
                {
                    HomeController.Administratori = JsonConvert.DeserializeObject<List<Administrator>>(sr.ReadToEnd());
                    sr.Dispose();
                }
            }

            if (File.Exists(HomeController.AppData + "profesori.json"))
            {
                using (StreamReader sr = new StreamReader(HomeController.AppData + "profesori.json"))
                {
                    HomeController.Profesori = JsonConvert.DeserializeObject<List<Profesor>>(sr.ReadToEnd());
                    sr.Dispose();
                }
            }

            if (File.Exists(HomeController.AppData + "studenti.json"))
            {
                using (StreamReader sr = new StreamReader(HomeController.AppData + "studenti.json"))
                {
                    HomeController.Studenti = JsonConvert.DeserializeObject<List<Student>>(sr.ReadToEnd());
                    sr.Dispose();
                }
            }

            if (File.Exists(HomeController.AppData + "ispiti.json"))
            {
                using (StreamReader sr = new StreamReader(HomeController.AppData + "ispiti.json"))
                {
                    HomeController.Ispiti = JsonConvert.DeserializeObject<List<Ispit>>(sr.ReadToEnd());
                    sr.Dispose();
                }
            }

            if (File.Exists(HomeController.AppData + "rezultati.json"))
            {
                using (StreamReader sr = new StreamReader(HomeController.AppData + "rezultati.json"))
                {
                    HomeController.RezultatiIspita = JsonConvert.DeserializeObject<List<RezultatIspita>>(sr.ReadToEnd());
                    sr.Dispose();
                }
            }
        }
    }
}