using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebProjekat.Models;

namespace WebProjekat.Controllers
{
    public class AdministratorController : Controller
    {
        public ActionResult Index()
        {
            if (Session["uid"] != null && Session["role"] != null)
            {
                if (((string)Session["role"]).Equals("admin"))
                    return View(HomeController.Studenti);
                if (((string)Session["role"]).Equals("profesor"))
                    return RedirectToAction("Index", "Profesor");
                if (((string)Session["role"]).Equals("student"))
                    return RedirectToAction("Index", "Student");
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult DodavanjeView()
        {
            Session["error"] = null;
            if (Session["uid"] != null && Session["role"] != null)
            {
                if (((string)Session["role"]).Equals("admin"))
                    return View();
                if (((string)Session["role"]).Equals("profesor"))
                    return RedirectToAction("Index", "Profesor");
                if (((string)Session["role"]).Equals("student"))
                    return RedirectToAction("Index", "Student");
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult IzmenaView(string uid)
        {
            if (Session["uid"] != null && Session["role"] != null && uid != null && uid.Trim().Length > 0)
            {
                if (((string)Session["role"]).Equals("admin"))
                {
                    foreach(Student s in HomeController.Studenti)
                    {
                        if(s.KorisnickoIme.Equals(uid.Trim()))
                        {
                            return View(s);
                        }
                    }

                    Session["error"] = "Student ne postoji!";
                    return RedirectToAction("Index");
                }
                if (((string)Session["role"]).Equals("profesor"))
                    return RedirectToAction("Index", "Profesor");
                if (((string)Session["role"]).Equals("student"))
                    return RedirectToAction("Index", "Student");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Dodavanje(Student ns)
        {
            if (Session["uid"] != null && Session["role"] != null && ((string)Session["role"]).Equals("admin"))
            {
                Session["error"] = null;
                Student sf = HomeController.Studenti.FirstOrDefault(s => s.KorisnickoIme.Equals(ns.KorisnickoIme) || s.Email.Equals(ns.Email) || s.BrojIndeksa.Equals(ns.BrojIndeksa));

                if (Validacija(ns) && sf == null)
                {
                    HomeController.Studenti.Add(ns);
                    Database.JsonSerializer.Serialize();
                    Session["error"] = "Student je uspešno dodat!";
                }
                else
                    Session["error"] = "Nisu uneti validni podaci ili student sa unetim podacima već postoji!";

                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Izmena(Student ns)
        {
            if (Session["uid"] != null && Session["role"] != null && ((string)Session["role"]).Equals("admin"))
            {
                Session["error"] = null;
                Student sf = HomeController.Studenti.FirstOrDefault(s => s.KorisnickoIme.Equals(ns.KorisnickoIme) == false && s.Email.Equals(ns.Email));

                if (Validacija(ns) && sf == null)
                {
                    for(int i = 0; i < HomeController.Studenti.Count; i++)
                    {
                        if (HomeController.Studenti[i].KorisnickoIme.Equals(ns.KorisnickoIme))
                        {
                            HomeController.Studenti[i].Sifra = ns.Sifra;
                            HomeController.Studenti[i].Ime = ns.Ime;
                            HomeController.Studenti[i].Prezime = ns.Prezime;
                            HomeController.Studenti[i].DatumRodjenja = ns.DatumRodjenja;
                            HomeController.Studenti[i].Email = ns.Email;
                            Database.JsonSerializer.Serialize();
                            Session["error"] = null;
                        }
                    }
                }
                else
                    Session["error"] = "Nisu uneti validni podaci ili student sa unetim podacima ne postoji!";

                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult Brisanje(string uid)
        {
            if (Session["uid"] != null && Session["role"] != null && ((string)Session["role"]).Equals("admin"))
            {
                Session["error"] = null;
                int sf = HomeController.Studenti.FindIndex(s => s.KorisnickoIme.Equals(uid));

                if (sf != -1)
                {
                    HomeController.Studenti.RemoveAt(sf);
                    HomeController.RezultatiIspita.RemoveAll(r => r.Student.KorisnickoIme.Equals(uid));
                    Database.JsonSerializer.Serialize();
                    Session["error"] = "Student je uspešno obrisan!";
                }
                
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index", "Home");
        }

        private bool Validacija(Student ns)
        {
            if (ns == null) return false;
            if(ns.KorisnickoIme == null || ns.KorisnickoIme.Trim().Length <= 0) return false;
            if(ns.BrojIndeksa == null || ns.BrojIndeksa.Trim().Length <= 0) return false;
            if(ns.Ime == null || ns.Ime.Trim().Length <= 0) return false;
            if(ns.Prezime == null || ns.Prezime.Trim().Length <= 0) return false;
            if(ns.Sifra == null || ns.Sifra.Trim().Length <= 0) return false;
            if(ns.Email == null || ns.Email.Trim().Length <= 0) return false;
            if(ns.DatumRodjenja >= DateTime.Now) return false;
            if (Regex.IsMatch(ns.Email, @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$") == false) return false;
            return true;
        }
    }
}