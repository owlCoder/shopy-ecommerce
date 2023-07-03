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

                if (uloga !== 2) {
                    // nije pitanju administrator - povratak na pocetnu stranicu
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

// get metoda za dobijanje listi svih korisnika
$.ajax({
    url: "/api/users/ListaKorisnika",
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
                $.each(data, function (key, k) {
                    // dodavanje reda po red
                    var uloga = "Auth";

                    if (k.Uloga == 0) uloga = "Kupac";
                    if (k.Uloga == 1) uloga = "Prodavac";
                    if (k.Uloga == 2) uloga = "Administrator";

                    $("#korisnici > tbody").append('<tr><td>' + k.KorisnickoIme + '</td>' +
                        '<td>' + k.Ime + '</td>' +
                        '<td>' + k.Prezime + '</td>' +
                        '<td>' + k.Pol + '</td>' +
                        '<td>' + k.Email + '</td>' +
                        '<td>' + new Date(k.DatumRodjenja).toLocaleDateString("en-GB") + '</td>' +
                        '<td><b>' + uloga.toUpperCase() + '</b></td>' +
                        '<td class="d-flex justify-content-center"><a class="btn btn-dark" href="Izmena.html?id=' + k.KorisnickoIme + '">&#128393;</a>&emsp;' +
                        '<a class="btn btn-danger" href="Brisanje.html?id=' + k.KorisnickoIme + '">&#128473;</a></td>' +
                        '</tr>');
                });
            });
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

    $("#ponistibtn").on('click', function () {
        $("#divgreske").addClass('d-none');

        // isprazni redove
        $('#korisnici tbody').empty();

        // popuni svim podacima
        // get metoda za dobijanje listi svih korisnika
        $.ajax({
            url: "/api/users/ListaKorisnika",
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
                        $.each(data, function (key, k) {
                            // dodavanje reda po red
                            var uloga = "Auth";

                            if (k.Uloga == 0) uloga = "Kupac";
                            if (k.Uloga == 1) uloga = "Prodavac";
                            if (k.Uloga == 2) uloga = "Administrator";

                            $("#korisnici > tbody").append('<tr><td>' + k.KorisnickoIme + '</td>' +
                                '<td>' + k.Ime + '</td>' +
                                '<td>' + k.Prezime + '</td>' +
                                '<td>' + k.Pol + '</td>' +
                                '<td>' + k.Email + '</td>' +
                                '<td>' + new Date(k.DatumRodjenja).toLocaleDateString("en-GB") + '</td>' +
                                '<td><b>' + uloga.toUpperCase() + '</b></td>' +
                                '<td class="d-flex justify-content-center"><a class="btn btn-dark" href="Izmena.html?id=' + k.KorisnickoIme + '">&#128393;</a>&emsp;' +
                                '<a class="btn btn-danger" href="Brisanje.html?id=' + k.KorisnickoIme + '">&#128473;</a></td>' +
                                '</tr>');
                        });
                    });
                }
            }
        });

        $("od").val() = "";
        $("do").val() = "";

        return false; // prevent ajax page reload
    });

    // dugme pretraga
    $("#pretragabtn").on('click', function () {
        // isprazni redove
        $('#korisnici tbody').empty();
        $("#divgreske").addClass('d-none');

        // uneo je okej datume, pokupimo vrednosti polja i onda ajax call
        var ime = $("#ime").val();
        var prezime = $("#prezime").val();
        var dmin = $("#od").val();
        var dmax = $("#do").val();
        var defDate = new Date("1900-01-01").toISOString();
        var minDate = new Date("1900-01-01").toISOString();
        var maxDate = new Date("1900-01-01").toISOString();
        var uloga = $("#uloga").val();

        if (dmin !== "") {
            dmin = Date.parse(dmin.split(".").reverse().join("-"));
            minDate = new Date(dmin).toISOString();
        }

        if (dmax !== "") {
            dmax = Date.parse(dmax.split(".").reverse().join("-"));
            maxDate = new Date(dmax).toISOString();
        }

        if (minDate === defDate && maxDate !== defDate) {
            // desila se greska - prikazi je
            $("#divgreske").removeClass('d-none');
            $("#greske").text("Unesite početni datum za pretragu!");
            return false;
        }

        if (minDate !== defDate && maxDate === defDate) {
            // desila se greska - prikazi je
            $("#divgreske").removeClass('d-none');
            $("#greske").text("Unesite krajnji datum za pretragu!");
            return false;
        }

        // poziv ka apiju za pretragu
        $.ajax({
            url: "/api/users/PretragaKorisnika",
            type: "POST",
            data: JSON.stringify({
                Ime: ime,
                Prezime: prezime,
                Od: minDate,
                Do: maxDate,
                Uloga: uloga
            }),
            async: false,
            cache: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                var data = JSON.parse(response);

                if (data.length > 0) {
                    $("#divgreske").removeClass('d-none');
                    $("#greske").text("Korisnici koji ispunjavaju uneti kriterijum su izlistani u tabeli ispod!");

                    // popuni tabelu
                    $.each(data, function (key, k) {
                        // dodavanje reda po red
                        var uloga = "Auth";

                        if (k.Uloga == 0) uloga = "Kupac";
                        if (k.Uloga == 1) uloga = "Prodavac";
                        if (k.Uloga == 2) uloga = "Administrator";

                        $("#korisnici > tbody").append('<tr><td>' + k.KorisnickoIme + '</td>' +
                            '<td>' + k.Ime + '</td>' +
                            '<td>' + k.Prezime + '</td>' +
                            '<td>' + k.Pol + '</td>' +
                            '<td>' + k.Email + '</td>' +
                            '<td>' + new Date(k.DatumRodjenja).toLocaleDateString("en-GB") + '</td>' +
                            '<td><b>' + uloga.toUpperCase() + '</b></td>' +
                            '<td class="d-flex justify-content-center"><a class="btn btn-dark" href="Izmena.html?id=' + k.KorisnickoIme + '">&#128393;</a>&emsp;' +
                            '<a class="btn btn-danger" href="Brisanje.html?id=' + k.KorisnickoIme + '">&#128473;</a></td>' +
                            '</tr>');
                    });

                    return false;
                }
                else {
                    // desila se greska - prikazi je
                    $("#divgreske").removeClass('d-none');
                    $("#greske").text("Nema korisnika koji ispunjavaju uneti kriterijum!");
                    return false;
                }
            }
        });

        return false; // prevent ajax page reload
    });

    // sortiranje korisnika
    $('#sort').on('change', function () {
        var Id = this.value; // trenutni id u padajecem meniju

        $("#divgreske").addClass('d-none');

        // isprazni redove
        $('#korisnici tbody').empty();

        // popuni svim podacima
        // get metoda za dobijanje listi svih korisnika
        $.ajax({
            url: "/api/users/SortiranjeKorisnika",
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

                if (data.length > 0) {
                    $(function () {
                        // popuni tabelu
                        $.each(data, function (key, k) {
                            // dodavanje reda po red
                            var uloga = "Auth";

                            if (k.Uloga == 0) uloga = "Kupac";
                            if (k.Uloga == 1) uloga = "Prodavac";
                            if (k.Uloga == 2) uloga = "Administrator";

                            $("#korisnici > tbody").append('<tr><td>' + k.KorisnickoIme + '</td>' +
                                '<td>' + k.Ime + '</td>' +
                                '<td>' + k.Prezime + '</td>' +
                                '<td>' + k.Pol + '</td>' +
                                '<td>' + k.Email + '</td>' +
                                '<td>' + new Date(k.DatumRodjenja).toLocaleDateString("en-GB") + '</td>' +
                                '<td><b>' + uloga.toUpperCase() + '</b></td>' +
                                '<td class="d-flex justify-content-center"><a class="btn btn-dark" href="Izmena.html?id=' + k.KorisnickoIme + '">&#128393;</a>&emsp;' +
                                '<a class="btn btn-danger" href="Brisanje.html?id=' + k.KorisnickoIme + '">&#128473;</a></td>' +
                                '</tr>');
                        });
                    });
                }
            }
        });
    });
});