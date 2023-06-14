using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online_Shop.Models
{
    public class SearchRequest
    {
        public string Ime { get; set; }

        public string Prezime { get; set; }

        public DateTime Od { get; set;}

        public DateTime Do { get; set; }

        public string Uloga { get; set; }
    }
}