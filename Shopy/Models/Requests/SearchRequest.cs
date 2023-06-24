using System;

namespace Online_Shop.Models
{
    public class SearchRequest
    {
        public string Ime { get; set; }

        public string Prezime { get; set; }

        public DateTime Od { get; set; }

        public DateTime Do { get; set; }

        public string Uloga { get; set; }
    }
}