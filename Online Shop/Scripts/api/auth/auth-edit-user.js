var STARI_DATUM = "";
var Id = "";

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

jQuery(function () {
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
                    $("#pl").remove();

                    // prikazi meni za nalog
                    $('#poruka').text("Dobrodošli nazad, " + data.KorisnickoIme);

                    // Prikazi samo dugmice vezane za korisnika koji ima datu ulogu
                    var uloga = data.Uloga;

                    if (uloga !== 2) {
                        // nije pitanju administrator - povratak na zadatu stranicu
                        window.location.href = "MojProfil.html";
                    }

                    // id iz url
                    let searchParams = new URLSearchParams(window.location.search)
                    Id = searchParams.get('id');

                    // obrisi korisnika po id
                    $.ajax({
                        url: "/api/users/GetKorisnikById",
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

                            // upisivanje u formu fetchovanih podataka
                            $("#korisnickoime").val(data.KorisnickoIme);
                            $("#ime").val(data.Ime);
                            $("#prezime").val(data.Prezime);
                            $("#pol").val(data.Pol);
                            $("#email").val(data.Email);

                            let today = new Date(data.DatumRodjenja);

                            let day = (today.getDate());
                            let month = ('0' + (today.getMonth() + 1)).slice(-2);
                            let year = today.getFullYear();

                            let currentDate = `${day}/${month}/${year}`;
                            $("#trenutni").text("Datum rođenja: " + currentDate + " (trenutni)");
                            STARI_DATUM = currentDate;
                            return false;
                        }
                    });
                });
            }
            else {
                $(function () {
                    // korisnik nije ulogovan - prikazi login i register
                    $("#pl").show();

                    // sakri meni za nalog
                    $('#profil').addClass('d-none');
                });
            }
        }
    });
});