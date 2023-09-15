using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProjekat.Models;

namespace WebProjekat.Controllers
{
    public class StudentController : Controller
    {
        public ActionResult Index()
        {
            if (Session["uid"] != null && Session["role"] != null)
            {
                if (((string)Session["role"]).Equals("admin"))
                    return RedirectToAction("Index", "Administrator");
                if (((string)Session["role"]).Equals("profesor"))
                    return RedirectToAction("Index", "Profesor");
                if (((string)Session["role"]).Equals("student"))
                {
                    return View(HomeController.RezultatiIspita.FindAll(r => r.Student.KorisnickoIme.Equals((string)Session["uid"])));
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult PrijavaView()
        {
            if (Session["uid"] != null && Session["role"] != null)
            {
                if (((string)Session["role"]).Equals("admin"))
                    return RedirectToAction("Index", "Administrator");
                if (((string)Session["role"]).Equals("profesor"))
                    return RedirectToAction("Index", "Profesor");
                if (((string)Session["role"]).Equals("student"))
                {
                    return View(HomeController.Ispiti);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Prijava(int iid)
        {
            Session["error"] = null;
            if (Session["uid"] != null && Session["role"] != null)
            {
                if (((string)Session["role"]).Equals("admin"))
                    return RedirectToAction("Index", "Administrator");
                if (((string)Session["role"]).Equals("profesor"))
                    return RedirectToAction("Index", "Profesor");
                if (((string)Session["role"]).Equals("student"))
                {
                    Ispit ispit = HomeController.Ispiti.FirstOrDefault(x => x.Id == iid);
                    Student student;

                    if (ispit != null && ispit.DatumIVremeOdrzavanja > DateTime.Now)
                    {
                        student = HomeController.Studenti.FirstOrDefault(s => s.KorisnickoIme.Equals((string)Session["uid"]));
                        
                        if(student != null)
                        {
                            Ispit prijavljen, polozen;
                            prijavljen = student.PrijavljeniIspiti.FirstOrDefault(i => i.Predmet == ispit.Predmet);
                            polozen = student.PolozeniIspiti.FirstOrDefault(i => i.Predmet == ispit.Predmet);

                            if (prijavljen == null && polozen == null)
                            {
                                HomeController.Studenti[HomeController.Studenti.FindIndex(s => s.KorisnickoIme.Equals(student.KorisnickoIme))].PrijavljeniIspiti.Add(ispit);
                                HomeController.RezultatiIspita.Add(new RezultatIspita(HomeController.RezultatiIspita.Count + 1, ispit, student));
                                Database.JsonSerializer.Serialize();
                                return RedirectToAction("Index", "Student");
                            }
                            else
                                Session["error"] = "Ispit je počeo ili je već prijavljen!";
                        }
                    }
                    else
                    {
                        Session["error"] = "Ispit je počeo ili je već prijavljen!";
                        return RedirectToAction("Index", "Student");
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}