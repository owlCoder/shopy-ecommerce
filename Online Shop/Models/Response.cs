using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Shop.Models
{
    public class Response
    {
        public ushort Kod { get; set; }
        public string Poruka { get; set; }

        public Response() 
        {
            // Prazan konstruktor
        }
    }
}