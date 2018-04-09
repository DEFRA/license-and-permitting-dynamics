var Payments = {

    PaymentRef: '',
    PaymentStatus: '',
    PaymentRecord: '',

    OnLoad: function () {

        // 1. Retrieve payment number from querystirng
        Payments.PaymentRef = Payments.getDataParam();

        // 4. Set-up return button
        Payments.GetPayment(Payments.PaymentRef);

        // 2. Call CRM Action to check payment status
        Payments.GetCardPaymentStatus(Payments.PaymentRef);



        // 3. Display Result
        //Payments.DisplayResult(status, PaymentRef);





    },

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
                    var paymentLink = Payments.GetApplicationRecordUrl(Payments.PaymentRecord.id);
                    Payments.SetReturnLink(paymentLink);
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();


    },


    GetCardPaymentStatus: function (paymentRefNumber) {


        var parameters = {};
        parameters.LookupByPaymentReference = paymentRefNumber;

        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/defra_get_payment_status", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    Payments.PaymentStatus = results.Status;
                    Payments.DisplayResult();
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send(JSON.stringify(parameters));

        /*
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
        */

    },


    GetApplicationRecordUrl: function (paymentGuid) {

        // var entityTypeCode = Mscrm.EntityPropUtil.EntityTypeName2CodeMap[entityName];
        var entityTypeCode = Payments.GetObjectTypeCode("defra_payment");


        // https://<orgname>.crm.dynamics.com/main.aspx?etc=<objecttypecode>&id=%7b<EntityRecordID>%7d&pagetype=entityrecord
        var serverUrl = Xrm.Page.context.getServerUrl();
        return serverUrl + "/main.aspx?etc=" + entityTypeCode + "&id=" + paymentGuid + "&pagetype=entityrecord";
    },



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



    CallAction: function (actionUrl, jsonData) {
        actionUrl = Xrm.Page.context.getClientUrl() + "/api/data/v8.2/" + actionUrl;
        $.ajax({
            url: actionUrl,
            async: false,
            type: "POST",
            data: jsonData,
            dataType: "json",
            beforeSend: function (x) {
                if (x && x.overrideMimeType) {
                    x.overrideMimeType("application/j-son;charset=UTF-8");
                }
            },
            success: function (result) {
                alert(result);
            },
            error: function (err) {
                var result = JSON.parse(err.responseText);
                alert(result.error.message);
            }
        });
    },

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

    GetReturnUrl: function (paymentReference) {
        Xrm.Page.context
            .getClientUrl() +
            "/WebResources/defra_/payments/return_from_portal.htm?ref=" +
            paymentReference;
    },



    getDataParam: function () {
        //Get the any query string parameters and load them
        //into the vals array

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
            if (!found)
            { noParams(); }
        }
        else {
            noParams();
        }

        return null;
    },

    parseDataValue: function (datavalue) {
        if (datavalue != "") {
            var vals = new Array();

            var message = document.createElement("p");
            setText(message, "These are the data parameters values that were passed to this page:");
            return message;


            document.body.appendChild(message);

            vals = decodeURIComponent(datavalue).split("&");
            for (var i in vals) {
                vals[i] = vals[i].replace(/\+/g, " ").split("=");
            }

            //Create a table and header using the DOM
            var oTable = document.createElement("table");
            var oTHead = document.createElement("thead");
            var oTHeadTR = document.createElement("tr");
            var oTHeadTRTH1 = document.createElement("th");
            setText(oTHeadTRTH1, "Parameter");
            var oTHeadTRTH2 = document.createElement("th");
            setText(oTHeadTRTH2, "Value");
            oTHeadTR.appendChild(oTHeadTRTH1);
            oTHeadTR.appendChild(oTHeadTRTH2);
            oTHead.appendChild(oTHeadTR);
            oTable.appendChild(oTHead);
            var oTBody = document.createElement("tbody");
            //Loop through vals and create rows for the table
            for (var i in vals) {
                var oTRow = document.createElement("tr");
                var oTRowTD1 = document.createElement("td");
                setText(oTRowTD1, vals[i][0]);
                var oTRowTD2 = document.createElement("td");
                setText(oTRowTD2, vals[i][1]);

                oTRow.appendChild(oTRowTD1);
                oTRow.appendChild(oTRowTD2);
                oTBody.appendChild(oTRow);
            }

            oTable.appendChild(oTBody);
            document.body.appendChild(oTable);
        }
        else {
            noParams();
        }
    },

    noParams: function () {
        var message = document.createElement("p");
        setText(message, "No data parameter was passed to this page");


        document.body.appendChild(message);
    },
    DisplayResult: function() {
        $('#paymentStatus').text(Payments.PaymentStatus);
    },
    SetReturnLink: function() {
        $("a").attr("href", Payments.PaymentRef);
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