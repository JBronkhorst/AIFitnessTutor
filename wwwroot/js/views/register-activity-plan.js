$(document).ready(function () {
    var noneCheckbox = $('#day-None');
    var activityTimeSelect = $('#activity-time');
    var activityDayCheckboxes = $('input[name="ActivityDays"]').not(noneCheckbox);

    function toggleActivityDays() {
        if (noneCheckbox.is(':checked')) {
            activityTimeSelect.val('None').prop('disabled', true);
            activityDayCheckboxes.prop('checked', false).prop('disabled', true);
        } else {
            activityTimeSelect.prop('disabled', false);
            activityDayCheckboxes.prop('disabled', false);
        }
    }

    noneCheckbox.change(function () {
        toggleActivityDays();
    });

    activityDayCheckboxes.change(function () {
        if (activityDayCheckboxes.is(':checked')) {
            noneCheckbox.prop('checked', false);
        }
    });

    toggleActivityDays();
});
