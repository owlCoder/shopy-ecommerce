using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class KorisnikLogin
    {
        [Required]
        public string KorisnickoIme { get; set; }

        [Required]
        public string Lozinka { get; set; }
    }
}