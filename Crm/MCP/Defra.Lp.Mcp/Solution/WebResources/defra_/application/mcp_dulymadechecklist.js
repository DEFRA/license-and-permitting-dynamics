
//** WE-2431 **/
// ** DRAFT CODE ** Created by Kassim Hassan for Duly made checklist grid
// ** REVIEWED BY:
// **********************************************************************

function dulymadeCheckListchanged() {
    var selectedGrid = null;
    var completedDulyMadeFlag = true;
    var arrayItems = [];

    try {
        selectedGrid = Xrm.Page.getControl("DulyMadeChecklist").getGrid().getRows();
        selectedGrid.forEach(function (row) {
           // att = row.getData().getEntity().attributes.getByName("defra_completed").getValue();
            arrayItems.push(row);

        })

        values = arrayItems.map(function (row) {
            return row.getData().getEntity().attributes.getByName("defra_completed").getValue();
        });

        // completedDulyMadeFlag is set as true if values array isn't empty and every item equals 910400000
        completedDulyMadeFlag = Boolean(values.length && values.every(function (item) {
            return item === 910400000;
        }));

        window.parent.Xrm.Page.getAttribute("defra_completeddulymade").setValue(completedDulyMadeFlag);


        //     if (att != null && att != 'undefined') {
        //         if (att != 910400000) {
        //             completedDulyMadeFlag = false;
        //         }
        //     }
        //     else {
        //         completedDulyMadeFlag = false;
        //     }
        // });







        // window.parent.Xrm.Page.getAttribute("defra_completeddulymade").setValue(completedDulyMadeFlag);


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
            concole.log(error.message);
        }
    );
}