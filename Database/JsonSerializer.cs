using Newtonsoft.Json;
using System.IO;
using WebProjekat.Controllers;

namespace WebProjekat.Database
{
    public class JsonSerializer
    {
        public static void Serialize()
        {
            File.WriteAllText(HomeController.AppData + "studenti.json", JsonConvert.SerializeObject(HomeController.Studenti, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            File.WriteAllText(HomeController.AppData + "profesori.json", JsonConvert.SerializeObject(HomeController.Profesori, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            File.WriteAllText(HomeController.AppData + "ispiti.json", JsonConvert.SerializeObject(HomeController.Ispiti, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            File.WriteAllText(HomeController.AppData + "rezultati.json", JsonConvert.SerializeObject(HomeController.RezultatiIspita, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}