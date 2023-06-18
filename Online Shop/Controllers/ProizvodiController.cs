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
            if (ModelState.IsValid)
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

        // Metoda za prikaz dostupnih proizvoda i filtriranje po korisniku
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

        // Metoda za prikaz dostupnih proizvoda i filtriranje u CELOM SISTEMU
        [HttpPost]
        [Route("ListaFiltriranihSvihProizvoda")]
        public string PrikazSvihDostupnihProizvoda(SingleIdRequest zahtev)
        {
            string[] request = zahtev.Id.Split(';');
            List<Proizvod> proizvodi;

            if (request[0].Equals("0"))
            {
                proizvodi = ProizvodiStorage.GetSviProizvodi();
            }
            else
            {
                proizvodi = ProizvodiStorage.GetDostupniSviProizvodi();
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

        // Metoda za prikaz svih proizvoda koji nisu obrisani i koji su dostupni
        [HttpGet]
        [Route("SviProizvodiPocetna")]
        public string PrikazSvihProizvoda()
        {
            return JsonConvert.SerializeObject(ProizvodiStorage.Proizvodi.FindAll(p => p.IsDeleted == false && p.Status == true));
        }

        [HttpGet]
        [Route("SviProizvodi")]
        public string PrikazSvihProizvodaAdministracija()
        {
            return JsonConvert.SerializeObject(ProizvodiStorage.Proizvodi.FindAll(p => p.IsDeleted == false));
        }

        // Metoda za brisanje proizvoda
        // proizvodi koji nisu dostupni (povuceni su) ne mogu biti obrisani niti izmenjeni
        [HttpPost]
        [Route("BrisanjeProizvoda")]
        public string ObrisiProizvod(SingleIdRequest zahtev)
        {
            if(int.TryParse(zahtev.Id, out int idp) && ProizvodiStorage.DeleteProizvod(idp))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Proizvod uspešno obrisan iz liste proizvoda." });
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 13, Poruka = "Proizvod nije moguće uspešno obrisati iz liste svih proizvoda. Proverite da li je proizvod na stanju!"});
            }
        }
    }
}
