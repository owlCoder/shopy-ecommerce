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
                $("#pl").remove();

                // prikazi meni za nalog
                $('#poruka').text("Dobrodošli nazad, " + data.KorisnickoIme);

                // Prikazi samo dugmice vezane za korisnika koji ima datu ulogu
                var uloga = data.Uloga;

                if (uloga !== 0) {
                    // nije pitanju kupac - povratak na pocetnu stranicu
                    window.location.href = "MojProfil.html";
                }
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

// get metoda za dobijanje listi svih proizvoda za korisnika
$.ajax({
    url: "/api/users/OmiljeniProizvodiPoKorisniku",
    type: "GET",
    async: false,
    cache: false,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    success: function (response) {
        var data = JSON.parse(response);

        if (data.length > 0) {
            $(function () {
                // popuni tabelu
                $("#nema").addClass('d-none');

                $.each(data, function (key, k) {
                    // dodavanje reda po red
                    var kupac_dodaj_naruci = "";

                    if (k.Status)
                        kupac_dodaj_naruci = '<a href="Proizvod.html?id=' + k.Id + '" class="btn btn-dark">Pogledajte proizvod</a>';
                    else
                        kupac_dodaj_naruci = '<a href="#" class="btn btn-outline-danger disabled">Proizvod nije dostupan</a>';

                    $("#proizvodi").append(
                        '<div class="card mb-4 border-success">' +
                        '<div class="card-body">' +
                        '<div class="row justify-content-between">' +
                        '<div class="col-2">' +
                        '<img alt="proizvod" class="card-img-top rounded-circle" width="150" height="150" src="/Uploads/' + k.Slika + '" />' +
                        '</div>' +
                        '<div class="col-10">' +
                        '<div class="d-flex justify-content-between">' +
                        '<span class="h5 card-title justify-content-start">' + k.Naziv + '</span>' +
                        '<span class="h5 card-title text-success fw-semibold justify-content-end">' + k.Grad + '</span>' +
                        '</div>' +
                        '<h6 class="card-subtitle mb-2 text-muted">' + k.Cena + ' RSD</h6>' +
                        '<p class="card-text">' + k.Opis + '</p>' +
                        kupac_dodaj_naruci +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>');
                });
            });

            return false;
        }
        else {
            // nema jos uvek nije proizvod
            $("#nema").removeClass("d-none");

            return false;
        }
    }
});

// Odjava korisnika
jQuery(function () {
    $("#divgreske").addClass('d-none');

    // proveri da li je na stranicu vraceno sa Brisanja ili izmene
    let searchParams = new URLSearchParams(window.location.search)
    if (searchParams.has('msg')) {
        // ima msg kao url parametar prikazi primljenu poruku
        $("#divgreske").removeClass('d-none');
        $("#greske").text(searchParams.get('msg'));
    }
});