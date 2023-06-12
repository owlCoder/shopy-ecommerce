using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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