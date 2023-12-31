﻿var uloga;
var Id;
var STARA_SLIKA;

// ako je ulogovan, prebaci ga na index - slucaj da ide na back opciju
$.ajax({
    url: "/api/auth/Ulogovan",
    type: "GET",
    async: false,
    cache: false,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    success: function (response) {
        if (response != null) {
            if (JSON.parse(response).Kod !== 0) // korisnik je ulogovan, ne moze da se registruje
            {
                window.location.href = "Index.html";
            }
        }
    }
});

// ako je ulogovan, prebaci prikazi meni u zavisnoti od uloge
$.ajax({
    url: "/api/auth/AuthKorisnik",
    type: "GET",
    async: false,
    cache: false,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    success: function (response) {
        var data = JSON.parse(response);
        if (data.KorisnickoIme !== "") {
            // prikazi podatke i promeni meni
            // sakri dugmice za login i register
            $(function () {
                // Prikazi samo dugmice vezane za korisnika koji ima datu ulogu
                uloga = data.Uloga;

                if (uloga !== 2) {
                    // nije pitanju admin - povratak na pocetnu stranicu
                    window.location.href = "MojProfil.html";
                }

                // id iz url
                let searchParams = new URLSearchParams(window.location.search)
                if (!searchParams.has('id')) window.location.href = "Index.html";
                Id = searchParams.get('id');

                // popuni informacije o porudzbini za koju se dodaje recenzija
                $.ajax({
                    url: "/api/reviews/OdobravanjeRecenzije",
                    type: "POST",
                    data: JSON.stringify({
                        Id: Id
                    }),
                    async: false,
                    cache: false,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        var data = JSON.parse(response);
                        window.location.href = "ListaPorudzbina.html?msg= " + data.Poruka;
                    }
                });
            });
        }
    }
});