var STARA_SLIKA = "";
var id_iz_url = "";
var uloga_trenutna = 0;

// popunjavanje forme podacima o proizvodu
// ako nema id u url, redirect
let searchParams = new URLSearchParams(window.location.search)
if (searchParams.has('id') === false) { // nema id, pokusaj hardcore pristupa, redirect
    window.location.href = "Index.html";
}
id_iz_url = searchParams.get('id');

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

                if (uloga !== 1 && uloga !== 2) {
                    // nije pitanju prodavac ni administrator - povratak na zadatu stranicu
                    window.location.href = "MojProfil.html";
                }

                uloga_trenutna = uloga;

                $.ajax({
                    url: "/api/products/ProizvodPoId",
                    type: "POST",
                    data: JSON.stringify({
                        Id: id_iz_url
                    }),
                    async: false,
                    cache: false,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        if (response != null) {
                            var data = JSON.parse(response);

                            // ne postoji proizvod, obrisan je u medjuvremenu
                            if (uloga === 1 && data.Id === 0) {
                                window.location.href = "MojiProizvodi.html";
                                return false;
                            }
                            else if (uloga === 2 && data.Id === 0) {
                                window.location.href = "ListaProizvoda.html";
                                return false;
                            }

                            // prodavac ne moze da menja ili brise proizvode koji nisu dostupni
                            if (uloga === 1 && data.Status === false) {
                                window.location.href = "MojiProizvodi.html?msg=Odabrani proizvod nije moguće izmeniti jer više nije DOSTUPAN!";
                                return false;
                            }

                            // tek sada moze videti formu
                            $("#bodyauth").removeClass("d-none");

                            // popunjavanje forme podacima
                            $("#naziv").val(data.Naziv);
                            $("#opis").val(data.Opis);
                            $("#cena").val(data.Cena);
                            $("#kolicina").val(data.Kolicina);
                            $("#grad").val(data.Grad);
                            STARA_SLIKA = data.Slika;
                            $("#pregled").attr('src', "/Uploads/" + data.Slika);
                            return false;
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

// Odjava korisnika
jQuery(function () {
    // inicijalno nema poruka o greskama
    $("#divgreske").addClass('d-none');

    // azuriranje pregleda slike
    $("#slika").on('change', function () {
        if (slika.files[0] === undefined) {
            pregled.src = "/Uploads/" + STARA_SLIKA;
        }
        else {
            pregled.src = URL.createObjectURL(slika.files[0]);
        }
    });


    // cuvanje izmena
    $("#izmenabtn").on('click', function () {
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

        if (greska === true) return; // ne izvrsava se ajax poziv - rasterecuje se server od callbacks

        if (slika.length > 0) {
            // priprema slike za slanje
            var slikaAjax = new FormData();
            slikaAjax.append("slika", $("#slika")[0].files[0]);

            // promenjena je slika
            // treba izvrsiti upload
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
                            STARA_SLIKA = msg.Poruka; // novi naziv tj guid slike na serveru

                            // ajax poziv ka API-ju za azuriranje proizvoda
                            $.ajax({
                                url: "/api/products/AzuriranjeProizvoda",
                                type: "POST",
                                data: JSON.stringify({
                                    Id: id_iz_url,
                                    Naziv: naziv,
                                    Cena: cena,
                                    Kolicina: kolicina,
                                    Opis: opis,
                                    Slika: STARA_SLIKA,
                                    Grad: grad
                                }),
                                async: false,
                                cache: false,
                                dataType: "json",
                                contentType: "application/json; charset=utf-8",
                                success: function (response) {
                                    if (response != null) {
                                        if (JSON.parse(response).Kod === 0) {
                                            if (uloga_trenutna == 1) { // prodavac
                                                window.location.href = "MojiProizvodi.html?msg=Proizvod je uspešno ažuriran u bazi podataka!";
                                                return false;
                                            }
                                            else if (uloga_trenutna == 2) { // admin
                                                window.location.href = "ListaProizvoda.html?msg=Proizvod je uspešno ažuriran u bazi podataka!";
                                                return false;
                                            }
                                            else {
                                                window.location.href = "MojProfil.html";
                                                return false;
                                            }
                                        }
                                    }
                                }
                            });
                        }
                        else {
                            // desila se greska - prikazi je
                            $("#divgreske").removeClass('d-none');
                            $("#greske").text(JSON.parse(response).Poruka);
                            return false;
                        }
                    }
                }
            });
        }

        // ajax poziv ka API-ju za azuriranje proizvoda
        $.ajax({
            url: "/api/products/AzuriranjeProizvoda",
            type: "POST",
            data: JSON.stringify({
                Id: id_iz_url,
                Naziv: naziv,
                Cena: cena,
                Kolicina: kolicina,
                Opis: opis,
                Slika: STARA_SLIKA,
                Grad: grad
            }),
            async: false,
            cache: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response != null) {
                    if (JSON.parse(response).Kod === 0) {
                        if (uloga_trenutna == 1) { // prodavac
                            window.location.href = "MojiProizvodi.html?msg=Proizvod je uspešno ažuriran u bazi podataka!";
                            return false;
                        }
                        else if (uloga_trenutna == 2) { // admin
                            window.location.href = "ListaProizvoda.html?msg=Proizvod je uspešno ažuriran u bazi podataka!";
                            return false;
                        }
                        else {
                            window.location.href = "MojProfil.html";
                            return false;
                        }
                    }
                }
            }
        });

        return false;
    });
});