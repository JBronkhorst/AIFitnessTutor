$(document).ready(function () {
    // Handle click event on day navigation links
    $('#day-nav .nav-link').click(function (e) {
        e.preventDefault();
        var targetDay = $(this).data('day');
        $('.nav-link').removeClass('active'); // Remove active class from all nav links
        $(this).addClass('active'); // Add active class to the clicked nav link
        $('.day-content').hide(); // Hide all day content sections
        $('.day-content[data-day="' + targetDay + '"]').show(); // Show the content of the selected day
        $('html, body').animate({
            scrollTop: $('.day-content[data-day="' + targetDay + '"]').offset().top
        }, 500);
    });

    // Show content for the current day by default
    var currentDay = "@currentDay";
    $('.day-content[data-day="' + currentDay + '"]').show(); // Show content for the current day
    $('.nav-link[data-day="' + currentDay + '"]').addClass('active'); // Add active class to the current day's nav link
    $('#day-nav .nav-link[data-day="' + currentDay + '"]').addClass('current-day'); // Add a class to style the current day
});
