var uloga = -1;
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

                uloga = data.Uloga;
            })
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

// Odjava korisnika
jQuery(function () {
    $("#divgreske").addClass('d-none');

    $("#odjava").on('click', function () {
        // ajax poziv ka api-ju za vrsenje registracije
        $.ajax({
            url: "/api/auth/Odjava",
            type: "GET",
            async: false,
            cache: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response != null) {
                    if (JSON.parse(response).Kod === 0) // korisnik se odjavio uspesno
                    {
                        window.location.href = "Index.html";
                        return false;
                    }
                }
            }
        });
    });
});