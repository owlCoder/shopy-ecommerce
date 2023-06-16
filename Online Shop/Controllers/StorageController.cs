using Newtonsoft.Json;
using Online_Shop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/storage")]
    public class StorageController : ApiController
    {
        [HttpPost]
        [Route("OtpremanjeSlike")]
        public string Otpremanje()
        {
            HttpFileCollection slika_forma = HttpContext.Current.Request.Files;
            HttpPostedFile otpremljena_slika = slika_forma.Get("slika");

            // Ako direktorijum ne postoji, kreiraj ga
            if(!Directory.Exists(HostingEnvironment.MapPath(@"~/Uploads/")))
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath(@"~/Uploads/"));
            }

            try
            {
                otpremljena_slika.SaveAs(Path.Combine(HostingEnvironment.MapPath(@"~/Uploads/") + new Guid().ToString() + ".jpg"));
                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
            }
            catch(Exception)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 11, Poruka = " Došlo je do neočekivane greške, pokušajte ponovo!" });
            }
        }
    }
}
