using System;
using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class KorisnikRegistracija
    {
        [Required]
        public string KorisnickoIme { get; set; }

        [Required]
        public string Lozinka { get; set; }

        [Required]
        public string Ime { get; set; }

        [Required]
        public string Prezime { get; set; }

        [Required]
        public string Pol { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime DatumRodjenja { get; set; }
    }
}