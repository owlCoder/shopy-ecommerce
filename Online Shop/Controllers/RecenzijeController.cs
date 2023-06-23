using Newtonsoft.Json;
using Online_Shop.Models;
using Online_Shop.Models.Requests;
using Online_Shop.Storage;
using System;
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
            if (!ModelState.IsValid || !int.TryParse(zahtev.Id, out int id))
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
            if (ModelState.IsValid)
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
                    if (RecenzijeStorage.Recenzije.FirstOrDefault(p => p.POID == pid && p.IsDeleted == false) != null)
                    {
                        // vec postoji recenzija
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće dodati recenziju! Porudžbina je već recenzirana." });
                    }

                    // ne postoji nijedna recenzija moze se dodati nova
                    int proizvodIDIzPorudzbine = PorudzbineStorage.Porudzbine.FirstOrDefault(p => p.Id == pid && p.IsDeleted == false && p.Proizvod.IsDeleted == false).Proizvod.Id;

                    if (proizvodIDIzPorudzbine == -1)
                    {
                        // proizvod je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće dodati recenziju! Proizvod je obrisan." });
                    }

                    int recenziran = ProizvodiStorage.Proizvodi.FindIndex(p => p.Id == proizvodIDIzPorudzbine && !p.IsDeleted);

                    if (recenziran == -1)
                    {
                        // proizvod je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće dodati recenziju! Proizvod je obrisan." });
                    }

                    int indeks_proizvoda = ProizvodiStorage.Proizvodi.FindIndex(p => p.Id == proizvodIDIzPorudzbine);

                    if (indeks_proizvoda == -1)
                    {
                        // proizvod ne postoji
                        return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Nije moguće dodati recenziju! Proizvod je obrisan." });
                    }

                    Proizvod za_recenziju = ProizvodiStorage.Proizvodi[indeks_proizvoda];
                    int kid = KorisniciStorage.Korisnici.FindIndex(p => p.Id.Equals(trenutni.Id));

                    if (kid == -1)
                    {
                        // korisnik je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Nije moguće dodati recenziju! Korisnik je obrisan." });
                    }

                    Korisnik tren = KorisniciStorage.Korisnici[kid];
                    Recenzija nova = new Recenzija(za_recenziju, tren, zahtev.Naslov, zahtev.Sadrzaj, zahtev.Slika);
                    nova.POID = zahtev.PorudzbinaId;
                    nova.KOID = int.Parse(trenutni.Id);
                    nova.PRID = proizvodIDIzPorudzbine;
                    ProizvodiStorage.Proizvodi[indeks_proizvoda].RID.Add(nova.Id); // recenzija za proizvod

                    // cuvanje u in memory
                    RecenzijeStorage.Recenzije.Add(nova);

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
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            // recenziju moze da dodaje samo kupac
            if (ModelState.IsValid)
            {
                // da li postoji porudzbina u listi porudzbina i da li je mozda porudzbina obrisana u medjuvremenu
                int pid = zahtev.PorudzbinaId; // porudzbina za koju je vezana recenzija

                if (PorudzbineStorage.Porudzbine.FirstOrDefault(p => p.Id == pid && p.IsDeleted == false && p.Proizvod.IsDeleted == false) == null)
                {
                    // ne postoji porudzbin
                    return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće izmeniti recenziju! Porudžbina više nije dostupna." });
                }
                else
                {
                    // porudzbina postoji - ako ne postoji recenzija za porudzbinu, ne dozvoliti izmenu, moguce je menjati samo recenzije koje postoje i na cekanju su
                    if (RecenzijeStorage.Recenzije.FirstOrDefault(p => p.POID == pid && p.IsDeleted == false && p.Status == STATUS_RECENZIJE.CEKA) == null)
                    {
                        // vec postoji recenzija
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće izmeniti recenziju! Recenzija nije dostupna za izmenu." });
                    }

                    // ne postoji nijedna recenzija moze se dodati nova
                    int proizvodIDIzPorudzbine = PorudzbineStorage.Porudzbine.FirstOrDefault(p => p.Id == pid && p.IsDeleted == false && p.Proizvod.IsDeleted == false).Proizvod.Id;

                    if (proizvodIDIzPorudzbine == -1)
                    {
                        // proizvod je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće izmeniti recenziju! Proizvod je obrisan." });
                    }

                    int recenziran = ProizvodiStorage.Proizvodi.FindIndex(p => p.Id == proizvodIDIzPorudzbine && !p.IsDeleted);

                    if (recenziran == -1)
                    {
                        // proizvod je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Nije moguće izmeniti recenziju! Proizvod je obrisan." });
                    }

                    int indeks_proizvoda = ProizvodiStorage.Proizvodi.FindIndex(p => p.Id == proizvodIDIzPorudzbine);

                    if (indeks_proizvoda == -1)
                    {
                        // proizvod ne postoji
                        return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Nije moguće izmeniti recenziju! Proizvod je obrisan." });
                    }

                    Proizvod za_recenziju = ProizvodiStorage.Proizvodi[indeks_proizvoda];
                    int kid = KorisniciStorage.Korisnici.FindIndex(p => p.Id.Equals(trenutni.Id));

                    if (kid == -1)
                    {
                        // korisnik je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Nije moguće izmeniti recenziju! Korisnik je obrisan." });
                    }

                    int id_recenzije = RecenzijeStorage.Recenzije.FindIndex(p => p.POID == pid);
                    
                    if (id_recenzije == -1)
                    {
                        // korisnik je obrisan u medjuvremenu
                        return JsonConvert.SerializeObject(new Response { Kod = 18, Poruka = "Nije moguće izmeniti recenziju! Recenzija je obrisana." });
                    }

                    // azuriranje recenzije
                    RecenzijeStorage.Recenzije[id_recenzije].Naslov = zahtev.Naslov;
                    RecenzijeStorage.Recenzije[id_recenzije].SadrzajRecenzije = zahtev.Sadrzaj;
                    RecenzijeStorage.Recenzije[id_recenzije].Slika = zahtev.Slika;

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

        // Metoda za brisanje recenzije
        [HttpPost]
        [Route("BrisanjeRecenzije")]
        public string BrisanjeRecenzije(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (!ModelState.IsValid || !int.TryParse(zahtev.Id, out var id))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste uneli validne podatka za recenziju!" });
            }
            else
            {
                // brisanje recenzije
                int indeks_recenzije = RecenzijeStorage.Recenzije.FindIndex(p => p.POID == id && p.IsDeleted == false);

                if(indeks_recenzije == -1)
                {
                    return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Nije moguće obrisati recenziju!" });
                }

                // Brisanje recenzije
                RecenzijeStorage.Recenzije[indeks_recenzije].IsDeleted = true;

                // azuriranje zapisa u json
                RecenzijeStorage.AzurirajRecenzijeUBazi();
                ProizvodiStorage.AzurirajProizvodeUBazi();

                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Recenzija uspešno obrisana!" });
            }
        }

        // Metoda koja proverava da li postoji recenzija za porudzbinu
        [HttpPost]
        [Route("PostojiRecenzija")]
        public string RecenzijaPostoji(SingleIdRequest zahtev)
        {
            if (!ModelState.IsValid || !int.TryParse(zahtev.Id, out int id))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 41, Poruka = "Niste uneli validne podatke!" });
            }

            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false && (trenutni.Uloga != ULOGA.Kupac || trenutni.Uloga != ULOGA.Administrator))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            Recenzija tmp = RecenzijeStorage.Recenzije.FirstOrDefault(p => p.POID == id && p.IsDeleted == false);
            if (tmp != null)
            {
                // postoji recenzija za datu porudzbinu a da nije odbijena
                return JsonConvert.SerializeObject(new Response { Kod = (ushort)tmp.Status, Poruka = "OK" });
            }
            else
            {
                // ne postoji recenzija za datu porudzbinu
                return JsonConvert.SerializeObject(new Response { Kod = 5, Poruka = "Ne postoji" });
            }
        }

        // Metoda koja vraca recenziju za porudzbinu
        [HttpPost]
        [Route("RecenzijaPoIdPorudzbine")]
        public string RecenzijaPoPorudzbini(SingleIdRequest zahtev)
        {
            if (!ModelState.IsValid || !int.TryParse(zahtev.Id, out int id))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 41, Poruka = "Niste uneli validne podatke!" });
            }

            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false && (trenutni.Uloga != ULOGA.Kupac || trenutni.Uloga != ULOGA.Administrator))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            // samo ne obrisane recenzije koje cekaju mogu biti i izmenjene
            Recenzija tmp = RecenzijeStorage.Recenzije.FirstOrDefault(p => p.POID == id && p.IsDeleted == false && p.Status == STATUS_RECENZIJE.CEKA);
            if (tmp != null)
            {
                // postoji recenzija za datu porudzbinu a da nije odbijena
                return JsonConvert.SerializeObject(tmp);
            }
            else
            {
                // ne postoji recenzija za datu porudzbinu
                return JsonConvert.SerializeObject(new Response { Kod = 5, Poruka = "Ne postoji" });
            }
        }

        // Metoda koja odobrava recenziju - ako je to moguce
        [HttpPost]
        [Route("OdobravanjeRecenzije")]
        public string Odobri(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (!ModelState.IsValid || !int.TryParse(zahtev.Id, out var id))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste uneli validne podatka za recenziju!" });
            }
            else
            {
                // postoji li recenzija recenzije
                int indeks_recenzije = RecenzijeStorage.Recenzije.FindIndex(p => p.POID == id && p.IsDeleted == false && p.Status == STATUS_RECENZIJE.CEKA);

                if (indeks_recenzije == -1)
                {
                    return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Nije moguće odobriti recenziju!" });
                }

                // odobravanje recenzije
                RecenzijeStorage.Recenzije[indeks_recenzije].Status = STATUS_RECENZIJE.ODOBRENA;

                // azuriranje zapisa u json
                RecenzijeStorage.AzurirajRecenzijeUBazi();
                ProizvodiStorage.AzurirajProizvodeUBazi();

                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Recenzija uspešno obrisana!" });
            }
        }

        // Metoda koja odbija recenziju - ako je to moguce
        [HttpPost]
        [Route("OtkazivanjeRecenzije")]
        public string Otkazi(SingleIdRequest zahtev)
        {
            // autentifikacija i autorizacija
            Korisnik trenutni = ((Korisnik)HttpContext.Current.Session["korisnik"]);
            if (trenutni == null || trenutni.IsLoggedIn == false || trenutni.Uloga != ULOGA.Kupac)
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste autentifikovani na platformi ili Vam zahtevana operacija nije dozvoljena!" });
            }

            if (!ModelState.IsValid || !int.TryParse(zahtev.Id, out var id))
            {
                return JsonConvert.SerializeObject(new Response { Kod = 50, Poruka = "Niste uneli validne podatka za recenziju!" });
            }
            else
            {
                // postoji li recenzija
                int indeks_recenzije = RecenzijeStorage.Recenzije.FindIndex(p => p.POID == id && p.IsDeleted == false && p.Status == STATUS_RECENZIJE.CEKA);

                if (indeks_recenzije == -1)
                {
                    return JsonConvert.SerializeObject(new Response { Kod = 12, Poruka = "Nije moguće obrisati recenziju!" });
                }

                // Odbijanje recenzije
                RecenzijeStorage.Recenzije[indeks_recenzije].Status = STATUS_RECENZIJE.ODBIJENA;

                // azuriranje zapisa u json
                RecenzijeStorage.AzurirajRecenzijeUBazi();
                ProizvodiStorage.AzurirajProizvodeUBazi();

                return JsonConvert.SerializeObject(new Response { Kod = 0, Poruka = "Recenzija uspešno obrisana!" });
            }
        }
    }
}
