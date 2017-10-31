function displayAddresses() {
    var uri = Xrm.Page.context.getClientUrl() + "/api/data/v8.2/defra_postcodelookup";
    //encodeURI(uri+"defra_postcodelookup");
            window.parent.Xrm.Page.getControl("defra_name").setDisabled(false);
            window.parent.Xrm.Page.getControl("defra_premises").setDisabled(false);
            window.parent.Xrm.Page.getControl("defra_street").setDisabled(false);
            window.parent.Xrm.Page.getControl("defra_locality").setDisabled(false);
            window.parent.Xrm.Page.getControl("defra_towntext").setDisabled(false);

    jQuery('#the_address').empty();
    var postcode = window.parent.Xrm.Page.getControl("defra_postcode").getValue();
    var requestdata = JSON.stringify({ "postcode": postcode });

    if (postcode === "") {

    } else {
        /*  var addressbaseAPI = "https://addressfacade.cloudapp.net/address-service/v1/addresses/postcode?key=client1&postcode=gu227uy"; */

        $.ajax({
            url: uri,
            type: 'POST',
            data: requestdata,
            dataType: 'json',
            contentType: "application/json",
            async: false,
            success: function (data) {

                var addresses = JSON.parse(data.addresses);

                jQuery('#the_address').append(jQuery('<option></option').val(0).html(addresses.totalMatches + " addresses found"));
                window.parent.Xrm.Page.ui.clearFormNotification("noAddress");
                $.each(addresses.results, function (i, results) {
                    jQuery('#the_address').append(
                        jQuery('<option></option').val(i).html(results.address)

                    )

                });


            },
            error: function (jqXHR, textStatus, errorThrown) {

                var responseText = JSON.stringify(jqXHR.responseText);
                //var responseText = responseText.slice(161, 255)
                if (jqXHR.status == 404) {
                    window.parent.Xrm.Page.ui.setFormNotification("The URL requested wasn't valid. The server responded with a 404 error", "ERROR", "noAddress");
                }
                if (jqXHR.status == 500) {
                    window.parent.Xrm.Page.ui.setFormNotification("Invalid postcode entered. The postcode must contain a minimum of the sector plus 1 digit of the district e.g. SO1", "ERROR", "noAddress");
                }
            }
        });


        jQuery('select[name="siteAddress"]').change(function () {
            //jQuery("#the_address").children().first().remove();


            var addressSelected = jQuery("#the_address option:selected").text();


             $.ajax({
            url: uri,
            type: 'POST',
            data: requestdata,
            dataType: 'json',
            contentType: "application/json",
            async: false,
            success: function (data) {

                var addresses = JSON.parse(data.addresses);

                $.each(addresses.results, function (i, results) {
                    if(results.address == addressSelected)
                    {
                      window.parent.Xrm.Page.getAttribute("defra_premises").setValue(results.premises);   
                      window.parent.Xrm.Page.getAttribute("defra_street").setValue(results.street_address);
                      window.parent.Xrm.Page.getAttribute("defra_locality").setValue(results.locality);
                      window.parent.Xrm.Page.getAttribute("defra_towntext").setValue(results.city);
                    }

                    

                });


            }
    
        });
            window.parent.Xrm.Page.getControl("defra_name").setDisabled(true);
            window.parent.Xrm.Page.getControl("defra_premises").setDisabled(true);
            window.parent.Xrm.Page.getControl("defra_street").setDisabled(true);
            window.parent.Xrm.Page.getControl("defra_locality").setDisabled(true);
            window.parent.Xrm.Page.getControl("defra_towntext").setDisabled(true);
            window.parent.Xrm.Page.getAttribute("defra_name").setValue(addressSelected);

        });
    }
}

