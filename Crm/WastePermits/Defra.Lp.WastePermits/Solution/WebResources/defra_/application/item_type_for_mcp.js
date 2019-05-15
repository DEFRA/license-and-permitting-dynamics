//  created by Kassim Hassan
// To switch form according to Status 
// For MCP Application lines 

const MCP_FORM = "MCP - Standard";
const WASTE_FORM = "WASTE - BESPOKE";
const ITEM_ID = "E3BF48FA-55E7-E811-A985-000D3AB311F1";
const ITEM_NAME = "Standard Rule";

// Created by Kassim Hassan to retrive regime from aplication entity
// ********************* For MCP New applications process ******************
function OnLoad() {

    var Application = {
        // The CRM Application ID
        ApplicationID: '',
        ApplicationRegime: ''
    }

    try
    {
    Application.ApplicationID = Xrm.Page.getAttribute("defra_applicationid").getValue();
    var req = new XMLHttpRequest();
    req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/defra_applications?$select=defra_applicationnumber&$expand=defra_regimeid($select=defra_name)&$filter=defra_applicationid%20eq%20" + Application.ApplicationID[0].id + "%20", true);
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
                console.log(results.value);
                Application.ApplicationRegime = results.value[0]["defra_regimeid"]["defra_name"];
                
                if (Application.ApplicationRegime == 'MCP') {
                    changeForm(MCP_FORM);

                    if (Xrm.Page.getAttribute("defra_item_type").getValue() == null) {
                        var item_type = new Array();
                        item_type[0] = new Object();
                        item_type[0].id = ITEM_ID;
                        item_type[0].name = ITEM_NAME;
                        item_type[0].entityType = "defra_itemtype";
                        Xrm.Page.getAttribute("defra_item_type").setValue(item_type);
                    }
                }
                else
                {
                    changeForm(WASTE_FORM); 
                }
               
            } else {
                alert('Error in New MCP application line XMLHttpRequest process. ' + this.statusText);
            }
        }
    };
    req.send();
}
catch (err) {
    alert('Error in New application line MCP process. ' + err)
}
};


function changeForm(formName) {
    try {
        var currentForm = Xrm.Page.ui.formSelector.getCurrentItem();
        var availableForms = Xrm.Page.ui.formSelector.items.get();
        if (currentForm.getLabel().toLowerCase() != formName.toLowerCase()) {
            for (var i in availableForms) {
                var form = availableForms[i];
                // try to find a form based on the name
                if (form.getLabel().toLowerCase() == formName.toLowerCase()) {
                    form.navigate();
                    return true;
                }
            }
        }
    }
    catch (err) {
        console.error('Error captured in changeForm: ' + err);
    }
};

