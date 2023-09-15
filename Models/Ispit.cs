using System;

namespace WebProjekat.Models
{
    public class Ispit
    {
        public int Id { get; set; }
        public Profesor Profesor { get; set; }
        public string Predmet { get; set; }
        public DateTime DatumIVremeOdrzavanja { get; set; }
        public string Ucionica { get; set; }
        public string NazivIspitnogRoka { get; set; }

        public Ispit(int id, Profesor profesor, string predmet, DateTime datumIVremeOdrzavanja, string ucionica, string nazivIspitnogRoka)
        {
            Id = id;
            Profesor = profesor;
            Predmet = predmet;
            DatumIVremeOdrzavanja = datumIVremeOdrzavanja;
            Ucionica = ucionica;
            NazivIspitnogRoka = nazivIspitnogRoka;
        }

        public Ispit() { }
    }
}