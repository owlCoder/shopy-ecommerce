﻿jQuery(function () {
    $("body").addClass("d-none");

    // navigacioni meni
    $("#nm").append('<nav id="navigacija" class="navbar navbar-expand-sm navbar-toggleable-sm navbar-dark bg-dark"> <div class="container"> <a href="/" class="navbar-brand"> 🛒 Shopy <span class="h6">eCommerce</span></a> <button type="button" class="navbar-toggler" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" title="Toggle navigation" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation"> <span class="navbar-toggler-icon"></span> </button> <div class="collapse navbar-collapse d-sm-inline-flex justify-content-between"> <ul class="navbar-nav flex-grow-1"> <li id="pl"> <a href="Registracija.html" type="submit" id="registracija" class="btn btn-outline-light navbar-btn login-btn">Registracija</a> <a href="Prijava.html" type="submit" id="prijava" class="btn btn-outline-light navbar-btn register-btn">Prijava</a> </li> <li id="profil"> <div class="btn-group profil-btn"> <button type="button" class="btn btn-outline-light dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false"> <span id="poruka"></span> </button> <ul class="dropdown-menu"> <li><a class="dropdown-item" href="MojProfil.html" id="mojprofil">Moj profil</a></li> <li><a class="dropdown-item" href="IzmenaLozinke.html" id="izmenalozinke">Izmena lozinke</a></li> <li> <hr class="dropdown-divider"> </li> <li><a class="dropdown-item text-danger" id="odjava" href="#">Odjava sa platforme</a></li> </ul> </div> </li> </ul> </div> </div> </nav>');
});