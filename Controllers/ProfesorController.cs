using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebProjekat.Models;

namespace WebProjekat.Controllers
{
    public class ProfesorController : Controller
    {
        public ActionResult Index()
        {
            if (Session["uid"] != null && Session["role"] != null)
            {
                if (((string)Session["role"]).Equals("admin"))
                    return RedirectToAction("Index", "Administrator");
                if (((string)Session["role"]).Equals("profesor"))
                {
                    return View(HomeController.Ispiti.FindAll(p => p.Profesor.KorisnickoIme.Equals((string)(Session["uid"]))));
                }    
                if (((string)Session["role"]).Equals("student"))
                    return RedirectToAction("Index", "Student");
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult DodavanjeView()
        {
            if (Session["uid"] != null && Session["role"] != null && ((string)Session["role"]).Equals("profesor"))
            {
                List<string> predmeti = HomeController.Profesori.FirstOrDefault(p => p.KorisnickoIme.Equals(((string)Session["uid"]))).Predmeti;
                return View(predmeti);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult OcenivanjeView() 
        {
            if (Session["uid"] != null && Session["role"] != null && ((string)Session["role"]).Equals("profesor"))
                return View(HomeController.RezultatiIspita.FindAll(p => p.Ispit.Profesor.KorisnickoIme.Equals((string)(Session["uid"]))));
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Dodaj(Ispit ni)
        {
            Session["error"] = null;
            if (Session["uid"] != null && Session["role"] != null && ((string)Session["role"]).Equals("profesor") && ni != null)
            {
                Ispit postoji = HomeController.Ispiti.FirstOrDefault(i => i.Ucionica.Equals(ni.Ucionica) &&
                                                                          i.NazivIspitnogRoka.Equals(ni.NazivIspitnogRoka) &&
                                                                          i.DatumIVremeOdrzavanja == ni.DatumIVremeOdrzavanja &&
                                                                          i.Profesor.KorisnickoIme.Equals(ni.Profesor.KorisnickoIme) &&
                                                                          i.Predmet == ni.Predmet);

                if(ni.Ucionica != null && ni.NazivIspitnogRoka != null && 
                   ni.DatumIVremeOdrzavanja >= DateTime.Now && ni.Ucionica.Trim().Length > 0 && 
                   ni.NazivIspitnogRoka.Trim().Length > 0 && postoji == null)
                {
                    ni.Profesor = HomeController.Profesori.FirstOrDefault(p => p.KorisnickoIme.Equals(((string)Session["uid"])));

                    if (HomeController.Ispiti.Count == 0)
                        ni.Id = 1;
                    else
                        ni.Id = HomeController.Ispiti.LastOrDefault().Id + 1;

                    HomeController.Ispiti.Add(ni);
                    HomeController.Profesori.FirstOrDefault(p => p.KorisnickoIme.Equals(((string)Session["uid"]))).Ispiti.Add(ni);
                    Database.JsonSerializer.Serialize();
                }
                else
                {
                    Session["error"] = "Niste uneli validne podatke za ispit ili je isti već kreiran!";
                }

                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Oceni(int iid, int o)
        {
            Session["error"] = null;
            if (Session["uid"] != null && Session["role"] != null && ((string)Session["role"]).Equals("profesor") && iid > 0 && o >= 5 && o <= 10)
            {
                int rez = HomeController.RezultatiIspita.FindIndex(r => r.Id == iid);

                if(rez != -1)
                {
                    RezultatIspita tmp = HomeController.RezultatiIspita[rez];
                    int std = HomeController.Studenti.FindIndex(s => s.KorisnickoIme.Equals(tmp.Student.KorisnickoIme));

                    if(std != -1 && tmp != null) 
                    {
                        try
                        {
                            int postoji_prijava = HomeController.Studenti[std].PrijavljeniIspiti.FindIndex(p => p.Id == tmp.Ispit.Id);

                            if(postoji_prijava != -1)
                            {
                                HomeController.RezultatiIspita[rez].Ocena = o; // upis ocene
                                HomeController.Studenti[std].PrijavljeniIspiti.RemoveAt(postoji_prijava);

                                if (o == 5) HomeController.Studenti[std].NepolozeniIspiti.Add(tmp.Ispit);
                                else HomeController.Studenti[std].PolozeniIspiti.Add(tmp.Ispit);
                                Database.JsonSerializer.Serialize();
                            }
                            else
                                Session["error"] = "Nije moguće oceniti ispit!";
                        }
                        catch { }
                    }
                    else
                        Session["error"] = "Nije moguće oceniti ispit!";
                }

                return RedirectToAction("OcenivanjeView");
            }
            else
                return RedirectToAction("Index", "Home");
        }
    }
}