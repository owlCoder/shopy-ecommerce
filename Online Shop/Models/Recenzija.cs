using Newtonsoft.Json;
using Online_Shop.Storage;

namespace Online_Shop.Models
{
    public class Recenzija
    {
        public int Id { get; set; }
        public Proizvod Proizvod { get; set; }
        public Korisnik Recenzent { get; set; }
        public string Naslov { get; set; }
        public string SadrzajRecenzije { get; set; }
        public string Slika { get; set; }
        public bool Odobrena { get; set; }
        public bool IsDeleted { get; set; }

        public Recenzija()
        {
            // Prazan konstruktor zbog serijalizacije
        }

        // Konstruktor za kreiranje recenzije
        public Recenzija(Proizvod proizvod, Korisnik recenzent, string naslov, string sadrzajRecenzije, string slika)
        {
            Id = RecenzijeStorage.Recenzije.Count + 1;
            Proizvod = proizvod;
            Recenzent = recenzent;
            Naslov = naslov;
            SadrzajRecenzije = sadrzajRecenzije;
            Slika = slika;
            Odobrena = false; // prvo je admin mora odobriti
            IsDeleted = false;
        }

        // Konstruktor za json serializer
        [JsonConstructor]
        public Recenzija(int id, Proizvod proizvod, Korisnik recenzent, string naslov, string sadrzajRecenzije, string slika, bool odobrena, bool isDeleted)
        {
            Id = id;
            Proizvod = proizvod;
            Recenzent = recenzent;
            Naslov = naslov;
            SadrzajRecenzije = sadrzajRecenzije;
            Slika = slika;
            Odobrena = odobrena;
            IsDeleted = isDeleted;
        }
    }
}