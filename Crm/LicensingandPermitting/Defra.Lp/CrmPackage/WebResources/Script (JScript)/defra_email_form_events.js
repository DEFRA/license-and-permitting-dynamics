//Sets the email From field to the user primary team queue
function SetPrimaryTeamQueue(){
	//If it is a new email form
	if(Xrm.Page.ui.getFormType() == 1) {
		var fromLkp = Xrm.Page.getAttribute("from").getValue();
		
		if(fromLkp[0].entityType != "queue") {
			//Clear the from field
			Xrm.Page.getAttribute("from").setValue(null);
			
			var systemUserId = Xrm.Page.context.getUserId().replace('{', '').replace('}', '');
			
			var uri = Xrm.Page.context.getClientUrl() + "/api/data/v8.2/systemusers(" + systemUserId + ")?$select=defra_DefaultTeamQueue&$expand=defra_DefaultTeamQueue($select=name)";
			
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
				Xrm.Page.getAttribute("from").setValue(lookupReference);
			}
		}
	}
	
	Xrm.Page.getControl("from").setDisabled(true);
}

