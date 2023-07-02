using System;
using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class KorisnikRegistracija
    {
        [Required]
        [MinLength(6)] 
        public string KorisnickoIme { get; set; }

        [Required]
        [MinLength(6)]
        public string Lozinka { get; set; }

        [Required]
        [MinLength(3)]
        public string Ime { get; set; }

        [Required]
        [MinLength(5)]
        public string Prezime { get; set; }

        [Required]
        public string Pol { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime DatumRodjenja { get; set; }
    }
}