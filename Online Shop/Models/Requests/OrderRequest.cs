using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online_Shop.Models
{
    public class OrderRequest
    {
        [Required]
        public int Id{ get; set; }
        [Required]
        public double Kolicina { get; set; }
    }
}