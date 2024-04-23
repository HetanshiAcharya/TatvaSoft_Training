var phone1 = document.querySelector("#phone");
var phoneInput1 = window.intlTelInput(phone1, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
    nationalMode: false,
    showSelectedDialCode: true,
    autoInsertDialCode: true,
    formatOnDisplay: false,
});
var phone2 = document.querySelector("#phone1");
var phoneInput2 = window.intlTelInput(phone2, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
    nationalMode: false,
    showSelectedDialCode: true,
    autoInsertDialCode: true,
    formatOnDisplay: false,
});
