// ------------------------------------------------------
// File: application_form_events.js
// JavaScript responsible for handling events for the 
// Application entity, such as WriteOffs and page
// refreshes.
// ------------------------------------------------------

var Applications = {

    BalanceAmount: 0,

    // Function sets the listeners for messages that the payment has been updated
    OnLoad: function () {
        if (window.XMLHttpRequest) {
            //for browsers other than ie
            window.addEventListener("message", Payments.ReceivedPostMessage, false);
        } else {
            //ie
            window.attachEvent('onmessage', Payments.ReceivedPostMessage);
        }
    },

    // Function called when user presses the Write Off button from the CRM ribbon
    WriteOffBalance: function () {

        // 2. Check form data is saved
        if (Xrm.Page.data.entity.getIsDirty() === true) {
            Xrm.Page.ui.setFormNotification("Please save the record first and try again.", "WARNING");
            return;
        };

        // 3. Refresh form to ensure we have the latest value

        // 4. Get write-off details
        Applications.BalanceAmount = Xrm.Page.getAttribute("defra_balance").getValue();

        // 5. Check if user can write off balance
        var currentUserId = Xrm.Page.context.getUserId().replace('{', '').replace('}', '');
        var maxWriteOff = Applications.CallGetMaximumWriteOffForUserAction(currentUserId);
        if (!maxWriteOff ||
            (Applications.BalanceAmount > 0 && maxWriteOff < Applications.BalanceAmount) ||
            (Applications.BalanceAmount < 0 && maxWriteOff < (Applications.BalanceAmount * -1))) {
            // Cannot write off
            Xrm.Page.ui.setFormNotification("The application balance of " + Applications.BalanceAmount + " exceeds the maximum write-off of " + maxWriteOff + " assigned to your team. Please check with the Finance Administrator.", "WARNING");
            return;
        }

        // 6. Confirm user wants to write off
        Xrm.Utility.confirmDialog("Are you sure you want to write-off the application balance of " +
            Applications.BalanceAmount +
            "?",
            Applications.WriteOffBalanceConfirmed,
            Applications.WriteOffBalanceCancelled);
    },

    WriteOffBalanceConfirmed: function () {
        // Call action to write off the application balance
        var entityId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');
        var currentUserId = Xrm.Page.context.getUserId().replace('{', '').replace('}', '');
        Applications.CallWriteOffAction(Applications.BalanceAmount, entityId, currentUserId);
    },

    WriteOffBalanceCancelled: function () {
        Xrm.Page.ui.setFormNotification("The balance write-off has been cancalled", "INFO");
    },


    // Generic function to call a CRM action with given parameters
    CallGetMaximumWriteOffForUserAction: function (userId) {
        var maximumWriteOffValue;
        var parameters = {};

        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/systemusers(" + userId + ")/Microsoft.Dynamics.CRM.defra_getmaximumwriteoffusercanmake", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    maximumWriteOffValue = results.MaximumWriteOffValue;
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send(JSON.stringify(parameters));

        return maximumWriteOffValue;
    },


    // Generic function to call a CRM action with given parameters
    CallWriteOffAction: function (writeOffAmount, applicationId, userId) {
        var parameters = {};
        parameters.Balance = writeOffAmount;
        //var user = {};
        //user.primarykeyid = userId;
        //user["@odata.type"] = "Microsoft.Dynamics.CRM.systemuser";
        //parameters.User = user;

        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/defra_applications(" + applicationId + ")/Microsoft.Dynamics.CRM.defra_write_off_application_balance", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    //Success
                    Xrm.Utility.alertDialog("The write-off has been completed.");
                    // Refresh the form
                    Xrm.Page.data.refresh();
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send(JSON.stringify(parameters));
    },

    // Function listens for messages that the record has been updated
    ReceivedPostMessage: function (event) {

        if (event.origin !== window.location.origin) {
            return;
        }

        // Refresh the  form if this is the record that has been updated
        var receivedId = event.data;
        var thisId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');
        if (receivedId === thisId) {
            // Refresh the form
            Xrm.Page.data.refresh();
        }
    }

}