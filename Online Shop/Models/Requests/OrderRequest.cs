using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class OrderRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public double Kolicina { get; set; }
    }
}