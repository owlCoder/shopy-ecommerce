using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Mvc;
using WebProjekat.Models;

namespace WebProjekat.Controllers
{
    public class HomeController : Controller
    {
        public static List<Administrator> Administratori { get; set; } = new List<Administrator>();
        public static List<Profesor> Profesori { get; set; } = new List<Profesor>();
        public static List<Student> Studenti { get; set; } = new List<Student>();
        public static List<Ispit> Ispiti { get; set; } = new List<Ispit>();
        public static List<RezultatIspita> RezultatiIspita { get; set; } = new List<RezultatIspita>();

        public static readonly string AppData = HostingEnvironment.MapPath(@"~/App_Data/");

        public ActionResult Index()
        {
            if (Session["uid"] != null && Session["role"] != null)
            {
               if(((string)Session["role"]).Equals("admin"))
                    return RedirectToAction("Index", "Administrator");
               if (((string)Session["role"]).Equals("profesor"))
                    return RedirectToAction("Index", "Profesor");
               if (((string)Session["role"]).Equals("student"))
                    return RedirectToAction("Index", "Student");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Prijava(string korisnickoIme, string lozinka)
        {
            Session["error"] = null;

            if (string.IsNullOrWhiteSpace(korisnickoIme) || string.IsNullOrWhiteSpace(lozinka) || 
                korisnickoIme.Trim().Length <= 0 || lozinka.Trim().Length <= 0)
            {
                Session["error"] = "Niste uneli validne podatke!";
                return RedirectToAction("Index");
            }

            foreach (Administrator a in Administratori)
            {
                if (a.KorisnickoIme.Equals(korisnickoIme) && a.Sifra.Equals(lozinka))
                {
                    Session["uid"] = korisnickoIme;
                    Session["role"] = "admin";
                    return RedirectToAction("Index");
                }
            }

            foreach (Profesor p in Profesori)
            {
                if (p.KorisnickoIme.Equals(korisnickoIme) && p.Sifra.Equals(lozinka))
                {
                    Session["uid"] = korisnickoIme;
                    Session["role"] = "profesor";
                    return RedirectToAction("Index");
                }
            }

            foreach (Student s in Studenti)
            {
                if (s.KorisnickoIme.Equals(korisnickoIme) && s.Sifra.Equals(lozinka))
                {
                    Session["uid"] = korisnickoIme;
                    Session["role"] = "student";
                    return RedirectToAction("Index");
                }
            }

            Session["error"] = "Korisnik ne postoji!";
            return RedirectToAction("Index");
        }

        public ActionResult Odjava()
        {
            Session["uid"] = null;
            Session["role"] = null;
            Session["error"] = null;
            return RedirectToAction("Index");
        }
    }
}