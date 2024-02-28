function dashboard() {
  window.location = "dashboard.html";
}
function sidebar() {
  var sidebar = document.getElementById("sidebar");
  var display = sidebar.style.display;
  if (display == "none" || display == "") {
    sidebar.style.display = "block";
  } else {
    sidebar.style.display = "none";
  }
}
