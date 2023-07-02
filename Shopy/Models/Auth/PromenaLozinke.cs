using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class PromenaLozinke
    {
        [Required]
        [MinLength(6)]
        public string StaraLozinka { get; set; }

        [Required]
        [MinLength(6)]
        public string NovaLozinka { get; set; }
    }
}