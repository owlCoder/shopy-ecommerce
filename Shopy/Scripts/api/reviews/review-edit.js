var uloga;
var Id;
var STARA_SLIKA;

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
                uloga = data.Uloga;

                if (uloga !== 0) {
                    // nije pitanju kupac - povratak na pocetnu stranicu
                    window.location.href = "MojProfil.html";
                }

                // id iz url
                let searchParams = new URLSearchParams(window.location.search)
                if (!searchParams.has('id')) window.location.href = "Index.html";
                Id = searchParams.get('id');

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
                                url: "/api/reviews/RecenzijaPoIdPorudzbine",
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
                                        $("#naslov").val(data.Naslov);
                                        $("#sadrzaj").text(data.SadrzajRecenzije);
                                        STARA_SLIKA = data.Slika;
                                        $("#pregled").attr('src', '/Uploads/' + data.Slika);
                                    }
                                    else {
                                        window.location.href = "MojePorudzbine.html?msg=Recenzija ne postoji ili je odbijena. Dalje izmena nije moguća!";
                                    }
                                }
                            });

                            return false;
                        }
                        else {
                            // desila se greska
                            $("#divgreske").removeClass('d-none');
                            $("#greske").text(data.Poruka);
                        }
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

// akcija za dodavanje recenzije
jQuery(function () {
    $("#divgreske").addClass("d-none");
    $("body").removeClass("d-none");

    $("#slika").on('change', function () {
        if (slika.files[0] === undefined) {
            pregled.src = "/Images/productplc.jpg";
        }
        else {
            pregled.src = URL.createObjectURL(slika.files[0]);
        }
    });

    // provera unosa - verifikacija unosa na klijentu
    $("#izmenirecenzijubtn").on('click', function () {
        var naslov = $("#naslov").val();
        var sadrzaj = $("#sadrzaj").val();
        var slika = $("#slika").val();
        var greska = false;

        // provera unosa na klijentu
        if (naslov.length < 3) {
            $("#g1").addClass("d-block");
            $("#g1").removeClass("d-none");
            $("#naslov").addClass("is-invalid");
            $("#g1").text("Naslov mora imati minimalno 3 karaktera!");
            greska = true;
        }
        else {
            $("#g1").addClass("d-none");
            $("#naslov").removeClass("is-invalid");
            $("#naslov").removeClass("invalid-feedback");
            $("#naziv").add("is-valid");
        }

        if (sadrzaj.length < 20) {
            $("#g2").addClass("d-block");
            $("#g2").removeClass("d-none");
            $("#sadrzaj").addClass("is-invalid");
            $("#g2").removeClass("d-none");
            $("#g2").text("Sadržaj mora imati minimalno 20 karaktera!");
            greska = true;
        }
        else {
            $("#g2").addClass("d-none");
            $("#sadrzaj").removeClass("is-invalid");
            $("#sadrzaj").removeClass("invalid-feedback");
            $("#sadrzaj").add("is-valid");
        }

        if (greska === true) return; // ne izvrsava se ajax poziv - rasterecuje se server od callbacks

        if (slika.length <= 0) {
            // ajax poziv ka API-ju za izmenu recenzije
            $.ajax({
                url: "/api/reviews/IzmenaRecenzije",
                type: "POST",
                data: JSON.stringify({
                    PorudzbinaId: parseInt(Id),
                    Naslov: naslov,
                    Sadrzaj: sadrzaj,
                    Slika: STARA_SLIKA
                }),
                async: false,
                cache: false,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (response) {

                    if (response != null) {
                        if (JSON.parse(response).Kod === 0) {
                            window.location.href = "MojePorudzbine.html?msg=Recenzija za porudžbinu uspešno ažurirana!";
                            return false;
                        }
                        else {
                            // desila se greska - prikazi je
                            $("#divgreske").removeClass('d-none');
                            $("#greske").text(JSON.parse(response).Poruka);
                        }
                    }
                }
            });
        }
        else {
            // priprema slike za slanje
            var slikaAjax = new FormData();
            slikaAjax.append("slika", $("#slika")[0].files[0]);

            // ajax poziv ka api-ju za upload slike
            $.ajax({
                url: "/api/storage/OtpremanjeSlike",
                type: "POST",
                contentType: false,
                processData: false,
                data: slikaAjax,
                success: function (response) {

                    if (response != null) {
                        var msg = JSON.parse(response);
                        if (msg.Kod === 0) // upis podataka o recenziji je prosao uspesno
                        {
                            // ajax poziv ka API-ju za izmenu recenzije
                            $.ajax({
                                url: "/api/reviews/IzmenaRecenzije",
                                type: "POST",
                                data: JSON.stringify({
                                    PorudzbinaId: parseInt(Id),
                                    Naslov: naslov,
                                    Sadrzaj: sadrzaj,
                                    Slika: msg.Poruka
                                }),
                                async: false,
                                cache: false,
                                dataType: "json",
                                contentType: "application/json; charset=utf-8",
                                success: function (response) {

                                    if (response != null) {
                                        if (JSON.parse(response).Kod === 0) {
                                            window.location.href = "MojePorudzbine.html?msg=Recenzija za porudžbinu uspešno ažurirana!";
                                            return false;
                                        }
                                        else {
                                            // desila se greska - prikazi je
                                            $("#divgreske").removeClass('d-none');
                                            $("#greske").text(JSON.parse(response).Poruka);
                                        }
                                    }
                                }
                            });

                            return false;
                        }
                        else {
                            // desila se greska - prikazi je
                            $("#divgreske").removeClass('d-none');
                            $("#greske").text(JSON.parse(response).Poruka);
                        }
                    }
                }
            });
        }
    });
});