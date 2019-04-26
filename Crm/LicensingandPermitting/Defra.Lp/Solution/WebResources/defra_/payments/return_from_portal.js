// ------------------------------------------------------
// File: return_from_portal.js
// JavaScript responsible for processing the return from
// GovPay payment page. 
// Queries CRM for the payment reference and calls a
// CRM action to check if the payment was successful
// Modified by Kassim Hassan 
// ------------------------------------------------------

var Payments = {

    // The CRM Payment reference number
    PaymentRef: '',

    // The GovPay payment status
    PaymentStatus: '',

    // The CRM Payment entity
    PaymentRecord: '',

    // The GovPay payment error message
    PaymentError: '',

    // The payment Account ID
    ConfigurationPrefix: '',

    // Triggered when the HTML package loads
    OnLoad: function () {

        // 1. Retrieve payment number from querystring passed back by portal
        Payments.PaymentRef = Payments.GetDataParam();

        // 2. Get the Payment Record for the given payment ref
        //Payments.GetPayment(Payments.PaymentRef);

        // 3. Get the Payment Record for the given payment ref
        Payments.GetPaymentAccount(Payments.PaymentRef);

    },

    // Function queries CRM with the given Payment Reference number
    // and retries the Payment Record and Payment Transaction record
    // Thenb 
    GetPayment: function (paymentRefNum) {

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/defra_payments?$select=_defra_payment_transaction_value&$expand=defra_defra_payment_defra_payment_originatingpayment($select=defra_payment_transaction)&$filter=defra_reference_number eq '" + paymentRefNum + "'", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    for (var i = 0; i < results.value.length; i++) {
                        var _defra_payment_transaction_value = results.value[i]["_defra_payment_transaction_value"];
                        var _defra_payment_transaction_value_formatted = results.value[i]["_defra_payment_transaction_value@OData.Community.Display.V1.FormattedValue"];
                        var _defra_payment_transaction_value_lookuplogicalname = results.value[i]["_defra_payment_transaction_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                        //Use @odata.nextLink to query resulting related records
                        var defra_defra_payment_defra_payment_originatingpayment_NextLink = results.value[i]["defra_defra_payment_defra_payment_originatingpayment@odata.nextLink"];
                    }
                    Payments.PaymentRecord = results.value[0];
                    //var paymentLink = Payments.GetApplicationRecordUrl(Payments.PaymentRecord.id);
                    //Payments.SetReturnLink(paymentLink);
                } else {
                    alert(this.statusText);
                }
            }
        };
        req.send();
    },

    //** Created by Kassim Hassan on 26/04/2019 for WE-2429 to fix the bug in removing the hard coded  (parameters.ConfigurationPrefix = "WastePermits.MOTO.")*/
    // Function queries CRM with the given Payment Reference number
    // and retries the Payment Account ID 
    // Thenb 
    GetPaymentAccount: function (paymentRefNum) {

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/defra_payments?$top=50&$select=defra_cardpaymentaccount&$filter=defra_reference_number eq '" + paymentRefNum + "'", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    Payments.ConfigurationPrefix = results.value[0]["defra_cardpaymentaccount"];
                    // Call CRM Action to check payment status
                    Payments.ProcessCardPaymentStatus(Payments.PaymentRef);
                } else {
                    alert(this.statusText);
                }
            }
        };
        req.send();
    },

    // Functions calls the Get Payment Status CRM Action to check the status
    // of the payment, then displayes the payment status
    ProcessCardPaymentStatus: function (paymentRefNumber) {

        // Set-up Action Parameters
        var parameters = {};
        parameters.LookupByPaymentReference = paymentRefNumber;
        parameters.ConfigurationPrefix = Payments.ConfigurationPrefix;

        console.log("TEMP *****   ready state now is parameters.ConfigurationPrefix : " + parameters.ConfigurationPrefix);

        // Set-up the Get Payment Status CRM Action request
        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/defra_get_payment_status", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");

        // Process result handler
        req.onreadystatechange = function () {

            if (this.readyState === 4) {
                req.onreadystatechange = null;

                if (this.status === 200) {
                    // On Successful result, display the payment status
                    var results = JSON.parse(this.response);
                    Payments.PaymentStatus = results.Status;

                } else {
                    // Error
                    Payments.PaymentStatus = 'Error';
                    Payments.PaymentError = this.statusText;
                }
                // Display message to user
                Payments.DisplayResult();
                // Prompt payment form to refresh
                Payments.SendMessageToPayment();
            }
        };

        // Call the Action
        req.send(JSON.stringify(parameters));
    },

    // Generates an href pointing to the CRM Payment record, used to create a hyperlink
    GetApplicationRecordUrl: function (paymentGuid) {

        // var entityTypeCode = Mscrm.EntityPropUtil.EntityTypeName2CodeMap[entityName];
        var entityTypeCode = Payments.GetObjectTypeCode("defra_payment");

        // https://<orgname>.crm.dynamics.com/main.aspx?etc=<objecttypecode>&id=%7b<EntityRecordID>%7d&pagetype=entityrecord
        var serverUrl = Xrm.Page.context.getServerUrl();
        return serverUrl + "/main.aspx?etc=" + entityTypeCode + "&id=" + paymentGuid + "&pagetype=entityrecord";
    },


    // Function returns a CRM Entity Type code for a given entity name,
    // Used when generating CRM hyperlinks
    GetObjectTypeCode: function (entityName) {
        try {
            var lookupService = new RemoteCommand("LookupService", "RetrieveTypeCode");
            lookupService.SetParameter("entityName", entityName);
            var result = lookupService.Execute();

            if (result.Success && typeof result.ReturnValue == "number") {
                return result.ReturnValue;
            }
            else {
                return null;
            }
        }
        catch (ex) {
            throw ex;
        }
    },

    // Function parses the return URL querystring for the Data parameter
    // which holds the Payment Reference
    GetDataParam: function () {

        var vals = new Array();
        if (location.search != "") {
            vals = location.search.substr(1).split("&");
            for (var i in vals) {
                vals[i] = vals[i].replace(/\+/g, " ").split("=");
            }
            //look for the parameter named 'data'
            var found = false;
            for (var i in vals) {
                if (vals[i][0].toLowerCase() == "data") {
                    return vals[i][1];
                    found = true;
                    break;
                }
            }
            if (!found) { noParams(); }
        }
        else {
            noParams();
        }

        return null;
    },


    noParams: function () {
        PaymentError = "No data parameter was passed to this page";
        Payments.DisplayResult();
    },


    DisplayResult: function () {
        $('#paymentStatus').text(Payments.PaymentStatus);
        if (Payments.PaymentStatus === 'success') {
            $('#paymentStatus')
                .html('The payment with reference <strong>' + Payments.PaymentRef + '</strong> was <strong>successful</strong>. <br/>Please close this popup and review the Payment record.');
        }
        else if (Payments.PaymentStatus === 'error') {
            $('#paymentStatus')
                .html('There was an <strong>error</strong> processing the payment. <br/>Please close this popup window and retry the payment.');
        }
        else if (Payments.PaymentStatus === 'fail') {
            $('#paymentStatus')
                .html('The payment was not successful. <br/>Please close this popup window and retry the payment using another card.');
        } else {
            $('#paymentStatus')
                .html('The payment provider reported the transaction status of "' + Payments.PaymentStatus + '". <br/>Please close this popup window and retry the payment if appropriate.');
        }
        $('#errormessage').text(Payments.PaymentError);
    },
    SetReturnLink: function () {
        // TODO: Complete return URL.
        //$("a").attr("href", Payments.PaymentRef);
    },

    // Alert Payment form of change
    SendMessageToPayment: function () {
        window.opener.postMessage(Payments.PaymentRef, window.location.origin);
    }
}

document.onreadystatechange = function () {
    if (document.readyState === "complete") {
        Payments.OnLoad();
    }
}

//Added for cross browser support.
function setText(element, text) {
    if (typeof element.innerText != "undefined") {
        element.innerText = text;
    }
    else {
        element.textContent = text;
    }

}