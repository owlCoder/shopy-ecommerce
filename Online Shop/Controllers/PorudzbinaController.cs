using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/orders")]
    public class PorudzbinaController : ApiController
    {
        [HttpPost]
        [Route("KreiranjePorudzbine")]
        public string KreirajPorudzbinu(OrderRequest order)
        {
            // prima se Id proizvoda koji se narucuje kao i kolicina koja je trazena
            if(ModelState.IsValid)
            {
                // proveriti da li je trenutni korisnik kupac
                Korisnik korisnik = ((Korisnik)HttpContext.Current.Session["korisnik"]);

                if(korisnik == null && korisnik.Uloga == ULOGA.Kupac)
                {
                    JsonConvert.SerializeObject(new Response { Kod = 21, Poruka = "Potrebno je da se ponovo prijavite kako bi izvršili porudžbinu!!" });
                }
                else
                {
                    // korisnik je prijavljen i kupac je
                    Proizvod proizvod = ProizvodiStorage.Proizvodi.FirstOrDefault(p => p.IsDeleted == false && p.Id == order.Id);

                    if(proizvod == null)
                    {
                        return JsonConvert.SerializeObject(new Response { Kod = 41, Poruka = "Nažalost, proizvod više nije u ponudi," });
                    }
                    else
                    {
                        if(proizvod.Kolicina < order.Kolicina)
                        {
                            // da li korisnik narucuje vise nego sto trenutno ima na stanju
                            return JsonConvert.SerializeObject(new Response { Kod = 42, Poruka = "Nažalost, količina proizvoda koju želite više nije dostupna. Smanjite količinu i pokušajte ponovo." });
                        }
                        else
                        {
                            Porudzbina nova = new Porudzbina(proizvod, order.Kolicina, korisnik);

                            // pokusaj smanjenja stanja kolicine proizvoda
                            if(ProizvodiStorage.AzuriranjeProizvoda(proizvod.Id.ToString(), proizvod.Naziv, proizvod.Cena, proizvod.Kolicina - order.Kolicina, proizvod.Opis, proizvod.Slika, proizvod.Grad))
                            {
                                // tek sada se moze dodati porudzbina
                                korisnik.Porudzbine.Add(nova);
                                PorudzbineStorage.Porudzbine.Add(nova);

                                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Uspešno ste kreirali porudžbinu! Status porudžbine može pratiti na stranici 'Moje Porudžbine." });
                            }
                            else
                            {
                                return JsonConvert.SerializeObject(new Response { Kod = 45, Poruka = "Nije moguće kreirati porudžbinu! Pokušajte ponovo kasnije." });
                            }
                        }    
                    }    
                }

                return JsonConvert.SerializeObject(new Response { Kod = 45, Poruka = "Nije moguće kreirati porudžbinu! Pokušajte ponovo kasnije." });
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 20, Poruka = "Nije moguće izvršiti kreiranje porudžbine!" });
            }
        }
    }
}
