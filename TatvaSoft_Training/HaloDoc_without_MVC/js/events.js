const togglePassword = document.querySelector("#togglePassword");
console.log(togglePassword);
const password = document.querySelector("#password");

togglePassword.addEventListener("click", function () {
  // toggle the type attribute
  const type =
    password.getAttribute("type") === "password" ? "text" : "password";
  password.setAttribute("type", type);

  // toggle the icon
  this.classList.toggle("fa-eye");
});
let isFirstImage = true;

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
////////////////////
function openTab(tabId) {
  // Hide all tab contents
  var tabContents = document.getElementsByClassName("tab-content");
  for (var i = 0; i < tabContents.length; i++) {
    tabContents[i].style.display = "none";
  }

  // Remove 'active-tab' class from all tabs
  var tabs = document.getElementsByClassName("tab");
  for (var i = 0; i < tabs.length; i++) {
    tabs[i].classList.remove("active-tab");
  }

  // Show the selected tab content and mark it as active
  document.getElementById(tabId).style.display = "block";
  document
    .querySelector("[onclick=\"openTab('" + tabId + "')\"]")
    .classList.add("active-tab");
}
