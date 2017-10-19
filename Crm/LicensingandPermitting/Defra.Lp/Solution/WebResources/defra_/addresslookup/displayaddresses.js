function displayAddresses() {
    var uri = Xrm.Page.context.getClientUrl() + "/api/data/v8.2/defra_postcodelookup";
    //encodeURI(uri+"defra_postcodelookup");
        window.parent.Xrm.Page.getAttribute("defra_name").setValue("");
        window.parent.Xrm.Page.getAttribute("defra_premises").setValue("");
        window.parent.Xrm.Page.getAttribute("defra_street").setValue("");

    
    jQuery('#the_address').empty();
     var postcode = window.parent.Xrm.Page.getControl("defra_postcode").getValue();
     
     var requestdata = JSON.stringify({"postcode" :  postcode });
     
     if(postcode==="")
     {

     }else{
   /*  var addressbaseAPI = "https://addressfacade.cloudapp.net/address-service/v1/addresses/postcode?key=client1&postcode=gu227uy"; */

    $.ajax({
        url: uri,
        type:'POST',
        data: requestdata,
        dataType: 'json',   
        contentType: "application/json",
        async: false,
        success: function(data){
    
            var addresses = JSON.parse(data.addresses);
                jQuery('#the_address').append(jQuery('<option></option').val(0).html(addresses.totalMatches + " addresses found"));
                window.parent.Xrm.Page.ui.clearFormNotification("noAddress"); 
                //   window.parent.Xrm.Page.getControl("defra_postcode").clearNotification();
                           $.each(addresses.results, function (i, results) {
                               jQuery('#the_address').append(
                              jQuery('<option></option').val(i).html(results.address)
                          ) 
                           }); 
    
            
        },
        error: function(jqXHR, textStatus, errorThrown ){
            
            var responseText = JSON.stringify(jqXHR.responseText);
            var responseText = responseText.slice(161,255)
            window.parent.Xrm.Page.ui.setFormNotification("Invalid postcode entered. " + responseText, "ERROR","noAddress");
    //    window.parent.Xrm.Page.getControl("defra_postcode").setNotification("No valid addresses found","ERROR");
        }
    });
 /*    $.post(uri,requestdata, function(data)
    {
        jQuery('#the_address').append(jQuery('<option></option').val(0).html(addressses.totalMatches + " addresses found"));
        
                   $.each(data.items, function (i, item) {
                       jQuery('#the_address').append(
                      jQuery('<option></option').val(i).html(item)
                  )
                   });
    }, 'jsonp');*/

    // jQuery('#the_address').append(jQuery('<option></option').val(0).html(addressses.totalMatches + " addresses found"));

    jQuery('select[name="siteAddress"]').change(function () {
        var addressSelected = jQuery("#the_address option:selected").text();
        
        var addressSeperated = addressSelected.split(',');
 

        window.parent.Xrm.Page.getAttribute("defra_name").setValue(addressSelected);
        window.parent.Xrm.Page.getAttribute("defra_premises").setValue(addressSeperated[1]);
        window.parent.Xrm.Page.getAttribute("defra_street").setValue(addressSeperated[2]);
    });
}
}

