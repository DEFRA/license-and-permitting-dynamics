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
            "PaymentRecord": {
                "@odata.type": "Microsoft.Dynamics.CRM.defra_payment",
                "defra_paymentid": entityId
            }
        }
        var actionResult = Payments.ActivateAction(actionName, actionParameters);

        // 4. Redirect to GovPay Portal using the url provided 
        if (actionResult) {

            // Todo: Check for errors

            // Redirect to GovPay next url
            var nextUrl = actionResult.PaymentNextUrlHref;
            window.open(nextUrl, 'GovPay', 'width=600,height=600');
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
    }
}