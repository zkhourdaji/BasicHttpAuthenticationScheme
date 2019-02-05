// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var logoutButton = document.getElementById("logoutButton");
logoutButton.addEventListener("click", logoutFunction);


function logoutFunction() {
    alert("Please close your browser to logout.")
}