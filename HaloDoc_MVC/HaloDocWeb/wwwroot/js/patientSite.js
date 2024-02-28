var isFirstImage = true;

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
