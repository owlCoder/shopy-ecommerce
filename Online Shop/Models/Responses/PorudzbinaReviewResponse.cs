using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Shop.Models.Responses
{
    public class PorudzbinaReviewResponse
    {
        public int Id { get; set; }
        public string Datum { get; set; }
        public string Slika { get; set; }
        public string NazivProizvoda { get; set; }
        public double UkupanIznos { get; set; }
    }
}