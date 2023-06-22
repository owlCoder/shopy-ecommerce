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
        public STATUS_RECENZIJE Status { get; set; }
        public int POID { get; set; }
        public int PRID { get; set; }
        public int KOID { get; set; }
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
            IsDeleted = false;
            Status = STATUS_RECENZIJE.CEKA; // kada se kreira ona je u stanju cekanja
        }

        // Konstruktor za json serializer
        [JsonConstructor]
        public Recenzija(int id, Proizvod proizvod, Korisnik recenzent, string naslov, string sadrzajRecenzije, string slika, bool isDeleted, STATUS_RECENZIJE status)
        {
            Id = id;
            Proizvod = proizvod;
            Recenzent = recenzent;
            Naslov = naslov;
            SadrzajRecenzije = sadrzajRecenzije;
            Slika = slika;
            IsDeleted = isDeleted;
            Status = status;
        }
    }
}