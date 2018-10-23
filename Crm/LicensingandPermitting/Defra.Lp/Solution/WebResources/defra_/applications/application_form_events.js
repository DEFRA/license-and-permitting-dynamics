// ------------------------------------------------------
// File: application_form_events.js
// JavaScript responsible for handling events for the 
// Application entity, such as WriteOffs and page
// refreshes.
// ------------------------------------------------------

var Applications = {

    BalanceAmount: 0,
    AboutToRefresh: false,
    LastRefresh: new Date().getTime(),

    // Called by the list views when they have changed/refreshed
    // Function refreshes the form so long as it it allowed by minimum refresh frequency
    Refresh: function () {
        if (Applications.CanRefresh()) {
            console.log("Refreshing...");

            // Refresh the screen to get the new updates fields
            Applications.AboutToRefresh = true;
            Xrm.Page.data.refresh();
            Applications.AboutToRefresh = false;
            Applications.LastRefresh = new Date().getTime();
        } else {
            console.log("Not refreshing: AboutToRefresh = " + Applications.AboutToRefresh);
        }
    },

    // Dictates whether the form can refresh
    CanRefresh: function () {
        var now = new Date().getTime();
        var msSinceLastRefresh = now - Applications.LastRefresh;

        if (Applications.AboutToRefresh) {
            // Not refreshing
            console.log("CanRefresh = No because AboutToRefresh = " + Applications.AboutToRefresh);
            return false;
        }

        if (msSinceLastRefresh < 10000) {
            console.log("CanRefresh = No because msSinceLastRefresh = " + msSinceLastRefresh);
            // Not refreshing
            return false;
        }

        if (Xrm.Page.data.entity.getIsDirty()) {
            console.log("CanRefresh = No because page is dirty");
            // Not refreshing, form has updates
            return false;
        }

        return true;
    },

    // Function sets the listeners for messages that the payment has been updated
    OnLoad: function () {

        // Set the refresh count to 0, ensure we are not re-using a value from another application
        Applications.RefreshCnt = 0;

        // Wait 3 seconds before adding the onload events to prevent render issues.
        setTimeout(
            function () {
                Applications.LastRefresh = new Date().getTime();
                Xrm.Page.getControl("ApplicationLines").addOnLoad(Applications.Refresh);
                Xrm.Page.getControl("Payments").addOnLoad(Applications.Refresh);
            }, 3000);

        // Add filter lookup for application sub type field
        Applications.PreFilterLookup();
    },

    // On Save event
    OnSave: function () {
        // Set the last refresh so that a grid refresh doesn't happen as we're saving.
        Applications.LastRefresh = new Date().getTime();
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

    // Called by the onload function, applies the Application Sub Type filter
    PreFilterLookup: function () {

        // Filter main form application subtype lookup
        var formLookupField = Xrm.Page.getControl("defra_application_subtype");
        if (formLookupField != null) {
            formLookupField.addPreSearch(function () {
                Applications.AddLookupFilterToApplicationSubType("defra_application_subtype");
            });
        }

        // Filter BPF application subtype lookup
        var bpfLookupField = Xrm.Page.getControl("header_process_defra_application_subtype");
        if (bpfLookupField != null) {
            bpfLookupField.addPreSearch(function () {
                Applications.AddLookupFilterToApplicationSubType("header_process_defra_application_subtype");
            });
        }
    },

    // Checks the current appplication type and filters the sub type
    AddLookupFilterToApplicationSubType: function (lookupFieldNameToFilter) {
        var applicationType = Xrm.Page.getAttribute("defra_applicationtype").getValue();
        if (applicationType != null) {
            var fetchXml = "<filter type='and'><condition attribute='defra_application_type' operator='eq' value='" + applicationType + "' /></filter>";
            Xrm.Page.getControl(lookupFieldNameToFilter).addCustomFilter(fetchXml);
        }
    }
}
