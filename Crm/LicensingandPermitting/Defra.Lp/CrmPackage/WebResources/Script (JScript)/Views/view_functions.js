// ------------------------------------------------------
// File: defra_view_functions.js
// JavaScript responsible for setting icons in views
// ------------------------------------------------------

var Views = {

    // Images
    ImageGreenFileName: "defra_icon_green",
    ImageAmberFileName: "defra_icon_amber",
    ImageRedFileName: "defra_icon_red",
    ImageBlankFileName: "defra_icon_blank",

    // Fields
    FieldPenfoldDate: "defra_penfolddate",

    // Function retuns the RAG status icon to be be displayed on an Application View
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedParameter
    DisplayApplicationPendfoldIcon: function (rowData, userLCID) {
        var str = JSON.parse(rowData);
        var diffHours = 0;
        var imgName;
        var tooltip;
        var isInActiveSlaStatus = false;

        console.log('Application Penfold Date:' + str.defra_penfolddate + ' typeof: ' + (typeof str.defra_penfolddate));

        // Get the number of hours till penfold date
        if (str.defra_penfolddate) {

            // Get hours till penfold date
            var currentDate = Date.now();
            var date = str.defra_penfolddate.split("/");
            var d = parseInt(date[0], 10),
                m = parseInt(date[1], 10),
                y = parseInt(date[2], 10);

            var penfoldDate = new Date(y, m - 1, d);
            var timeDiff = Math.abs(penfoldDate - currentDate);
            diffHours = Math.ceil(timeDiff / (1000 * 3600));
            console.log('Application Penfold ' + penfoldDate + ' - ' + currentDate + ' = diffHours:' + diffHours);
        }

        // Check if in one of the statues where we're counting the SLA
        console.log('Application Status Reason ' + str.statuscode);
        if (str.statuscode === 'Determination'
            || str.statuscode === 'Peer Review'
            || str.statuscode === 'Pending Consultation'
            || str.statuscode === 'On Hold: Pending Further Info'
            || str.statuscode === 'On Hold: Pending Schedule 5'
            || str.statuscode === 'Area Approval'
            || str.statuscode === 'Team Leader Approval') {
            isInActiveSlaStatus = true;
        }

        // Not in applicable status, or penfold date not yet set
        if (isInActiveSlaStatus === false || !str.defra_penfolddate) {
            // Blank Icon
            imgName = Views.ImageBlankFileName;
            tooltip = "SLA not running on this application";
        }

        // Four weeks remaining, or penfold date not set
        else if (diffHours > 672) {
            // Green Icon
            imgName = Views.ImageGreenFileName;
            tooltip = "More than 4 weeks remaining";
        }

        // Two to four weeks remaining
        else if (diffHours > 336) {
            // Amber Icon
            imgName = Views.ImageAmberFileName;
            tooltip = "2 to 4 weeks remaining";
        }

        // Two to four weeks remaining, or overdue
        else {
            // Red Icon
            imgName = Views.ImageRedFileName;
            tooltip = "Less than two weeks remaining";
        }

        console.log('Application Penfold Image:' + imgName);

        // Return to CRM
        var resultarray = [imgName, tooltip];
        return resultarray;
    }
}