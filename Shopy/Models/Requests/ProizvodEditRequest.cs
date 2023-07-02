using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class ProizvodEditRequest
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Naziv { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Cena { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public double Kolicina { get; set; }

        [Required]
        [MinLength(20)]
        public string Opis { get; set; }

        [Required]
        public string Slika { get; set; }

        [Required]
        [MinLength(3)]
        public string Grad { get; set; }
    }
}