using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class ProizvodEditRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Naziv { get; set; }

        [Required]
        public double Cena { get; set; }

        [Required]
        public double Kolicina { get; set; }

        [Required]
        public string Opis { get; set; }

        [Required]
        public string Slika { get; set; }

        [Required]
        public string Grad { get; set; }
    }
}