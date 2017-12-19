function validateWithCompanyHouse() 
{
    //Xrm.Page.ui.clearFormNotification("1");
    //Xrm.Page.ui.setFormNotification("Customer Details Are Correct.", "INFORMATION", "1");
	CallCompaniesHouseAction();
}

function CallCompaniesHouseAction()
{
	if(Xrm.Page.data.entity.getIsDirty() == true)
	{
		alert("Please save the form first!", "ERROR", "formDirty");
		return;
	};
	
	var actionName = "defra_companieshousevalidation";
	var accountId = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');
	var compRegNum = Xrm.Page.getAttribute("defra_companyhouseid");
	
	if(compRegNum == null || compRegNum.getValue() == null)
	{
		alert("No company reg number");
		return;
	}

	var uri = Xrm.Page.context.getClientUrl() + "/api/data/v8.2/accounts(" + accountId + ")/Microsoft.Dynamics.CRM." + actionName;
	
	var requestdata = JSON.stringify({
		//"Account": { "accountid" : "'{" + accountId + "}'", "@data.type" : "Microsoft.Dynamics.CRM.account" },
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
			
                //alert(data);
				 
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
				var responseText = JSON.stringify(jqXHR.responseText);
				
				//alert(jqXHR.status);
				//alert(responseText);
				
                //var responseText = responseText.slice(161, 255)
                if (jqXHR.status == 404) {
                    //window.parent.Xrm.Page.ui.setFormNotification("The URL requested wasn't valid. The server responded with a 404 error", "ERROR", "noAddress");
                }
                if (jqXHR.status == 500) {
                    //window.parent.Xrm.Page.ui.setFormNotification("Invalid postcode entered. The postcode must contain a minimum of the sector plus 1 digit of the district e.g. SO1", "ERROR", "noAddress");
                    //alert(responseText);
                    var result = JSON.parse(jqXHR.responseText);
                    alert(result.error.message);
                }
            }
        });
		
	Xrm.Page.data.refresh(false);
}

//http://crm2016/Waste/api/data/v8.2/accounts(BB4E6036-62C2-E711-9C0D-00155D014A07)/Microsoft.Dynamics.CRM.defra_companieshouseaccount