// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Function to show or hide loading screen
function toggleLoadingScreen(show) {
    var loadingScreen = document.getElementById('loading-screen');
    if (show) {
        loadingScreen.style.display = 'flex'; // Show loading screen
    } else {
        loadingScreen.style.display = 'none'; // Hide loading screen
    }
}
