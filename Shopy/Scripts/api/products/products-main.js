jQuery(function () {
    let searchParams = new URLSearchParams(window.location.search)

    if (searchParams.has('msg')) {
        $("#divgreske").removeClass('d-none');
        $("#greske").text(searchParams.get('msg'));
        return false;
    }

    // dugme pretraga
    $("#pretragabtn").on('click', function () {
        // isprazni redove
        $('#proizvodi').empty();
        $("#divgreske").addClass('d-none');

        var sortiranje = $("#sort").val().toString();
        var naziv = $("#naziv").val();
        var grad = $("#grad").val();
        var minCena = $("#od").val();
        var maxCena = $("#do").val();

        if (minCena === "" && maxCena !== "") {
            $("#divgreske").removeClass('d-none');
            $("#greske").text("Potrebno je uneti i minimalnu cenu kako bi se prikazali proizvodi u željenom cenovnom opsegu.");
            return false;
        }
        else if (minCena !== "" && maxCena === "") {
            $("#divgreske").removeClass('d-none');
            $("#greske").text("Potrebno je uneti i maksimalnu cenu kako bi se prikazali proizvodi u željenom cenovnom opsegu.");
            return false;
        }

        // ako nije uneo cene - nema primene tog opsega
        if (minCena === "" && maxCena === "") {
            minCena = -1.0;
            maxCena = -1.0;
        }
        else {
            // unete su obe cene
            minCena = parseFloat(minCena);
            maxCena = parseFloat(maxCena);

            if (minCena < 0) {
                $("#divgreske").removeClass('d-none');
                $("#greske").text("Potrebno je uneti minimalnu cenu koja nije negativan broj kako bi se prikazali proizvodi u željenom cenovnom opsegu.");
                return false;
            }

            if (maxCena <= 0) {
                $("#divgreske").removeClass('d-none');
                $("#greske").text("Potrebno je uneti maksimalnu cenu koja nije negativan broj kako bi se prikazali proizvodi u željenom cenovnom opsegu.");
                return false;
            }

            if (minCena > maxCena) {
                $("#divgreske").removeClass('d-none');
                $("#greske").text("Niste uneli validan opseg cena. Proverite unete cenovne opsege!");
                return false;
            }
        }

        // poziv ka apiju za prikaz po filteru
        $.ajax({
            url: "/api/products/ListaFiltriranihProizvodaPocetna",
            type: "POST",
            data: JSON.stringify({
                Naziv: naziv,
                Grad: grad,
                MinCena: minCena,
                MaxCena: maxCena,
                Sortiranje: sortiranje
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
                            var kupac_dodaj_naruci = '<a href="Proizvod.html?id=' + k.Id + '" class="btn btn-dark">Pogledajte proizvod</a>';

                            $("#proizvodi").append(
                                '<div class="card mb-4 border-success">' +
                                '<div class="card-body">' +
                                '<div class="row">' +
                                '<div class="col-2" style="padding-bottom: 0 !important">' +
                                '<img alt="proizvod" class="card-img-top w-75 h-75 mt-3" width="150" height="150" src="/Uploads/' + k.Slika + '" />' +
                                '</div>' +
                                '<div class="col-10">' +
                                '<div class="d-flex justify-content-between">' +
                                '<span class="h5 card-title justify-content-start">' + k.Naziv + '</span>' +
                                '<span class="h5 card-title text-success fw-semibold justify-content-end">' + k.Grad + '</span>' +
                                '</div>' +
                                '<h6 class="card-subtitle mb-2 text-muted">' + k.Cena + ' RSD</h6>' +
                                '<p class="card-text">' + k.Opis.substring(0, 300) + '</p>' +
                                kupac_dodaj_naruci +
                                '</div>' +
                                '</div>' +
                                '</div>' +
                                '</div>');
                        });
                    });

                    var kriterijum = $("#sort").find(":selected").text().toLowerCase();
                    // desila se greska - prikazi je
                    $("#divgreske").removeClass('d-none');
                    $("#greske").text("Prikazani su svi proizvodi dobijenim kombinovanom pretragom i sortirani koristeći kriterijum " + kriterijum + ".");
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

    $("#sort").on('change', function () {
        // isprazni redove
        $('#proizvodi').empty();
        $("#divgreske").addClass('d-none');

        var sortiranje = $("#sort").val().toString();
        var naziv = $("#naziv").val();
        var grad = $("#grad").val();
        var minCena = $("#od").val();
        var maxCena = $("#do").val();

        if (minCena === "" && maxCena !== "") {
            $("#divgreske").removeClass('d-none');
            $("#greske").text("Potrebno je uneti i minimalnu cenu kako bi se prikazali proizvodi u željenom cenovnom opsegu.");
            return false;
        }
        else if (minCena !== "" && maxCena === "") {
            $("#divgreske").removeClass('d-none');
            $("#greske").text("Potrebno je uneti i maksimalnu cenu kako bi se prikazali proizvodi u željenom cenovnom opsegu.");
            return false;
        }

        // ako nije uneo cene - nema primene tog opsega
        if (minCena === "" && maxCena === "") {
            minCena = -1.0;
            maxCena = -1.0;
        }
        else {
            // unete su obe cene
            minCena = parseFloat(minCena);
            maxCena = parseFloat(maxCena);

            if (minCena < 0) {
                $("#divgreske").removeClass('d-none');
                $("#greske").text("Potrebno je uneti minimalnu cenu koja nije negativan broj kako bi se prikazali proizvodi u željenom cenovnom opsegu.");
                return false;
            }

            if (maxCena <= 0) {
                $("#divgreske").removeClass('d-none');
                $("#greske").text("Potrebno je uneti maksimalnu cenu koja nije negativan broj kako bi se prikazali proizvodi u željenom cenovnom opsegu.");
                return false;
            }

            if (minCena > maxCena) {
                $("#divgreske").removeClass('d-none');
                $("#greske").text("Niste uneli validan opseg cena. Proverite unete cenovne opsege!");
                return false;
            }
        }

        // poziv ka apiju za prikaz po filteru
        $.ajax({
            url: "/api/products/ListaFiltriranihProizvodaPocetna",
            type: "POST",
            data: JSON.stringify({
                Naziv: naziv,
                Grad: grad,
                MinCena: minCena,
                MaxCena: maxCena,
                Sortiranje: sortiranje
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
                            var kupac_dodaj_naruci = '<a href="Proizvod.html?id=' + k.Id + '" class="btn btn-dark">Pogledajte proizvod</a>';

                            $("#proizvodi").append(
                                '<div class="card mb-4 border-success">' +
                                '<div class="card-body">' +
                                '<div class="row">' +
                                '<div class="col-2" style="padding-bottom: 0 !important">' +
                                '<img alt="proizvod" class="card-img-top w-75 h-75 mt-3" width="150" height="150" src="/Uploads/' + k.Slika + '" />' +
                                '</div>' +
                                '<div class="col-10">' +
                                '<div class="d-flex justify-content-between">' +
                                '<span class="h5 card-title justify-content-start">' + k.Naziv + '</span>' +
                                '<span class="h5 card-title text-success fw-semibold justify-content-end">' + k.Grad + '</span>' +
                                '</div>' +
                                '<h6 class="card-subtitle mb-2 text-muted">' + k.Cena + ' RSD</h6>' +
                                '<p class="card-text">' + k.Opis.substring(0, 300) + '</p>' +
                                kupac_dodaj_naruci +
                                '</div>' +
                                '</div>' +
                                '</div>' +
                                '</div>');
                        });
                    });

                    var kriterijum = $("#sort").find(":selected").text().toLowerCase();
                    // desila se greska - prikazi je
                    $("#divgreske").removeClass('d-none');
                    $("#greske").text("Prikazani su svi proizvodi dobijenim kombinovanom pretragom i sortirani koristeći kriterijum " + kriterijum + ".");
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

    $("#ponistibtn").on('click', function () {
        // isprazni redove
        $('#proizvodi').empty();
        $("#divgreske").addClass('d-none');

        // get metoda za dobijanje listi svih proizvoda
        $.ajax({
            url: "/api/products/SviProizvodiPocetna",
            type: "GET",
            async: false,
            cache: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                var ucitano = JSON.parse(response);

                if (ucitano.length > 0) {
                    $(function () {
                        // popuni tabelu
                        $.each(ucitano, function (key, k) {
                            // dodavanje reda po red
                            var kupac_dodaj_naruci = '<a href="Proizvod.html?id=' + k.Id + '" class="btn btn-dark">Pogledajte proizvod</a>';

                            $("#proizvodi").append(
                                '<div class="card mb-4 border-success">' +
                                '<div class="card-body">' +
                                '<div class="row">' +
                                '<div class="col-2" style="padding-bottom: 0 !important">' +
                                '<img alt="proizvod" class="card-img-top w-75 h-75 mt-3" width="150" height="150" src="/Uploads/' + k.Slika + '" />' +
                                '</div>' +
                                '<div class="col-10">' +
                                '<div class="d-flex justify-content-between">' +
                                '<span class="h5 card-title justify-content-start">' + k.Naziv + '</span>' +
                                '<span class="h5 card-title text-success fw-semibold justify-content-end">' + k.Grad + '</span>' +
                                '</div>' +
                                '<h6 class="card-subtitle mb-2 text-muted">' + k.Cena + ' RSD</h6>' +
                                '<p class="card-text">' + k.Opis.substring(0, 300) + '</p>' +
                                kupac_dodaj_naruci +
                                '</div>' +
                                '</div>' +
                                '</div>' +
                                '</div>');
                        });
                    });
                }
            }
        });

    });
});