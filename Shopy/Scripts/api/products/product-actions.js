var uloga = -1;
var Id = "";
var maxQuantity = 0;

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
                uloga = data.Uloga;


            });
        }
    }
});

jQuery(function () {
    $("#kupmsg").addClass("d-none");
    $("#nema").addClass("d-none");

    // id iz url
    let searchParams = new URLSearchParams(window.location.search)

    if (!searchParams.has('id')) {
        window.location.href = "Index.html?msg=Proizvod više nije dostupan na platformi!";
        return false;
    }

    // samo kupci mogu porucivati i dodavati u omiljene
    if (uloga !== 0) {
        $("#porucibtn").addClass("disabled");
        $("input").attr("disabled", true);
        $("#kupmsg").removeClass("d-none");
        $("#omiljenbtn").addClass("d-none");
    }

    Id = searchParams.get('id');

    // obrisi korisnika po id
    $.ajax({
        url: "/api/products/DostupanProizvodPoId",
        type: "POST",
        data: JSON.stringify({
            id: Id
        }),
        async: false,
        cache: false,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            // ako je proizvod obrisan u medjuvremenu)
            var data = JSON.parse(response);

            // ako je proizvod postao nedostupan u medjuvremenu
            if (data.Id === 0) {
                window.location.href = "Index.html?msg=Proizvod više nije dostupan na platformi!";
                return false;
            }

            // ispis podataka o proizvodu
            maxQuantity = data.Kolicina;

            $("#grad").text(data.Grad);
            $("#naslov").text(data.Naziv.substring(0, 100));
            $("#cena").text(data.Cena + " RSD");
            $("#opis").text(data.Opis);
            $("#slika").attr('src', "/Uploads/" + data.Slika);
        }
    });

    // proveriti da li je proizvod vec dodat u omiljene proizvode
    $.ajax({
        url: "/api/users/OmiljeniProizvod",
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
            if (data.Poruka === "OK") {
                // nalazi se u omiljenim
                $(function () {
                    $("#omiljenbtn").addClass('d-none'); // sakriti dugme za dodavanje u omiljenje
                });
            }
            else {
                $(function () {
                    // nije jos uvek u omiljenim proizvodima
                    $("#omiljenovec").addClass('d-none'); // sakriti dugme vec u omiljenim
                });
            }
        }
    });

    // ucitavanje recenzija
    $.ajax({
        url: "/api/reviews/RecenzijeZaProizvodPoId",
        type: "POST",
        data: JSON.stringify({
            Id: Id,
        }),

        async: false,
        cache: false,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            var res = JSON.parse(response);

            if (res.length > 0) {
                $(function () {
                    // popuni tabelu
                    $.each(res, function (key, data) {
                        // dodavanje reda po red
                        $("#recenzijelista").append(
                            '<div class="col-md-10 pt-4"><div class="card"><div class="card-body m-3"><div class="row"><div class="col-lg-4 d-flex justify-content-center align-items-center mb-4 mb-lg-0">' +
                            '<img src="/Uploads/' + data.Slika + '" class=" shadow-1 rounded-5" alt="sliak recenzije" width="150" height="150" /></div>' +
                            '<div class="col-lg-8"><h4>' + data.Naslov + '</h4><p class="text-muted fw-light mb-4">' + data.SadrzajRecenzije + '</p><p class="fw-bold lead mb-1"><strong>' + data.Recenzent.Ime + ' ' + data.Recenzent.Prezime + '</strong></p>' +
                            '<p class="fw-bold text-muted mb-0">Shopy Kupac</p></div></div></div></div></div>'
                        );
                    });
                });
            }
            else {
                // nema recenzija
                $("#nema").removeClass("d-none");
            }
        }
    });

    // limitiranje unosa kolicine
    $('input').on('input', function () {
        var value = $(this).val();
        if ((value !== '') && (value.indexOf('.') === -1)) {
            $(this).val(Math.max(Math.min(value, maxQuantity), -maxQuantity));
        }
    });

    // inicijalno nema poruka o greskama
    $("#divgreske").addClass('d-none');

    // kreiranje porudzbine
    $("#porucibtn").on('click', function () {
        var kolicina = parseFloat($("#kolicina").val());

        if (kolicina <= 0.0) {
            $(function () {
                $("#divgreske").removeClass('d-none');
                $("#greske").text("Minimalna količina porudžbine je 1 komad!");
            });
        }

        // ajax poziv ka api-ju za vrsenje porudzbine
        $.ajax({
            url: "/api/orders/KreiranjePorudzbine",
            type: "POST",
            data: JSON.stringify({
                Id: Id,
                Kolicina: kolicina
            }),
            async: false,
            cache: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                var data = JSON.parse(response);

                $(function () {
                    $("#divgreske").removeClass('d-none');
                    $("#greske").text(data.Poruka);
                });
            }
        });

    });

    // dodavanje u omiljene
    $("#omiljenbtn").on('click', function () {
        // ajax poziv ka api-ju za dodavanje u omiljenje proizvode
        // Id je trenutni id proizvoda koji se dodaje u omiljene
        $.ajax({
            url: "/api/users/DodajOmiljeniProizvod",
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
                if (data.Poruka === "OK") {
                    // nalazi se u omiljenim
                    $(function () {
                        $("#omiljenbtn").addClass('d-none'); // sakriti dugme za dodavanje u omiljenje
                        $("#omiljenovec").removeClass('d-none');
                        $("#divgreske").removeClass('d-none');
                        $("#greske").text("Proizvod je dodat u omiljene proizvode!");
                    });
                }
                else {
                    $(function () {
                        // nije jos uvek u omiljenim proizvodima
                        $("#omiljenovec").addClass('d-none'); // sakriti dugme vec u omiljenim
                    });
                }
            }
        });

    });
});