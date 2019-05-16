
//** WE-2431 */
// ** DRAFT CODE ** Created by Kassim Hassan for Duly made checklist grid
// ** REVIEWED BY: Stuart Adair
// **********************************************************************

function dulymadeCheckListchanged() {
    var selectedGrid = null;
    var completedDulyMadeFlag;
    var values;

    try {
        selectedGrid = Xrm.Page.getControl("DulyMadeChecklist").getGrid().getRows();

        // map values into an array
        values = selectedGrid.map(function (row) {
            return row.getData().getEntity().attributes.getByName("defra_completed").getValue();
        });

        // completedDulyMadeFlag is set as true if every item in values array equals 910400000
        completedDulyMadeFlag = values.every(function (item) {
            return item === 910400000;
        });
        
        window.parent.Xrm.Page.getAttribute("defra_completeddulymade").setValue(completedDulyMadeFlag);

    } catch (e) {
        Xrm.Utility.alertDialog(e.message);
        showAlert("Error in dulymadeCheckListchanged – " + e.description);
    }
}

function showAlert(message) {
    var alertStrings = { confirmButtonLabel: "Yes", text: message };
    var alertOptions = { height: 120, width: 400 };
    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
        function success(result) {
            console.log("alert dialog closed");
        },
        function (error) {
            console.log(error.message);
        }
    );
}