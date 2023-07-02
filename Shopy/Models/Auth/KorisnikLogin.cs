using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class KorisnikLogin
    {
        [Required]
        [MinLength(6)]
        public string KorisnickoIme { get; set; }

        [Required]
        [MinLength(6)]
        public string Lozinka { get; set; }
    }
}