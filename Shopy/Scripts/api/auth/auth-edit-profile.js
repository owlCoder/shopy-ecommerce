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

                let currentDate = `${day}/${month}/${year}`;
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
        var korisnickoIme = $("#korisnickoime").val();
        var ime = $("#ime").val();
        var prezime = $("#prezime").val();
        var pol = $("#pol").val();
        var email = $("#email").val();
        var datumRodjenja = $("#datumrodjenja").val();

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

        if (ime.length < 3) {
            $("#g3").addClass("d-block");
            $("#g3").removeClass("d-none");
            $("#ime").addClass("is-invalid");
            $("#g3").removeClass("d-none");
            $("#g3").text("Ime mora imati minimalno 3 karaktera!");
            greska = true;
        }
        else {
            $("#g3").addClass("d-none");
            $("#ime").removeClass("is-invalid");
            $("#ime").removeClass("invalid-feedback");
            $("#ime").add("is-valid");
        }

        if (prezime.length < 5) {
            $("#g4").addClass("d-block");
            $("#g4").removeClass("d-none");
            $("#prezime").addClass("is-invalid");
            $("#g4").text("Prezime mora imati minimalno 5 karaktera!");
            greska = true;
        }
        else {
            $("#g4").addClass("d-none");
            $("#prezime").removeClass("is-invalid");
            $("#prezime").removeClass("invalid-feedback");
            $("#prezime").add("is-valid");
        }

        var regexEmail = /^[A-Z0-9._%+-]+@([A-Z0-9-]+\.)+[A-Z]{2,4}$/i;
        if (regexEmail.test(email) === false) {
            $("#g5").addClass("d-block");
            $("#g5").removeClass("d-none");
            $("#email").addClass("is-invalid");
            $("#g5").removeClass("d-none");
            $("#g5").text("Email nije unet u validnom formatu!");
            greska = true;
        }
        else {
            $("#g5").addClass("d-none");
            $("#email").removeClass("is-invalid");
            $("#email").removeClass("invalid-feedback");
            $("#email").add("is-valid");
        }

        var minDate = Date.parse("1970-01-01");
        var maxDate = Date.parse("2020-01-01");
        if (datumRodjenja === "") // datum se nije promenio
            datumRodjenja = STARI_DATUM;

        var datum = Date.parse(datumRodjenja.split("/").join("-"));
        datumRodjenja = Date.parse(datumRodjenja.split("/").join("-"));
        datumRodjenja = new Date(datumRodjenja).toISOString();
        
        if ((datum < minDate) || (datum > maxDate)) {
            $("#g6").addClass("d-block");
            $("#g6").removeClass("d-none");
            $("#datumRodjenja").addClass("is-invalid");
            $("#g6").text("Niste uneli datum rođenja u dozvoljenom opsegu!");
            greska = true;
        }
        else {
            $("#g6").addClass("d-none");
            $("#datumRodjenja").removeClass("is-invalid");
            $("#datumRodjenja").removeClass("invalid-feedback");
            $("#datumRodjenja").add("is-valid");
        }

        if (greska === true) return; // ne izvrsava se ajax poziv - rasterecuje se server od callbacks

        // ajax poziv ka api-ju za vrsenje registracije
        $.ajax({
            url: "/api/auth/AzuriranjeProfila",
            type: "POST",
            data: JSON.stringify({
                KorisnickoIme: korisnickoIme,
                Ime: ime,
                Lozinka: "oc_auth_434u3dfs-dsdw-dummy",
                Prezime: prezime,
                Pol: pol,
                Email: email,
                DatumRodjenja: datumRodjenja
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