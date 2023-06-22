using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Models.Requests;
using Online_Shop.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Online_Shop.Controllers
{
    [RoutePrefix("api/reviews")]
    public class RecenzijeController : ApiController
    {
        // Metoda koja vraca sve recenzije za dati proizvod
        [HttpPost]
        [Route("RecenzijeZaProizvodPoId")]
        public string RecenzijePoProizvodu(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (!ModelState.IsValid || int.TryParse(zahtev.Id, out int id))
            {
                return JsonConvert.SerializeObject(new List<Recenzija>());
            }
            else
            {
                return JsonConvert.SerializeObject(RecenzijeStorage.RecenzijePoProizvodu(id));
            }
        }

        // Metoda koja vraca sve recenzije od datog korisnika po korisnickom imenu
        // stranica za kupce, moje recenzije
        [HttpPost]
        [Route("RecenzijePoKupcu")]
        public string RecenzijeKupac(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (!ModelState.IsValid)
            {
                return JsonConvert.SerializeObject(new List<Recenzija>());
            }
            else
            {
                return JsonConvert.SerializeObject(RecenzijeStorage.RecenzijePoKorisniku(zahtev.Id));
            }
        }

        // Metoda za proveru da li postoji porudzbina za koju je ostavljena recenzija
        // 

        // Metoda za dodavanje porudzbine
        [HttpPost]
        [Route("DodavanjeRecenzije")]
        public string DodajRecenziju(RecenzijaAddRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            // recenziju moze da dodaje samo kupac
            if(ModelState.IsValid)
            {
                // da li postoji porudzbina u listi porudzbina i da li je mozda porudzbina obrisana u medjuvremenu
                int pid = zahtev.PorudzbinaId; // porudzbina za koju je vezana recenzija

                if (PorudzbineStorage.Porudzbine.FirstOrDefault(p => p.Id == pid && p.IsDeleted == false && p.Proizvod.IsDeleted == false) == null)
                {
                    // ne postoji porudzbin
                    return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće dodati recenziju! Porudžbina više nije dostupna." });
                }
                else
                {
                    // porudzbina postoji - ako vec postoji recenzija za porudzbinu, ne dozvoliti dodavanje nove
                    if(RecenzijeStorage.Recenzije.FirstOrDefault(p => p.PID == pid && p.IsDeleted == false) != null)
                    {
                        // vec postoji recenzija
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće dodati recenziju! Porudžbina je već recenzirana." });
                    }

                    // ne postoji nijedna recenzija moze se dodati nova
                    int proizvodIzPorudzbine = PorudzbineStorage.Porudzbine.FirstOrDefault(p => p.Id == pid && p.IsDeleted == false && p.Proizvod.IsDeleted == false).Proizvod.Id;

                    if(proizvodIzPorudzbine == -1)
                    {
                        // proizvod je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće dodati recenziju! Proizvod je obrisan." });
                    }

                    int recenziran = ProizvodiStorage.Proizvodi.FindIndex(p => p.Id == proizvodIzPorudzbine && !p.IsDeleted);

                    Recenzija nova = new Recenzija();
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste uneli validne podatka za recenziju!" });
            }

        }
    }
}
