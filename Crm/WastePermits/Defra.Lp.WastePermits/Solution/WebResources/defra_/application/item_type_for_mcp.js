//  created by Kassim Hassan
// To switch form according to Status 
// For MCP Application lines 

const MCP_SR_FORM = "MCP - Standard";
const WASTE_FORM = "WASTE - BESPOKE";
const MCP_BESPOKE_FORM = "MCP - BESPOKE";
const ITEM_ID = "E3BF48FA-55E7-E811-A985-000D3AB311F1";
const ITEM_NAME = "Standard Rule";

const MCP_BESPOKE = "C994C55B-7B2F-E911-A9A2-000D3AB31AD6";
const MCP_SR = "B724DA4F-162E-E911-A98D-000D3AB311F1";
const WASTE_BT = "E563D143-162E-E911-A98D-000D3AB311F1";

// Created by Kassim Hassan to retrive regime from aplication entity
// ********************* For MCP New applications process ******************
function OnLoad() {

    var Application = {
        // The CRM Application ID
        ApplicationID: '',
        ApplicationBusinessTrack: ''
    }

    try {
        Application.ApplicationID = Xrm.Page.getAttribute("defra_applicationid").getValue();
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/defra_applications?$top=50&$select=_defra_businesstrackid_value&$filter=defra_applicationid%20eq%20" + Application.ApplicationID[0].id + "%20", true);
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
                    Application.ApplicationBusinessTrack = results.value[0]["_defra_businesstrackid_value"];
                    switch (Application.ApplicationBusinessTrack.toUpperCase()) {
                        case MCP_BESPOKE:  //MCP_BESPOKE
                            changeForm(MCP_BESPOKE_FORM, Application.ApplicationID);
                            break;
                        case MCP_SR:  //MCP_SR
                           
                            changeForm(MCP_SR_FORM, Application.ApplicationID);
                            if (Xrm.Page.getAttribute("defra_item_type").getValue() == null) {
                                var item_type = new Array();
                                item_type[0] = new Object();
                                item_type[0].id = ITEM_ID;
                                item_type[0].name = ITEM_NAME;
                                item_type[0].entityType = "defra_itemtype";
                                Xrm.Page.getAttribute("defra_item_type").setValue(item_type);
                            }
                            break;
                        case WASTE_BT:  //WASTE_BESPOKE
                            changeForm(WASTE_FORM, Application.ApplicationID);
                            break;
                        default:
                            return;
                    };

                } else {
                    console.log('Error in New MCP application line XMLHttpRequest process. ' + this.statusText);
                }
            }
        };
        req.send();
    }
    catch (err) {
        console.log('Error in New application line MCP process. ' + err)
    }
};


function changeForm(formName, applicationId) {
    try {
        var currentForm = Xrm.Page.ui.formSelector.getCurrentItem();
        var availableForms = Xrm.Page.ui.formSelector.items.get();
        if (currentForm.getLabel().toLowerCase() != formName.toLowerCase()) {
            for (var i in availableForms) {
                var form = availableForms[i];
                // try to find a form based on the name
                if (form.getLabel().toLowerCase() == formName.toLowerCase()) {
                    form.navigate();
                    Xrm.Page.getAttribute("defra_applicationid").setValue(applicationId);
                    return true;
                }
            }
        }
    }
    catch (err) {
        console.error('Error captured in changeForm: ' + err);
    }
};

