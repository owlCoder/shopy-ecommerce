using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online_Shop.Models
{
    public class FilterProductRequest
    {
        public string Naziv { get; set; }
        public string Grad { get; set; }
        public double MinCena { get; set; }
        public double MaxCena { get; set; }
        [Required]
        public string Sortiranje { get; set; }
    }
}