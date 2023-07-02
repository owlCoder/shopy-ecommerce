var STARI_DATUM = "";

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

                // Popuni formu
                $("#korisnickoime").val(data.KorisnickoIme);
                $("#ime").val(data.Ime);
                $("#prezime").val(data.Prezime);
                $("#pol").val(data.Pol);
                $("#email").val(data.Email);

                let today = new Date(data.DatumRodjenja);

                let day = (today.getDate());
                let month = ('0' + (today.getMonth() + 1)).slice(-2);
                let year = today.getFullYear();

                let currentDate = `${day}.${month}.${year}`;
                $("#trenutni").text("Datum rođenja: " + currentDate + " (trenutni)");
                STARI_DATUM = currentDate;
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

// Odjava korisnika
jQuery(function () {
    // inicijalno nema poruka o greskama
    $("#divgreske").addClass('d-none');

    // cuvanje izmena
    $("#azurirajbtn").on('click', function () {
        var stara_lozinka = $("#staralozinka").val();
        var nova_lozinka = $("#novalozinka").val();

        var greska = false;

        // provera unosa na klijentu
        if (stara_lozinka.length < 6) {
            $("#g2").addClass("d-block");
            $("#g2").removeClass("d-none");
            $("#staralozinka").addClass("is-invalid");
            $("#g2").text("Lozinka mora imati minimalno 6 karaktera!");
            greska = true;
        }
        else {
            $("#g2").addClass("d-none");
            $("#staralozinka").removeClass("is-invalid");
            $("#staralozinka").removeClass("invalid-feedback");
            $("#staralozinka").add("is-valid");
        }

        if (nova_lozinka.length < 6) {
            $("#g1").addClass("d-block");
            $("#g2").removeClass("d-none");
            $("#novalozinka").addClass("is-invalid");
            $("#g2").text("Lozinka mora imati minimalno 6 karaktera!");
            greska = true;
        }
        else {
            $("#g2").addClass("d-none");
            $("#novalozinka").removeClass("is-invalid");
            $("#novalozinka").removeClass("invalid-feedback");
            $("#novalozinka").add("is-valid");
        }

        if (greska === true) return; // ne izvrsava se ajax poziv - rasterecuje se server od callbacks

        // ajax poziv ka api-ju za vrsenje registracije
        $.ajax({
            url: "/api/auth/AzuriranjeLozinke",
            type: "POST",
            data: JSON.stringify({
                StaraLozinka: stara_lozinka,
                NovaLozinka: nova_lozinka
            }),

            async: false,
            cache: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response != null) {
                    if (JSON.parse(response).Kod === 0) // azuriranje podataka je proslo uspesno
                    {
                        window.location.href = "MojProfil.html";
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