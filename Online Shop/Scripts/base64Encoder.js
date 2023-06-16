// Funkcija za pretvaranje slike u base64 string za slanje kroz json
function getBase64(file)
{
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        return reader.result.toString();
    };
    reader.onerror = function () {
        return "";
    };
}