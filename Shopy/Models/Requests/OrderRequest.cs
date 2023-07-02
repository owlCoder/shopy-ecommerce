using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class OrderRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public double Kolicina { get; set; }
    }
}