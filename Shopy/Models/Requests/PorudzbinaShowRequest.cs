using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online_Shop.Models.Requests
{
    public class PorudzbinaShowRequest
    {
        [Required]
        public int Id { get; set; }
    }
}