namespace PR55_2020_Danijel_Jovanovic.Models
{
    public class Recenzija
    {
        public Proizvod Proizvod { get; set; }
        public Korisnik Recenzent { get; set; }
        public string Naslov { get; set; }
        public string SadrzajRecenzije { get; set; }
        public string Slika { get; set; }
    }
}