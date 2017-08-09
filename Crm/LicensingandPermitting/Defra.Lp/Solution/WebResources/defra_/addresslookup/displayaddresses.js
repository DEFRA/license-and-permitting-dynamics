function displayAddresses() {
    var addressses = { "totalMatches": 3, "startMatch": 1, "endMatch": 3, "uri_to_supplier": "https://api.ordnancesurvey.co.uk/places/v1/addresses/postcode?lr=EN&postcode=bs15ah&maxresults=100&dataset=DPA", "uri_from_client": "stub", "results": [{ "uprn": 340116, "address": "ENVIRONMENT AGENCY, HORIZON HOUSE, DEANERY ROAD, BRISTOL, BS1 5AH", "organisation": "ENVIRONMENT AGENCY", "premises": "HORIZON HOUSE", "street_address": "DEANERY ROAD", "locality": null, "city": "BRISTOL", "postcode": "BS1 5AH", "x": "358205.03", "y": "172708.07", "coordinate_system": null, "state_date": "12/10/2009", "blpu_state_code": null, "postal_address_code": null, "logical_status_code": null, "source_data_type": "dpa" }, { "uprn": 10091760640, "address": "HARMSEN GROUP, TRIODOS BANK, DEANERY ROAD, BRISTOL, BS1 5AH", "organisation": "HARMSEN GROUP", "premises": "TRIODOS BANK", "street_address": "DEANERY ROAD", "locality": null, "city": "BRISTOL", "postcode": "BS1 5AH", "x": "358130.1", "y": "172687.87", "coordinate_system": null, "state_date": null, "blpu_state_code": null, "postal_address_code": null, "logical_status_code": null, "source_data_type": "dpa" }, { "uprn": 340117, "address": "THRIVE RENEWABLES PLC, DEANERY ROAD, BRISTOL, BS1 5AH", "organisation": "THRIVE RENEWABLES PLC", "premises": null, "street_address": "DEANERY ROAD", "locality": null, "city": "BRISTOL", "postcode": "BS1 5AH", "x": "358130.1", "y": "172687.87", "coordinate_system": null, "state_date": "12/10/2009", "blpu_state_code": null, "postal_address_code": null, "logical_status_code": null, "source_data_type": "dpa" }] };
    // var postcode = Xrm.Page.getControl("defra_postcode").getValue();
    // var addressbaseAPI = "https://addressfacade.cloudapp.net/address-service/v1/addresses/postcode?key=client1&postcode=gu227uy";
    // $.getJSON(addressbaseAPI)
    //   .done(function (data) {
    //       $.each(data.items, function (i, item) {
    //           jQuery('#the_address').append(
    //           jQuery('<option></option').val(i).html(item)
    //       )
    //       });
    //   });
    jQuery('#the_address').empty();
    jQuery('#the_address').append(jQuery('<option></option').val(0).html(addressses.totalMatches + " addresses found"));
    $.each(addressses.results, function (i, item) {
        jQuery('#the_address').append(
            jQuery('<option></option').val(i).html(item.address)
        )
    });
    jQuery('select[name="siteAddress"]').change(function () {
        var addressSelected = jQuery("#the_address option:selected").text();
        
        var addressSeperated = addressSelected.split(',');
 

        window.parent.Xrm.Page.getAttribute("defra_name").setValue(addressSelected);
        window.parent.Xrm.Page.getAttribute("defra_premises").setValue(addressSeperated[1]);
        window.parent.Xrm.Page.getAttribute("defra_street").setValue(addressSeperated[2]);
    });
}

