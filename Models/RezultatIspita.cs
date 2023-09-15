namespace WebProjekat.Models
{
    public class RezultatIspita
    {
        public int Id { get; set; }
        public Ispit Ispit { get; set; }
        public Student Student { get; set; }
        public int Ocena { get; set; }

        public RezultatIspita(int id, Ispit ispit, Student student)
        {
            Id = id;
            Ispit = ispit;
            Student = student;
            Ocena = 0;
        }
        public RezultatIspita() { }
    }
}