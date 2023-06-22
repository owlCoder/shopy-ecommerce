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
                    int proizvodIDIzPorudzbine = PorudzbineStorage.Porudzbine.FirstOrDefault(p => p.Id == pid && p.IsDeleted == false && p.Proizvod.IsDeleted == false).Proizvod.Id;

                    if(proizvodIDIzPorudzbine == -1)
                    {
                        // proizvod je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće dodati recenziju! Proizvod je obrisan." });
                    }

                    int recenziran = ProizvodiStorage.Proizvodi.FindIndex(p => p.Id == proizvodIDIzPorudzbine && !p.IsDeleted);

                    if(recenziran == -1)
                    {
                        // proizvod je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće dodati recenziju! Proizvod je obrisan." });
                    }

                    Proizvod za_recenziju = ProizvodiStorage.Proizvodi[proizvodIDIzPorudzbine];
                    int kid = KorisniciStorage.Korisnici.FindIndex(p => p.Id.Equals(trenutni.Id));

                    if(kid == -1)
                    {
                        // korisnik je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Nije moguće dodati recenziju! Korisnik je obrisan." });
                    }

                    Korisnik tren = KorisniciStorage.Korisnici[kid];
                    Recenzija nova = new Recenzija(za_recenziju, tren, zahtev.Naslov, zahtev.Sadrzaj, zahtev.Slika);
                    Recenzija nova_temp = new Recenzija(za_recenziju, tren, zahtev.Naslov, zahtev.Sadrzaj, zahtev.Slika);
                    nova.PID = pid;
                    nova_temp.PID = pid;
                    RecenzijeStorage.Recenzije.Add(nova);
                    ProizvodiStorage.Proizvodi[proizvodIDIzPorudzbine].Recenzija.Add(nova_temp);

                    // azuziranje json
                    ProizvodiStorage.AzurirajProizvodeUBazi();
                    RecenzijeStorage.AzurirajRecenzijeUBazi();

                    return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Recenzija uspešno dodata!" });
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste uneli validne podatka za recenziju!" });
            }
        }

        // Metoda za izmenu recenzije
        [HttpPost]
        [Route("IzmenaRecenzije")]
        public string IzmeniRecenziju(RecenzijaEditRequest zahtev)
        {
            return JsonConvert.SerializeObject("");
        }

        // Metoda za brisanje recenzije
        [HttpPost]
        [Route("BrisanjeRecenzije")]
        public string IzmeniRecenziju(SingleIdRequest zahtev)
        {
            return JsonConvert.SerializeObject("");
        }

        // Metoda koja proverava da li postoji recenzija za porudzbinu
        [HttpPost]
        [Route("PostojiRecenzija")]
        public string RecenzijaPostoji(SingleIdRequest zahtev)
        {
            int id = 0;
            if(!ModelState.IsValid || !int.TryParse(zahtev.Id, out id))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 41, Poruka = "Niste uneli validne podatke!" });
            }

            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false && (trenutni.Uloga != ULOGA.Kupac || trenutni.Uloga != ULOGA.Administrator))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (RecenzijeStorage.Recenzije.FirstOrDefault(p => p.PID == id) != null)
            {
                // postoji recenzija za datu porudzbinu
                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "OK" });
            }
            else
            {
                // ne postoji recenzija za datu porudzbinu
                return JsonConvert.SerializeObject(new Response { Kod = 5, Poruka = "Ne postoji" });
            }
        }
    }
}
