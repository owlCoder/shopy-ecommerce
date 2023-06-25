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
    url: "/api/orders/MojePorudzbine",
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

                    //if (k.Status)
                    //    kupac_dodaj_naruci = '<a href="Proizvod.html?id=' + k.Id + '" class="btn btn-dark">Pogledajte proizvod</a>';
                    //else
                    //    kupac_dodaj_naruci = '<a href="#" class="btn btn-outline-danger disabled">Proizvod nije dostupan</a>';

                    var status = "";
                    var akcijaPrispeca = "";
                    var ostaviRecenziju = "";
                    var izmeniRecenziju = "";
                    var obrisiRecenziju = "";

                    if (k.Status === 0) {
                        // aktivna
                        status = '<span class="h5 card-title text-primary fw-semibold justify-content-end">AKTIVNA</span>';

                        // aktivne mogu preci u izvrsene
                        akcijaPrispeca = '<a href="OznaciPrispece.html?id=' + k.Id + '" class="btn btn-dark">Označite kao prispelu</a>';
                        ostaviRecenziju = '<a href="#" class="btn btn-outline-primary disabled">Čeka se prispeće porudžbine...</a>';
                    }
                    else if (k.Status === 1) {
                        // izvrsena
                        status = '<span class="h5 card-title text-success fw-semibold justify-content-end">IZVRŠENA</span>';
                        akcijaPrispeca = '<a href="#" class="btn btn-outline-dark disabled">Porudžbina obrađena</a>';

                        // provera da li porudzbina ako je izvrsena ima recenziju, ako ima, kupac ima mogucnost
                        // da recenziju izmeni ili obrise
                        // obrisi korisnika po id

                        $.ajax({
                            url: "/api/reviews/PostojiRecenzija",
                            type: "POST",
                            data: JSON.stringify({
                                id: k.Id
                            }),
                            async: false,
                            cache: false,
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            success: function (response) {
                                var data = JSON.parse(response);

                                if (data.Kod === 0) {
                                    // postoji i ceka se na odobrenje, kupac je moze izmeniti
                                    ostaviRecenziju = "";
                                    izmeniRecenziju = '<a href="IzmeniRecenziju.html?id=' + k.Id + '" class="btn btn-outline-success">Izmenite recenziju</a>';
                                    obrisiRecenziju = '<a href="ObrisiRecenziju.html?id=' + k.Id + '" class="btn btn-outline-danger">Obrišite recenziju</a>';
                                }
                                else if (data.Kod === 5) { // recenzija ne postoji uopste, tek se treba kreirati
                                    ostaviRecenziju = '<a href="OstaviRecenziju.html?id=' + k.Id + '" class="btn btn-outline-primary">Ostavite recenziju</a>';
                                    izmeniRecenziju = "";
                                    obrisiRecenziju = "";
                                }
                                else if (data.Kod === 1) {
                                    // odobrena je, nema dalje izmene
                                    ostaviRecenziju = '<a href="#' + k.Id + '" class="btn btn-outline-success disabled">Recenzija odobrena</a>';
                                    izmeniRecenziju = "";
                                    obrisiRecenziju = "";
                                }
                                else if (data.Kod === 2) {
                                    // odbijena je, nema dalje izmene
                                    ostaviRecenziju = '<a href="#' + k.Id + '" class="btn btn-outline-danger disabled">Recenzija odbijena</a>';
                                    izmeniRecenziju = "";
                                    obrisiRecenziju = "";
                                }
                            }
                        });
                    }
                    else {
                        // otkazana
                        status = '<span class="h5 card-title text-danger fw-semibold justify-content-end">OTKAZANA</span>';
                        akcijaPrispeca = '<a href="#" class="btn btn-outline-danger disabled">Porudžbina otkazana</a>';
                        ostaviRecenziju = '<a href="#" class="btn btn-outline-danger disabled">Ostavljanje recenzije nije moguće</a>';
                    }

                    $("#proizvodi").append(
                        '<div class="card mb-4 border-success">' +
                        '<div class="card-body">' +
                        '<div class="row justify-content-between">' +
                        '<div class="col-2">' +
                        '<img alt="proizvod" class="card-img-top rounded-circle" width="150" height="150" src="/Uploads/' + k.Proizvod.Slika + '" />' +
                        '</div>' +
                        '<div class="col-10">' +
                        '<div class="d-flex justify-content-between">' +
                        '<span class="h5 card-title justify-content-start">' + k.Proizvod.Naziv + '</span>' +
                        status +
                        '</div>' +
                        '<h6 class="card-subtitle mb-2 text-muted">Ukupno: ' + (parseFloat(k.Proizvod.Cena) * parseFloat(k.Kolicina)) + ' RSD</h6>' +
                        '<p class="card-text">Količina: ' + k.Kolicina + '</p>' +
                        kupac_dodaj_naruci + akcijaPrispeca + "&emsp;" + ostaviRecenziju + izmeniRecenziju + "&emsp;" + obrisiRecenziju +
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