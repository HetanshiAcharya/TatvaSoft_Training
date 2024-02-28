swal({
  title: "Information",
  text: "When submitting a request, you must provide the correct contact information for the patient or the responsibly party. Failure to provide the correct email and phone number will delay service or be declined.",
  type: "warning",
  confirmButtonColor: "#0dcaf0",
});

var isFirstImage = true;

function myFunction() {
  var element = document.body;
  var img = document.querySelector("#moon-img");

  element.classList.toggle("dark-mode");
  console.log(isFirstImage);
  if (isFirstImage) {
    img.src = "./images/ScreenShots/sun.png";
  } else {
    img.src = "./images/ScreenShots/moon.png";
  }
  isFirstImage = !isFirstImage;
  console.log(isFirstImage);
}

const phoneInputField1 = document.querySelector("#phone1");
const phoneInput1 = window.intlTelInput(phoneInputField1, {
  utilsScript:
    "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});
const phoneInputField2 = document.querySelector("#phone2");
const phoneInput2 = window.intlTelInput(phoneInputField2, {
  utilsScript:
    "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});

function homepage() {
  window.location.href = "C:Userspci68DesktopHaloDocindex.html";
}
