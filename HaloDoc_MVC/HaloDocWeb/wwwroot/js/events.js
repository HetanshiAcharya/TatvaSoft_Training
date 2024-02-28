

const togglePassword = document.querySelector("#togglePassword");
console.log(togglePassword);
const password = document.querySelector("#password");

togglePassword.addEventListener("click", function () {
  const type =
    password.getAttribute("type") === "password" ? "text" : "password";
  password.setAttribute("type", type);

  this.classList.toggle("fa-eye");
});
let isFirstImage = true;

function myFunction() {
  var element = document.body;
  var img = document.querySelector("#moon-img");

  element.classList.toggle("dark-mode");
  console.log(isFirstImage);
  if (isFirstImage) {
    img.src = "./images/sun.png";
  } else {
    img.src = "./images/moon.png";
  }
  isFirstImage = !isFirstImage;
  console.log(isFirstImage);
}

var triggerTabList = [].slice.call(document.querySelectorAll('#home a'))
triggerTabList.forEach(function (triggerEl) {
    var tabTrigger = new bootstrap.Tab(triggerEl)

    triggerEl.addEventListener('click', function (event) {
        event.preventDefault()
        tabTrigger.show()
    })
})

