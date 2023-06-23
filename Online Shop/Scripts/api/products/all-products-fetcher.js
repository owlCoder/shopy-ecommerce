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
                $.each(ucitano, function (key, data) {
                    // dodavanje reda po red
                    var kupac_dodaj_naruci = "";

                    kupac_dodaj_naruci = '<a href="Proizvod.html?id=' + data.Id + '" class="btn btn-dark">Pogledajte proizvod</a>';

                    $("#proizvodi").append(
                        '<div class="card mb-4 border-success">' +
                        '<div class="card-body">' +
                        '<div class="row">' +
                        '<div class="col-2" style="padding-bottom: 0 !important">' +
                        '<img alt="proizvod" class="card-img-top w-75 h-75 mt-3" width="150" height="150" src="/Uploads/' + data.Slika + '" />' +
                        '</div>' +
                        '<div class="col-10">' +
                        '<div class="d-flex justify-content-between">' +
                        '<span class="h5 card-title justify-content-start">' + data.Naziv + '</span>' +
                        '<span class="h5 card-title text-success fw-semibold justify-content-end">' + data.Grad + '</span>' +
                        '</div>' +
                        '<h6 class="card-subtitle mb-2 text-muted">' + data.Cena + ' RSD</h6>' +
                        '<p class="card-text">' + data.Opis + '</p>' +
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