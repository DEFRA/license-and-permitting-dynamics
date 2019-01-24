//Sets the email From field to the user primary team queue
function SetPrimaryTeamQueue(executionContext) {

    // Check if the form is for a "New" or "Reply" (i.e. Update) email
    var formContext = executionContext.getFormContext();
    var formType = formContext.ui.getFormType();

    if (formType == 1 || formType == 2) {
        
        var fromLkp = formContext.getAttribute("from").getValue();
		
		if(fromLkp[0].entityType != "queue") {
			//Clear the from field
		    formContext.getAttribute("from").setValue(null);
			
		    var systemUserId = formContext.context.getUserId().replace('{', '').replace('}', '');

		    var globalContext = Xrm.Utility.getGlobalContext();
		    var uri = globalContext.getClientUrl() + "/api/data/v8.2/systemusers(" + systemUserId + ")?$select=defra_DefaultTeamQueue&$expand=defra_DefaultTeamQueue($select=name)";
			
			var queueId = null;
			var queueName = null;
			
			//Retrieve the user primary team queue from the user record
			$.ajax({
				url: uri,
				type: 'GET',
				dataType: 'json',
				contentType: "application/json",
				async: false,
				success: function (data) {
					queueName = data.defra_DefaultTeamQueue.name;
					queueId = data.defra_DefaultTeamQueue.queueid;
				},
				error: function (jqXHR, textStatus, errorThrown) {
					//alert(JSON.parse(jqXHR.responseText).error.message);
					alert("An error has occurred retrieving the system user default mailbox!");
				}
			});
			
			if(queueId != null && queueName != null) {
				//Set the From field value with the retrieved queue
				var lookupReference = [];
				lookupReference[0] = {};
				lookupReference[0].id = queueId;
				lookupReference[0].entityType = "queue";
				lookupReference[0].name = queueName;
			    formContext.getAttribute("from").setValue(lookupReference);
			}
		}
	}
	
    formContext.getControl("from").setDisabled(true);
}

//Sets the email From field to the user primary team queue
function SetToField(executionContext) {

    var formContext = executionContext.getFormContext();

    // 1. Validation

    //If it is a new email form
    var formType = formContext.ui.getFormType();
    if (formType !== 1) {
        // Not a new email form
        return;
    }

    // TO field already set?
    var toLookup = formContext.getAttribute("to").getValue();
    if (toLookup && toLookup.length > 0) {
        return;
    }

    // Regarding field not set?
    var regardingLookup = formContext.getAttribute("regardingobjectid").getValue();
    if (!regardingLookup || regardingLookup.length < 1) {
        return;
    }

    // Regarding not an application?
    if (regardingLookup[0].entityType !== 'defra_application') {
        return;
    }

    // 2. Get Application Primary Contact
    var globalContext = Xrm.Utility.getGlobalContext();
    var uri = globalContext.getClientUrl() +
        "/api/data/v8.2/defra_applications?$select=_defra_primarycontactid_value&$filter=defra_applicationid eq " +
        regardingLookup[0].id.replace('{', '').replace('}', '');

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: uri,
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
            XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("Prefer", "odata.include-annotations=\"*\",odata.maxpagesize=1");
        },
        async: true,
        success: function (data) {

            if (data && data.value && data.value.length > 0) {

                // 3. Set TO field to Primary Contact
                var _defra_primarycontactid_value = data.value[0]["_defra_primarycontactid_value"];
                var _defra_primarycontactid_value_formatted = data.value[0]["_defra_primarycontactid_value@OData.Community.Display.V1.FormattedValue"];
                var _defra_primarycontactid_value_lookuplogicalname = data.value[0]["_defra_primarycontactid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];

                if (data.value[0] && _defra_primarycontactid_value_lookuplogicalname) {
                    var lookupReference = [];
                    lookupReference[0] = {};
                    lookupReference[0].id = _defra_primarycontactid_value;
                    lookupReference[0].entityType = _defra_primarycontactid_value_lookuplogicalname;
                    lookupReference[0].name = _defra_primarycontactid_value_formatted;
                    formContext.getAttribute("to").setValue(lookupReference);
                }
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
        }
    });
}

