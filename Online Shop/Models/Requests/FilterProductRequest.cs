using System.ComponentModel.DataAnnotations;

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