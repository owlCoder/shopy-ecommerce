﻿using Online_Shop.Models;
using Online_Shop.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace Online_Shop
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Ucitavanje podataka o korisnicima
            KorisniciStorage.UcitajKorisnike();

            // Ucitavanje podataka o proizvodima
            ProizvodiStorage.UcitajProizvode();

            // Registracija filtera i ruta
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // Podrška za sesije u okviru poziva rest servisa
        public override void Init()
        {
            PostAuthenticateRequest += ApiRequestHandler;
            base.Init();
        }

        // Sesije, url zahteva /api/ kao pocetni parametar
        void ApiRequestHandler(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Url.AbsolutePath.StartsWith("/api/"))
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }
    }
}
