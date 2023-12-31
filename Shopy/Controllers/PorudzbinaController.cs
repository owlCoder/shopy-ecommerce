﻿using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Models.Requests;
using Online_Shop.Models.Responses;
using Online_Shop.Storage;
using System.Globalization;
using System.Linq;
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
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            // prima se Id proizvoda koji se narucuje kao i kolicina koja je trazena
            if (ModelState.IsValid)
            {
                // proveriti da li je trenutni korisnik kupac
                Korisnik korisnik = ((Korisnik)HttpContext.Current.Session["korisnik"]);

                if (korisnik == null || korisnik.IsLoggedIn == false || korisnik.Uloga != ULOGA.Kupac)
                {
                    JsonConvert.SerializeObject(new Response { Kod = 21, Poruka = "Potrebno je da se ponovo prijavite kako bi izvršili porudžbinu!!" });
                }
                else
                {
                    // korisnik je prijavljen i kupac je
                    int proizvod = ProizvodiStorage.Proizvodi.FindIndex(p => p.IsDeleted == false && p.Id == order.Id);
                    int korisnikUlogovan = KorisniciStorage.Korisnici.FindIndex(p => p.Id == korisnik.Id && p.IsLoggedIn);

                    if (proizvod == -1 || korisnikUlogovan == -1)
                    {
                        return JsonConvert.SerializeObject(new Response { Kod = 41, Poruka = "Nažalost, proizvod više nije u ponudi," });
                    }
                    else
                    {
                        if (ProizvodiStorage.Proizvodi[proizvod].Kolicina < order.Kolicina)
                        {
                            // da li korisnik narucuje vise nego sto trenutno ima na stanju
                            return JsonConvert.SerializeObject(new Response { Kod = 42, Poruka = "Nažalost, količina proizvoda koju želite više nije dostupna. Smanjite količinu i pokušajte ponovo." });
                        }
                        else
                        {
                            Porudzbina nova = new Porudzbina(ProizvodiStorage.Proizvodi[proizvod], order.Kolicina, korisnik.Id);
                            ProizvodiStorage.Proizvodi[proizvod].PID.Add(nova.Id); // pid
                            int korid = KorisniciStorage.Korisnici.FindIndex(p => p.Id.Equals(korisnik.Id));

                            // pokusaj smanjenja stanja kolicine proizvoda
                            if (korid != -1 && ProizvodiStorage.AzuriranjeProizvoda(ProizvodiStorage.Proizvodi[proizvod].Id.ToString(),
                                ProizvodiStorage.Proizvodi[proizvod].Naziv, ProizvodiStorage.Proizvodi[proizvod].Cena,
                                ProizvodiStorage.Proizvodi[proizvod].Kolicina - order.Kolicina,
                                ProizvodiStorage.Proizvodi[proizvod].Opis,
                                ProizvodiStorage.Proizvodi[proizvod].Slika, ProizvodiStorage.Proizvodi[proizvod].Grad))
                            {
                                // tek sada se moze dodati porudzbina
                                PorudzbineStorage.Porudzbine.Add(nova);
                                int pid = PorudzbineStorage.Porudzbine.FindIndex(p => p.Id == nova.Id);
                                KorisniciStorage.Korisnici[korid].Porudzbine.Add(PorudzbineStorage.Porudzbine[pid]);
                                KorisniciStorage.AzurirajKorisnikeUBazi();
                                PorudzbineStorage.AzurirajPorudzbineUBazi();

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

        // Metoda koja izlistava sve porudzbine koje je napravio trenutni kupac
        [HttpGet]
        [Route("MojePorudzbine")]
        public string PorudzbineKupac()
        {
            // proveriti da li je trenutni korisnik kupac
            Korisnik korisnik = ((Korisnik)HttpContext.Current.Session["korisnik"]);

            if (korisnik == null || korisnik.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 21, Poruka = "Potrebno je da se ponovo prijavite kako bi videli svoje porudžbine!!" });
            }
            else
            {
                return JsonConvert.SerializeObject(Enumerable.Reverse(PorudzbineStorage.PorudzbineKupac()).ToList(), new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }

        }

        // Metoda koja menja status porudzbine na prispela ako je moguce
        [HttpPost]
        [Route("OznaciPrispece")]
        public string PrispelaPorudzbina(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false && (trenutni.Uloga != ULOGA.Kupac || trenutni.Uloga != ULOGA.Administrator))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            // jeste admin ili kupac, pa moze promeniti status porudzbine ako nije obrisana ili vec promenjenog statusa
            if(!int.TryParse(zahtev.Id, out int pid))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 43, Poruka = "Nije moguće izmeniti status porudžbine!" });
            }

            // validan id format, provera da li porudzbina postoji
            int index = PorudzbineStorage.Porudzbine.FindIndex(p => p.Id == pid && p.IsDeleted == false);

            // Porudzbina ne postoji
            if(index == -1)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 32, Poruka = "Porudžbina ne postoji! Nije moguće označiti prispeće" });
            }

            // porudzbina postoji - korisnik kome pripada porudzbina
            if(!int.TryParse(PorudzbineStorage.Porudzbine[index].Kupac, out int kid))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 43, Poruka = "Nije moguće izmeniti status porudžbine!" });
            }

            STATUS status = PorudzbineStorage.Porudzbine[index].Status;
            int kupacid = KorisniciStorage.Korisnici.FindIndex(p => p.Id.Equals(kid.ToString()) && p.IsDeleted == false);

            if (kupacid == -1)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 61, Poruka = "Porudžbina ne postoji! Nije moguće označiti prispeće." });
            }

            int kpidx = KorisniciStorage.Korisnici[kupacid].Porudzbine.FindIndex(p => p.Id == pid && p.IsDeleted == false);

            if (status == STATUS.AKTIVNA && kpidx != -1)
            {
                // oznacavanje prispeca
                PorudzbineStorage.Porudzbine[index].Status = STATUS.IZVRSENA;
                KorisniciStorage.Korisnici[kupacid].Porudzbine[kpidx].Status = STATUS.IZVRSENA;

                // azuriranje stanja
                KorisniciStorage.AzurirajKorisnikeUBazi();
                PorudzbineStorage.AzurirajPorudzbineUBazi();

                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Porudžbina je uspešno označena kao izvršena." });
            }

            return JsonConvert.SerializeObject(new Response { Kod = 43, Poruka = "Nije moguće izmeniti status porudžbine!" });
        }

        // Metoda koja menja status porudzbine na otkazana ako je moguce
        [HttpPost]
        [Route("OtkazivanjePorudzbine")]
        public string OtkazivanjePorudzbine(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Administrator)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            // jeste admin pa moze promeniti status porudzbine ako nije obrisana ili vec promenjenog statusa (nije AKTIVNA)
            if (!int.TryParse(zahtev.Id, out int pid))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 43, Poruka = "Nije moguće izmeniti status porudžbine!" });
            }

            // validan id format, provera da li porudzbina postoji
            int index = PorudzbineStorage.Porudzbine.FindIndex(p => p.Id == pid && p.IsDeleted == false);

            // Porudzbina ne postoji
            if (index == -1)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 32, Poruka = "Porudžbina ne postoji! Nije moguće njeno otkazivanje." });
            }

            // porudzbina postoji - korisnik kome pripada porudzbina
            if (!int.TryParse(PorudzbineStorage.Porudzbine[index].Kupac, out int kid))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 43, Poruka = "Nije moguće izmeniti status porudžbine!" });
            }

            STATUS status = PorudzbineStorage.Porudzbine[index].Status;
            int kupacid = KorisniciStorage.Korisnici.FindIndex(p => p.Id.Equals(kid.ToString()) && p.IsDeleted == false);

            if (kupacid == -1)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 61, Poruka = "Porudžbina ne postoji! Nije moguće njeno otkazivanje." });
            }

            int kpidx = KorisniciStorage.Korisnici[kupacid].Porudzbine.FindIndex(p => p.Id == pid && p.IsDeleted == false);

            if (status == STATUS.AKTIVNA && kpidx != -1)
            {
                // oznacavanje prispeca
                PorudzbineStorage.Porudzbine[index].Status = STATUS.OTKAZANA;
                KorisniciStorage.Korisnici[kupacid].Porudzbine[kpidx].Status = STATUS.OTKAZANA;

                // kada se porudzbina otkaze, sva kolicina koja je narucena se vraca na kolicinu u proizvodu
                int proid = PorudzbineStorage.Porudzbine[index].Proizvod.Id;
                int proidx = ProizvodiStorage.Proizvodi.FindIndex(p => p.Id == proid);

                // vrati kolicinu iz otkazane porudzbine
                ProizvodiStorage.Proizvodi[proidx].Kolicina += PorudzbineStorage.Porudzbine[index].Kolicina;

                // azuriranje stanja
                KorisniciStorage.AzurirajKorisnikeUBazi();
                PorudzbineStorage.AzurirajPorudzbineUBazi();
                ProizvodiStorage.AzurirajProizvodeUBazi();
                RecenzijeStorage.AzurirajRecenzijeUBazi();

                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Porudžbina je uspešno otkazana! Poručena količina iz porudžbine je vraćena na lager." });
            }

            return JsonConvert.SerializeObject(new Response { Kod = 43, Poruka = "Nije moguće izmeniti status porudžbine!" });
        }

        [HttpGet]
        [Route("SvePorudzbine")]
        public string SvePorudzbine()
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Administrator)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            // prioritet prikaza imaju aktivne porudzbine
            return JsonConvert.SerializeObject(Enumerable.Reverse(PorudzbineStorage.Porudzbine.FindAll(p => p.IsDeleted == false)).ToList());
        }

        // Metoda koja vraca tacno odredjenu porudzbinu
        [HttpPost]
        [Route("PorudzbinaRecenzija")]
        public string PorudzbinaPoIdZaRecenziju(PorudzbinaShowRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false && (trenutni.Uloga != ULOGA.Kupac || trenutni.Uloga != ULOGA.Administrator))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            Porudzbina pronadjena = PorudzbineStorage.Porudzbine.FirstOrDefault(p => p.IsDeleted == false && p.Status == STATUS.IZVRSENA && p.Id == zahtev.Id);

            if (pronadjena == null)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 15, Poruka = "Porudžbina više ne postoji na platformi!" });
            }
            else
            {
                string datum = pronadjena.DatumPorudzbine.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                return JsonConvert.SerializeObject(new PorudzbinaReviewResponse { 
                    Id = pronadjena.Id, 
                    Datum = datum, 
                    NazivProizvoda = pronadjena.Proizvod.Naziv, 
                    Slika = pronadjena.Proizvod.Slika, 
                    UkupanIznos = pronadjena.Proizvod.Cena * pronadjena.Kolicina});
            }    
        }
    }
}
