﻿using Newtonsoft.Json;
using Online_Shop.Models;
using System;
using System.IO;
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
            string naziv_fajla = Guid.NewGuid().ToString() + ".jpg";

            // Ako direktorijum ne postoji, kreiraj ga
            if (!Directory.Exists(HostingEnvironment.MapPath(@"~/Uploads/")))
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath(@"~/Uploads/"));
            }

            try
            {
                otpremljena_slika.SaveAs(Path.Combine(HostingEnvironment.MapPath(@"~/Uploads/") + naziv_fajla));
                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = naziv_fajla });
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 11, Poruka = "Došlo je do neočekivane greške, pokušajte ponovo!" });
            }
        }
    }
}
