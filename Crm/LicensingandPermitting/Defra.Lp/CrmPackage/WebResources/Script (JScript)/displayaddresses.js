var addresses = null;

function displayAddresses() {
    var uri = Xrm.Page.context.getClientUrl() + "/api/data/v8.2/defra_postcodelookup";

    window.parent.Xrm.Page.ui.clearFormNotification("noAddress");
	
	ClearAll();
	
	window.parent.Xrm.Page.getAttribute("defra_fromaddresslookup").setValue(false);
	window.parent.Xrm.Page.getAttribute("defra_fromaddresslookup").fireOnChange();
	
	jQuery('#the_address').hide();
	jQuery('#the_address').empty();
	jQuery('#lkp_msg').html("");
	
    var postcode = window.parent.Xrm.Page.getAttribute("defra_postcode").getValue();
	
	if(postcode == null){
		return;
	}
	
    var requestdata = JSON.stringify({ "postcode": postcode });
				
    if (postcode === "") {

    } else {
		jQuery('#lkp_msg').html("Retrieving postcode addresses");
		
        $.ajax({
            url: uri,
            type: 'POST',
            data: requestdata,
            dataType: 'json',
            contentType: "application/json",
            async: false,
            success: function (data) {
			
				jQuery('#lkp_msg').html("");
                
				addresses = JSON.parse(data.addresses);
				
				if(addresses.totalMatches > 0) {
					jQuery('#lkp_msg').html(addresses.totalMatches + " addresses found");
					
					jQuery('#the_address').append(jQuery('<option></option').val(0).html("Please select an address"));

					$.each(addresses.results, function (i, results) {
						jQuery('#the_address').append(
							jQuery('<option></option').val(i).html(results.address)

						)

					});
					
					jQuery('#the_address').show();
				} 
            },
            error: function (jqXHR, textStatus, errorThrown) {
				
				jQuery('#lkp_msg').html("");
                
				var responseText = JSON.stringify(jqXHR.responseText);
                //var responseText = responseText.slice(161, 255)
                if (jqXHR.status == 404) {
                    window.parent.Xrm.Page.ui.setFormNotification("The URL requested wasn't valid. The server responded with a 404 error", "ERROR", "noAddress");
                }
                if (jqXHR.status == 500) {
                    //window.parent.Xrm.Page.ui.setFormNotification("Invalid postcode entered. The postcode must contain a minimum of the sector plus 1 digit of the district e.g. SO1", "ERROR", "noAddress");
                    var result = JSON.parse(jqXHR.responseText);
                    alert(result.error.message);
                }
            }
        });


        jQuery('select[name="siteAddress"]').change(function () {
            //Check if it is the first option
			if($("#the_address")[0].selectedIndex == 0){
				ClearAll();
				
				window.parent.Xrm.Page.getAttribute("defra_fromaddresslookup").setValue(false);
				window.parent.Xrm.Page.getAttribute("defra_fromaddresslookup").fireOnChange();
	
				return;
			}


            var addressSelected = jQuery("#the_address option:selected").text();
			window.parent.Xrm.Page.getAttribute("defra_name").setValue(addressSelected);
            
			$.each(addresses.results, function (i, results) {
				if(results.address == addressSelected)
				{
				    window.parent.Xrm.Page.getAttribute("defra_uprn").setValue(results.uprn.toString());
					window.parent.Xrm.Page.getAttribute("defra_premises").setValue(results.premises);   
                    window.parent.Xrm.Page.getAttribute("defra_street").setValue(results.street_address);
                    window.parent.Xrm.Page.getAttribute("defra_locality").setValue(results.locality);
                    window.parent.Xrm.Page.getAttribute("defra_towntext").setValue(results.city);
                }
			});

			window.parent.Xrm.Page.getAttribute("defra_fromaddresslookup").setValue(true);
			window.parent.Xrm.Page.getAttribute("defra_fromaddresslookup").fireOnChange();
        });
    }
}

function ClearAll() {
    window.parent.Xrm.Page.getAttribute("defra_uprn").setValue(null);
	window.parent.Xrm.Page.getAttribute("defra_name").setValue(null);
	window.parent.Xrm.Page.getAttribute("defra_premises").setValue(null);   
	window.parent.Xrm.Page.getAttribute("defra_street").setValue(null);
    window.parent.Xrm.Page.getAttribute("defra_locality").setValue(null);
    window.parent.Xrm.Page.getAttribute("defra_towntext").setValue(null);
}