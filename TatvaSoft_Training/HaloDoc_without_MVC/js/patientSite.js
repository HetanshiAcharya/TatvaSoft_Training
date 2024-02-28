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
