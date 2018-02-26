function validateWithCompanyHouse() {
    CallCompaniesHouseAction();
}

function CallCompaniesHouseAction() {
    if (Xrm.Page.data.entity.getIsDirty() === true) {
        Xrm.Page.ui.setFormNotification("Please save the record first and try again.", "WARNING");
        return;
    };

    var actionName = "defra_companieshousevalidation";
    var accountId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');
    var compRegNum = Xrm.Page.getAttribute("defra_companyhouseid");

    if (compRegNum == null || compRegNum.getValue() === null) {
        Xrm.Page.ui.setFormNotification("Enter a company number.", "WARNING");
        return;
    }

    var uri = Xrm.Page.context.getClientUrl() + "/api/data/v8.2/accounts(" + accountId + ")/Microsoft.Dynamics.CRM." + actionName;

    var requestdata = JSON.stringify({
        "CompanyRegistrationNumber": compRegNum.getValue() + ""
    });

    $.ajax({
        url: uri,
        type: 'POST',
        data: requestdata,
        dataType: 'json',
        contentType: "application/json",
        async: false,
        success: function (data) {
            // Refresh page to get updated details
            Xrm.Page.data.refresh(false);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Xrm.Page.data.refresh(false);

            if (jqXHR.status === 404) {
                Xrm.Page.ui.setFormNotification("There was an error connecting to Companies House. Please contact your administrator referencing the following error message: 404", "ERROR");
            }
            else if (jqXHR.status === 500) {

                var result = JSON.parse(jqXHR.responseText);

                // Check if 404 error in message, this means Companies House could not find a company with the given co number
                if (result.error.message.substring(0, 3) === "404") {
                    Xrm.Page.ui.setFormNotification("Company number not found. Enter a valid company number.",
                        "WARNING");
                } else {
                    Xrm.Page.ui.setFormNotification("There was an error connecting to Companies House. Please contact your administrator referencing the following error message: " + result.error.message, "WARNING");
                }
            }
        }
    });
}

//http://crm2016/Waste/api/data/v8.2/accounts(BB4E6036-62C2-E711-9C0D-00155D014A07)/Microsoft.Dynamics.CRM.defra_companieshouseaccount