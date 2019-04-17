// ------------------------------------------------------
// File: payment_form_events.js
// JavaScript responsible for initiating the GovPay
// Card Payment Process
// Retrieves the relevant payment details from the CRM 
// Payment form, calls the Create Payment CRM Action
// and directs the browser to the GovPay payment portal
// ------------------------------------------------------

var Payments = {

    // Function sets the listeners for messages that the payment has been updated
    OnLoad: function () {
        if (window.XMLHttpRequest) {
            //for browsers other than ie
            window.addEventListener("message", Payments.ReceivedPostMessage, false);
        } else {
            //ie
            window.attachEvent('onmessage', Payments.ReceivedPostMessage);
        }

        // Remove the Write-off option from payment type
        Xrm.Page.getControl("defra_type").removeOption("910400010");

        // Remove the Online Payment option from payment type
        Xrm.Page.getControl("defra_type").removeOption("910400000");
    },

    // Function called when user presses the Take Card Payment button from within CRM
    TakeCardPayment: function () {

        // 0. Clear form notifications so as not to clutter the screen
        Xrm.Page.ui.clearFormNotification();

        // 1. Check form data is saved
        if (Xrm.Page.data.entity.getIsDirty() === true) {
            Xrm.Page.ui.setFormNotification("Please save the record first and try again.", "WARNING");
            return;
        };

        // TODO: Add validation and friendly error messages.

        // 2. Get payment record details
        var entityId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');
        var paymentAmount = Xrm.Page.getAttribute("defra_paymentvalue").getValue();
        var paymentDescription = Xrm.Page.getAttribute("defra_description").getValue();
        var paymentReference = Xrm.Page.getAttribute("defra_reference_number").getValue();
        var returnUrl = Payments.GetReturnUrl(paymentReference);

        // 3. Call the Create Payment CRM Action
        var actionName = "defra_create_payment_transaction";
        var payment = {};
        payment.primarykeyid = entityId;
        payment["@odata.type"] = "Microsoft.Dynamics.CRM.defra_payment";
        var actionParameters = {
            "Amount": paymentAmount,
            "ReturnUrl": returnUrl,
            "Reference": paymentReference,
            "Description": paymentDescription,
            "ConfigurationPrefix": "MOTO",
            "PaymentRecord": {
                "@odata.type": "Microsoft.Dynamics.CRM.defra_payment",
                "defra_paymentid": entityId
            }
        }
        var actionResult = Payments.ActivateAction(actionName, actionParameters);

        // 4. Redirect to GovPay Portal using the url provided 
        if (actionResult) {
            // Display error message if status of error returned with a 200
            var paymentStatus = actionResult.PaymentStatus;
            if (paymentStatus === "error") {
                // Display error message
                Xrm.Page.ui.setFormNotification("Payment transaction could not be completed. GOV.UK Pay is not available. Please contact your system administrator with payment ref: " + paymentReference, "ERROR");
            } else {
                // Redirect to GovPay next url
                var nextUrl = actionResult.PaymentNextUrlHref;
                Payments.PopupCenter(nextUrl, 'GovPay', 750, 700);
            }
        } else {
            // Display error message
            Xrm.Page.ui.setFormNotification("Payment transaction could not be completed. Please contact your system administrator with payment ref: " + paymentReference, "ERROR");
        }
    },

    // Generic function to call a CRM action with given parameters
    ActivateAction: function (actionName, actionParams) {
        var result = null;
        // set endpoint
        var endpoint = Xrm.Page.context.getClientUrl() + "/api/data/v8.2/"; // Constants.ODATA_ENDPOINT_URI;

        var req = new XMLHttpRequest();
        req.open("POST", endpoint + actionName, false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");


        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;

                // handle success
                if (this.status === 200) {
                    result = JSON.parse(this.response);
                } else {
                    // Error
                    var error = JSON.parse(this.response).error;
                    Xrm.Page.ui.setFormNotification("ERROR: " + error.message, "ERROR");
                }
            }
        }

        req.send(window.JSON.stringify(actionParams));
        return result;
    },

    // Function generates a return url, which will be provided to 
    // GovPay. Once the payment has been completed/cancelled, the
    // Gov pay portal will return to this URL.
    GetReturnUrl: function (paymentReference) {
        return Xrm.Page.context.getClientUrl() + "/WebResources/defra_/payments/return_from_portal.htm?Data=" + paymentReference;
    },

    // Function listens for messages that the payment has been updated
    ReceivedPostMessage: function (event) {

        if (event.origin !== window.location.origin) {
            return;
        }

        // Refresh the payment form if this is the payment that has been updated.
        var receivedReference = event.data;
        var paymentReference = Xrm.Page.getAttribute("defra_reference_number").getValue();
        if (paymentReference === receivedReference) {
            // Refresh the form
            Xrm.Page.data.refresh();
        }

        // Let the parent know
        Payments.SendMessageToApplication();
    },

    PopupCenter: function (url, title, w, h) {
        // Fixes dual-screen position
        var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : window.screenX;
        var dualScreenTop = window.screenTop != undefined ? window.screenTop : window.screenY;

        var width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        var height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        if (h < height) {
            h = height - 20;
        }

        var left = ((width / 2) - (w / 2)) + dualScreenLeft;
        var top = ((height / 2) - (h / 2)) + dualScreenTop;
        var newWindow = window.open(url, title, 'scrollbars=yes, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);

        // Puts focus on the newWindow
        if (window.focus) {
            newWindow.focus();
        }
    },

    // Function sets the payment description based on fields in form
    UpdateDescriptionField: function () {

        // Get the Payment Type
        var paymentType = Xrm.Page.getAttribute("defra_type").getText();

        // Get the Payment Category
        var paymentCategory = Xrm.Page.getAttribute("defra_paymentcategory").getText();

        // Get the Application Name
        var lookupObj = Xrm.Page.getAttribute("defra_applicationid");

        var applicationName = '';
        if (lookupObj != null) {
            var lookupObjValue = lookupObj.getValue(); //Check for Lookup Value
            if (lookupObjValue != null && lookupObjValue[0].name != null && lookupObjValue[0].name != 'null') {
                applicationName = lookupObjValue[0].name;
            }
        }

        // Set the Description Field
        var newDescription = paymentCategory ? paymentCategory + ' ' : '';
        newDescription = newDescription + (paymentType ? paymentType + ' ' : '');
        newDescription = newDescription + (applicationName ? 'for application "' + applicationName + '"' : '');

        Xrm.Page.getAttribute("defra_description").setValue(newDescription);
    },

    // Function sets the payment description based on fields in form
    PaymentCategoryChanged: function () {

        var paymentCategory = Xrm.Page.getAttribute("defra_paymentcategory").getValue();

        // If Inbound
        if (paymentCategory === 910400000) {
            // Inbound

            // Remove refund, reversal, manual refund, manual reversal, writeoff
            Xrm.Page.getControl("defra_type").removeOption("910400006");
            Xrm.Page.getControl("defra_type").removeOption("910400007");
            Xrm.Page.getControl("defra_type").removeOption("910400008");
            Xrm.Page.getControl("defra_type").removeOption("910400009");

        }
        else if (paymentCategory === 910400001){
            // Outbound

            // Remove inbound options
            Xrm.Page.getControl("defra_type").removeOption("910400001");
            Xrm.Page.getControl("defra_type").removeOption("910400002");
            Xrm.Page.getControl("defra_type").removeOption("910400003");
            Xrm.Page.getControl("defra_type").removeOption("910400004");
            Xrm.Page.getControl("defra_type").removeOption("910400005");


        }
    },

    // Alert Application form of change
    SendMessageToApplication: function () {
        window.parent.parent.postMessage(Payments.PaymentRef, window.location.origin);
    }
}