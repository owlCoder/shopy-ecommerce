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
                    // nije pitanju prodavac - povratak na zadatu stranicu
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

jQuery(function () {
    // inicijalno nema poruka o greskama
    $("#divgreske").addClass('d-none');

    // azuriranje pregleda slike
    // azuriranje pregleda slike
    $("#slika").on('change', function () {
        if (slika.files[0] === undefined) {
            pregled.src = "/Images/productplc.jpg";
        }
        else {
            pregled.src = URL.createObjectURL(slika.files[0]);
        }
    });


    // cuvanje izmena
    $("#dodajbtn").on('click', function () {
        var naziv = $("#naziv").val();
        var opis = $("#opis").val();
        var cena = $("#cena").val();
        var kolicina = $("#kolicina").val();
        var grad = $("#grad").val();
        var slika = $("#slika").val();
        var greska = false;

        // provera unosa na klijentu
        if (naziv.length < 3) {
            $("#g1").addClass("d-block");
            $("#g1").removeClass("d-none");
            $("#naziv").addClass("is-invalid");
            $("#g1").text("Naziv mora imati minimalno 3 karaktera!");
            greska = true;
        }
        else {
            $("#g1").addClass("d-none");
            $("#naziv").removeClass("is-invalid");
            $("#naziv").removeClass("invalid-feedback");
            $("#naziv").add("is-valid");
        }

        if (opis.length < 20) {
            $("#g2").addClass("d-block");
            $("#g2").removeClass("d-none");
            $("#opis").addClass("is-invalid");
            $("#g2").removeClass("d-none");
            $("#g2").text("Opis mora imati minimalno 20 karaktera!");
            greska = true;
        }
        else {
            $("#g2").addClass("d-none");
            $("#opis").removeClass("is-invalid");
            $("#opis").removeClass("invalid-feedback");
            $("#opis").add("is-valid");
        }

        if (cena <= 0) {
            $("#g3").addClass("d-block");
            $("#g3").removeClass("d-none");
            $("#cena").addClass("is-invalid");
            $("#g3").text("Cena ne sme biti negativan broj!");
            greska = true;
        }
        else {
            $("#g3").addClass("d-none");
            $("#cena").removeClass("is-invalid");
            $("#cena").removeClass("invalid-feedback");
            $("#cena").add("is-valid");
        }

        if (kolicina < 0) {
            $("#g4").addClass("d-block");
            $("#g4").removeClass("d-none");
            $("#kolicina").addClass("is-invalid");
            $("#g4").text("Količina ne sme biti negativan broj!");
            greska = true;
        }
        else {
            $("#g4").addClass("d-none");
            $("#kolicina").removeClass("is-invalid");
            $("#kolicina").removeClass("invalid-feedback");
            $("#kolicina").add("is-valid");
        }

        if (grad.length < 3) {
            $("#g5").addClass("d-block");
            $("#g5").removeClass("d-none");
            $("#grad").addClass("is-invalid");
            $("#g5").text("Grad mora imati minimalno 3 karaktera!");
            greska = true;
        }
        else {
            $("#g5").addClass("d-none");
            $("#grad").removeClass("is-invalid");
            $("#grad").removeClass("invalid-feedback");
            $("#grad").add("is-valid");
        }

        if (slika.length <= 0) {
            $("#g6").addClass("d-block");
            $("#g6").removeClass("d-none");
            $("#slika").addClass("is-invalid");
            $("#g6").text("Morate odabrati sliku proizvoda!");
            greska = true;
        }
        else {
            $("#g6").addClass("d-none");
            $("#slika").removeClass("is-invalid");
            $("#slika").removeClass("invalid-feedback");
            $("#slika").add("is-valid");
        }

        if (greska === true) return; // ne izvrsava se ajax poziv - rasterecuje se server od callbacks

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
                    if (msg.Kod === 0) // azuriranje podataka je proslo uspesno
                    {
                        // ajax poziv ka API-ju za kreiranje proizvoda
                        $.ajax({
                            url: "/api/products/DodavanjeProizvoda",
                            type: "POST",
                            data: JSON.stringify({
                                Naziv: naziv,
                                Cena: cena,
                                Kolicina: kolicina,
                                Opis: opis,
                                Slika: msg.Poruka,
                                Grad: grad
                            }),
                            async: false,
                            cache: false,
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            success: function (response) {
                                if (response != null) {
                                    if (JSON.parse(response).Kod === 0) {
                                        window.location.href = "MojiProizvodi.html";
                                        return false;
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
    });
});