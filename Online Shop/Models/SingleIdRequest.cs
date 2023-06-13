using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online_Shop.Models
{
    public class SingleIdRequest
    {
        [Required]
        public string Id { get; set; }
    }
}