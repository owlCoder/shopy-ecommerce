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
            if (JSON.parse(response).Kod === 0) // korisnik je ulogovan, ne moze da se registruje
            {
                window.location.href = "Index.html";
            }
        }
    }
});

jQuery(function () {
    // inicijalno nema poruka o greskama
    $("#divgreske").addClass('d-none');

    $("#prijavabtn").on('click', function () {
        var korisnickoIme = $("#korisnickoime").val();
        var lozinka = $("#lozinka").val();

        var greska = false;

        // provera unosa na klijentu
        if (korisnickoIme.length < 6) {
            $("#g1").addClass("d-block");
            $("#g1").removeClass("d-none");
            $("#korisnickoime").addClass("is-invalid");
            $("#g1").text("Korisničko ime mora imati minimalno 6 karaktera!");
            greska = true;
        }
        else {
            $("#g1").addClass("d-none");
            $("#korisnickoime").removeClass("is-invalid");
            $("#korisnickoime").removeClass("invalid-feedback");
            $("#korisnickoime").add("is-valid");
        }

        if (lozinka.length < 6) {
            $("#g2").addClass("d-block");
            $("#g2").removeClass("d-none");
            $("#lozinka").addClass("is-invalid");
            $("#g2").text("Lozinka mora imati minimalno 6 karaktera!");
            greska = true;
        }
        else {
            $("#g2").addClass("d-none");
            $("#lozinka").removeClass("is-invalid");
            $("#lozinka").removeClass("invalid-feedback");
            $("#lozinka").add("is-valid");
        }

        if (greska === true) return; // ne izvrsava se ajax poziv - rasterecuje se server od callbacks

        // ajax poziv ka api-ju za vrsenje registracije
        $.ajax({
            url: "/api/auth/Prijava",
            type: "POST",
            data: JSON.stringify({
                KorisnickoIme: korisnickoIme,
                Lozinka: lozinka
            }),

            async: false,
            cache: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response != null) {
                    if (JSON.parse(response).Kod === 0) // korisnik se prijavio uspesno
                    {
                        window.location.href = "Index.html";
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