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

// get metoda za dobijanje listi svih proizvoda za korisnika
$.ajax({
    url: "/api/orders/SvePorudzbine",
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
                    var status = "";
                    var akcijaPrispeca = "";
                    var otkaziPorudzbinu = "";
                    var odobriRecenziju = "";
                    var otkaziRecenziju = "";

                    if (k.Status === 0) {
                        // aktivna
                        status = '<span class="h5 card-title text-primary fw-semibold justify-content-end">AKTIVNA</span>';

                        // aktivne mogu preci u izvrsene
                        akcijaPrispeca = '<a href="OznaciPrispece.html?id=' + k.Id + '" class="btn btn-outline-dark">Potvrda prispeća porudžbine</a>';
                        otkaziPorudzbinu = '<a href="OtkaziPorudzbinu.html?id=' + k.Id + '" class="btn btn-outline-danger">Otkazivanje porudžbine</a>';
                    }
                    else if (k.Status === 1) {
                        // izvrsena
                        status = '<span class="h5 card-title text-success fw-semibold justify-content-end">IZVRŠENA</span>';
                        akcijaPrispeca = '<a href="#" class="btn btn-outline-dark disabled">Porudžbina obrađena</a>';

                        // proveri da li postoji recenzija, ako postoji, proveri onda i status,
                        // ako je na cekanju
                        // prikazi ODOBRI ILI ODBI
                        // ako je ODBIJENA prikazi odbijena
                        // ako je odobrena prikazi odobrena
                        // id iz url
                        Id = k.Id.toString();

                        // popuni informacije o porudzbini za koju se dodaje recenzija
                        $.ajax({
                            url: "/api/orders/PorudzbinaRecenzija",
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

                                if (data.hasOwnProperty("Id")) {
                                    // tek sada moze videti formu
                                    $("#bodyauth").removeClass("d-none");

                                    // popunjavanje forme podacima
                                    $("#pid").text("Porudžbina #" + data.Id);
                                    $("#datum").text("Izvršena: " + data.Datum);
                                    $("#nazivpr").text(data.NazivProizvoda);
                                    $("#ukupaniznos").text("Ukupan iznos: " + data.UkupanIznos + " RSD");
                                    $("#slikapr").attr('src', "/Uploads/" + data.Slika);

                                    // popuni podatke o recenziji, Id je id porudzbine
                                    // porudzbina - recenzija 1:1
                                    $.ajax({
                                        url: "/api/reviews/RecenzijaPoIdPorudzbineAdmin",
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

                                            // postoji recenzija vracen je json objekat sa Id
                                            if (data.hasOwnProperty("Id")) {
                                                // koji je status recenzije
                                                if (data.Status === 0) {
                                                    // ceka na odobrenje
                                                    odobriRecenziju = '<a href="OdobriRecenziju.html?id=' + data.Id + '" class="btn btn-outline-success">Odobri recenziju</a>'
                                                    otkaziRecenziju = '<a href="OtkaziRecenziju.html?id=' + data.Id + '" class="btn btn-outline-danger">Odbi recenziju</a>'
                                                }
                                                else if (data.Status === 1) {
                                                    // odobrena je
                                                    odobriRecenziju = '<a href="#" class="btn btn-outline-success disabled">Odobrili ste recenziju</a>'
                                                    otkaziRecenziju = "";
                                                }
                                                else if (data.Status === 2) {
                                                    // odbijena je
                                                    odobriRecenziju = '<a href="#" class="btn btn-outline-danger disabled">Odbili ste recenziju</a>'
                                                    otkaziPorudzbinu = "";
                                                }
                                            }
                                            else {
                                                odobriRecenziju = '<a href="#" class="btn btn-outline-primary disabled">Recenzija nije kreirana</a>'
                                            }
                                        }
                                    });
                                }
                                else {
                                    // desila se greska
                                    $("#divgreske").removeClass('d-none');
                                    $("#greske").text(data.Poruka);
                                }
                            }
                        });
                    }
                    else {
                        // otkazana
                        status = '<span class="h5 card-title text-danger fw-semibold justify-content-end">OTKAZANA</span>';
                        akcijaPrispeca = '<a href="#" class="btn btn-outline-danger disabled">Porudžbina otkazana</a>';
                        otkaziPorudzbinu = '<a href="#" class="btn btn-outline-danger disabled">Promena statusa porudžbine nije moguća</a>';
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
                        kupac_dodaj_naruci + akcijaPrispeca + "&emsp;" + otkaziPorudzbinu + odobriRecenziju + "&emsp;" + otkaziRecenziju +
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