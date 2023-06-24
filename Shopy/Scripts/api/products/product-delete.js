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

                if (uloga !== 1 && uloga !== 2) {
                    // nije pitanju ni administrator ni prodavac - povratak na pocetnu stranicu
                    window.location.href = "MojProfil.html";
                }

                // ako nema id u url, redirect
                let searchParams = new URLSearchParams(window.location.search)
                if (searchParams.has('id') === false) { // nema id, pokusaj hardcore pristupa, redirect
                    window.location.href = "Index.html";
                }
                Id = searchParams.get('id');
                $.ajax({
                    url: "/api/products/ProizvodPoId",
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

                            // ne postoji proizvod, obrisan je u medjuvremenu
                            if (uloga === 1 && data.Id === 0) {
                                window.location.href = "MojiProizvodi.html";
                                return false;
                            }
                            else if (uloga === 2 && data.Id === 0) {
                                window.location.href = "ListaProizvoda.html";
                                return false;
                            }

                            // prodavac ne moze da menja ili brise proizvode koji nisu dostupni
                            if (uloga === 1 && data.Status === false) {
                                window.location.href = "MojiProizvodi.html?msg=Odabrani proizvod nije moguće izmeniti jer više nije DOSTUPAN!";
                                return false;
                            }

                            // brisanje proizvoda
                            // obrisi proizvod po id
                            $.ajax({
                                url: "/api/products/BrisanjeProizvoda",
                                type: "POST",
                                data: JSON.stringify({
                                    id: Id
                                }),
                                async: false,
                                cache: false,
                                dataType: "json",
                                contentType: "application/json; charset=utf-8",
                                success: function (response) {
                                    var data = JSON.parse(response);

                                    if (uloga === 1) {
                                        // u pitanju je prodavac, on se preusmerava na stranicu svojih proizvoda
                                        window.location.href = "MojiProizvodi.html?msg=" + data.Poruka;
                                        return false;
                                    }
                                    else if (uloga == 2) {
                                        // u pitanju je administrator, on se preusmerava na stranicu svih proizvoda
                                        window.location.href = "ListaProizvoda.html?msg=" + data.Poruka;
                                        return false;
                                    }
                                }
                            });
                        }
                    }
                });
            });
        }

    }
});