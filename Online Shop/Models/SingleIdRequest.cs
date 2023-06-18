using System.ComponentModel.DataAnnotations;

namespace Online_Shop.Models
{
    public class SingleIdRequest
    {
        [Required]
        public string Id { get; set; }
    }
}