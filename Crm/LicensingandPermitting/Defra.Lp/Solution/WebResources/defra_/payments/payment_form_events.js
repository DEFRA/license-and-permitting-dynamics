var Payments = {

    TakeCardPayment: function () {

        // Check form data is saved
        if (Xrm.Page.data.entity.getIsDirty() === true) {
            Xrm.Page.ui.setFormNotification("Please save the record first and try again.", "WARNING");
            return;
        };

        // Get payment record details
        var entityName = Xrm.Page.data.entity.getEntityName();
        var entityId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');
        var paymentAmount = Xrm.Page.getAttribute("defra_paymentvalue").getValue();
        var paymentDescription = Xrm.Page.getAttribute("defra_description").getValue();
        var paymentReference = Xrm.Page.getAttribute("defra_reference_number").getValue();
        var returnUrl = Payments.GetReturnUrl(paymentReference);

        // Direct call Create Payment Transaction Action
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

        // Redirect to Payment Portal
        if (actionResult) {

            // Todo: Check for errors
            // Todo: Save link to Payment Transaction

            // Redirect to next url
            var nextUrl = actionResult.PaymentNextUrlHref;

            window.open(nextUrl);
        } else {
            Xrm.Page.ui.setFormNotification("Payment transaction could not be completed. Please contact your system administrator with payment ref: " + paymentReference, "ERROR");
        }
    },


    GetCurrentRecordUrl: function () {
        var entityName = Xrm.Page.data.entity.getEntityName();
        // var entityTypeCode = Mscrm.EntityPropUtil.EntityTypeName2CodeMap[entityName];
        var entityTypeCode = GetObjectTypeCode(entityName);
        var entityId = Xrm.Page.data.entity.getId();

        // https://<orgname>.crm.dynamics.com/main.aspx?etc=<objecttypecode>&id=%7b<EntityRecordID>%7d&pagetype=entityrecord
        var serverUrl = Xrm.Page.context.getServerUrl();
        return serverUrl + "/main.aspx?etc=" + entityTypeCode + "&id=" + entityId + "&pagetype=entityrecord";
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


        req.onreadystatechange = function() {
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

    GetReturnUrl: function(paymentReference) {
        return Xrm.Page.context.getClientUrl() + "/WebResources/defra_/payments/return_from_portal.htm?Data=" + paymentReference;
    }
}
