using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class PromenaLozinke
    {
        [Required]
        public string StaraLozinka { get; set; }

        [Required]
        public string NovaLozinka { get; set; }
    }
}