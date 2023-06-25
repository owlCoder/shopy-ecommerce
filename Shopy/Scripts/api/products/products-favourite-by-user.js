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

                if (uloga !== 1) {
                    // nije pitanju prodavac - povratak na pocetnu stranicu
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
    url: "/api/products/ListaProizvoda",
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
                    $("#proizvodi > tbody").append('<tr><td class="align-middle">' + k.Naziv.substring(0, 20) + '</td>' +
                        '<td><img alt="proizvod" width="80" height="80" src="/Uploads/' + k.Slika + '" /></td>' +
                        '<td class="align-middle">' + k.Kolicina + '</td>' +
                        '<td class="align-middle">' + k.Cena + '&nbsp;RSD</td>' +
                        '<td class="align-middle">' + k.Opis.substring(0, 20) + '</td>' +
                        '<td class="align-middle">' + new Date(k.DatumPostavljanjaProizvoda).toLocaleDateString("en-GB") + '</td>' +
                        '<td class="align-middle">' + k.Grad.substring(0, 20) + '</td>' +
                        '<td class="align-middle"><b>' + (k.Status ? "NA&nbsp;STANJU" : "NEDOSTUPAN") + '</b></td>' +
                        '<td class="align-middle"><div class="align-middle justify-content-center">' +
                        '&emsp;<a class="btn btn-dark" href="IzmenaProizvoda.html?id=' + k.Id + '">Izmena</a>&emsp;' +
                        '<a class="btn btn-danger" href="BrisanjeProizvoda.html?id=' + k.Id + '">Brisanje</a></div></td>' +
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

    // dugme pretraga
    $("#dostupni, #sort").on('change', function () {
        // isprazni redove
        $('#proizvodi tbody').empty();
        $("#divgreske").addClass('d-none');

        // da li je cekirano = dostupni
        // nije cekirano, svi
        // NAPOMENA: proveriti da li se sortiraju SVI ili SAMO DOSTUPNI PROIZVODI
        // checkbox -> dostupni
        var combo = $("#sort").val().toString();
        var primenjeno = $("#dostupni").is(":checked") ? "1;" : "0;";
        primenjeno += combo;

        // poziv ka apiju za prikaz po filteru
        $.ajax({
            url: "/api/products/ListaFiltriranihProizvoda",
            type: "POST",
            data: JSON.stringify({
                Id: primenjeno
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
                            $("#proizvodi > tbody").append('<tr><td class="align-middle">' + k.Naziv.substring(0, 20) + '</td>' +
                                '<td><img alt="proizvod" width="80" height="80" src="/Uploads/' + k.Slika + '" /></td>' +
                                '<td class="align-middle">' + k.Kolicina + '</td>' +
                                '<td class="align-middle">' + k.Cena + '&nbsp;RSD</td>' +
                                '<td class="align-middle">' + k.Opis.substring(0, 20) + '</td>' +
                                '<td class="align-middle">' + new Date(k.DatumPostavljanjaProizvoda).toLocaleDateString("en-GB") + '</td>' +
                                '<td class="align-middle">' + k.Grad.substring(0, 20) + '</td>' +
                                '<td class="align-middle"><b>' + (k.Status ? "NA&nbsp;STANJU" : "NEDOSTUPAN") + '</b></td>' +
                                '<td class="align-middle"><div class="align-middle justify-content-center">' +
                                '&emsp;<a class="btn btn-dark" href="IzmenaProizvoda.html?id=' + k.Id + '">Izmena</a>&emsp;' +
                                '<a class="btn btn-danger" href="BrisanjeProizvoda.html?id=' + k.Id + '">Brisanje</a></div></td>' +
                                '</tr>');
                        });
                    });

                    var kriterijum = $("#sort").find(":selected").text().toLowerCase();
                    // desila se greska - prikazi je
                    $("#divgreske").removeClass('d-none');
                    $("#greske").text("Prikazani su svi " + ($("#dostupni").is(":checked") ? "dostupni" : "") + " proizvodi sortirani koristeći kriterijum " + kriterijum + ".");
                    return false;
                }
                else {
                    // desila se greska - prikazi je
                    $("#divgreske").removeClass('d-none');
                    $("#greske").text("Nema proizvoda koji ispunjavaju uneti kriterijum!");
                    return false;
                }
            }
        });

        return false; // prevent ajax page reload
    });
});