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
                var uloga = data.Uloga;

                if (uloga !== 0) {
                    // nije pitanju ni kupac
                    window.location.href = "MojProfil.html";
                }

                // ako nema id u url, redirect
                let searchParams = new URLSearchParams(window.location.search)
                if (searchParams.has('id') === false) { // nema id, pokusaj hardcore pristupa, redirect
                    window.location.href = "Index.html";
                }
                Id = searchParams.get('id');
                $.ajax({
                    url: "/api/reviews/BrisanjeRecenzije",
                    type: "POST",
                    data: JSON.stringify({
                        Id: Id
                    }),
                    async: false,
                    cache: false,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        if (response != null) {
                            var data = JSON.parse(response);

                            window.location.href = "MojePorudzbine.html?msg=" + data.Poruka;
                        }
                    }
                });
            });
        }

    }
});