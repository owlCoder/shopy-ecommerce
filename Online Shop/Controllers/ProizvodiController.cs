using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/products")]
    public class ProizvodiController : ApiController
    {
        // Metoda za dodavanje proizvoda
        [HttpPost]
        [Route("DodavanjeProizvoda")]
        public string DodajProizvod(ProizvodAddRequest zahtev)
        {
            if(ModelState.IsValid)
            {
                ProizvodiStorage.DodajProizvod(zahtev.Naziv, zahtev.Cena, zahtev.Kolicina, zahtev.Opis, zahtev.Slika, zahtev.Grad);
                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Uneti podaci nisu validni!" });
            }    
        }

        // Metoda za prikazivanje svih proizvoda
        [HttpGet]
        [Route("ListaProizvoda")]
        public string PrikazObjavljenihProizvoda()
        {
            return JsonConvert.SerializeObject(ProizvodiStorage.GetProizvodiPerUser());
        }

        [HttpPost]
        [Route("ListaFiltriranihProizvoda")]
        public string PrikazDostupnihProizvoda(SingleIdRequest zahtev)
        {
            string[] request = zahtev.Id.Split(';');
            List<Proizvod> proizvodi;

            if (request[0].Equals("0"))
            {
                proizvodi = ProizvodiStorage.GetProizvodiPerUser();
            }
            else
            {
                proizvodi = ProizvodiStorage.GetDostupniProizvodi();
            }

            // filtiranje po drugom kriterijumu - ako nije 0 - podrazumevano
            if (request[1].Equals("0") || request[1].Equals("")) 
            { 
                return JsonConvert.SerializeObject(proizvodi);
            }
            else
            {
                proizvodi = ProizvodiStorage.SortirajPoKriterijumu(request[1], proizvodi);
                return JsonConvert.SerializeObject(proizvodi);
            }
        }
    }
}
