using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online_Shop.Models.Requests
{
    public class RecenzijaEditRequest
    {
        [Required]
        public int RecenzijaId { get; set; }
        [Required]
        public int PorudzbinaId { get; set; }
        [Required]
        public string Naslov { get; set; }
        [Required]
        public string Sadrzaj { get; set; }
        [Required]
        public string Slika { get; set; }
    }
}