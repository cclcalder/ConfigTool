// <JQuery datepickers>

// Here: https://api.jqueryui.com/datepicker/ (and go down to Localization)
// it's written how to include the british culture for date pickers.
// I couldn't make it with referencing their datepicker-en-GB.js file
// so I've pasted the required code in here

$.datepicker.regional["en-GB"] = {
    closeText: "Done",
    prevText: "Prev",
    nextText: "Next",
    currentText: "Today",
    monthNames: ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"],
    monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
    "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
    dayNames: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
    dayNamesShort: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
    dayNamesMin: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
    weekHeader: "Wk",
    dateFormat: "dd/mm/yy",
    firstDay: 1,
    isRTL: false,
    showMonthAfterYear: false,
    yearSuffix: ""
};

// </JQuery datepicers>
 
function closeListingEditor() {
    // Sliding the item editor to the right (specifically: toggling it)
    $("#itemEditorPanel").animate({ width: 'toggle' }, 350, function () {
        // Using a callback (function ()) to hide the overlay after the item editor finishes sliding (not during)
        $("#overlay").hide();
    });
}