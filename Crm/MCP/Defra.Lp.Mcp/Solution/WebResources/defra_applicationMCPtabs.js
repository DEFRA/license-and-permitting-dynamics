var TAB_SUMMARY = "tab_general";
var TAB_OPERATOR = "tab_operator";
var TAB_RULESET = "tab_ruleset";
var TAB_PAYMENT = "tab_finance";
var TAB_DOCUMENTS = "Documents";

function getLastUsedTab() {

    try {

        var currentformtab = Xrm.Page.getAttribute("defra_currentformtab");

        if (currentformtab == null) {
            Xrm.Page.getAttribute("defra_currentformtab").setValue(TAB_SUMMARY);
        }
        else {

            Xrm.Page.ui.tabs.get(currentformtab.getValue()).setFocus();

        }
    }

    catch (err) {
        console.error('Error captured in getLastUsedTab: ' + err);
    }

}


function setLastUsedTab() {

    try {


        var tabs = Xrm.Page.ui.tabs.get();
        for (var i in tabs) {
           
            var tab = tabs[i];
            console.error('Error captured in getName: ' + tabs[i].getName());
                if (tab.getDisplayState() == 'expanded')
                {
                    console.error('Error tab.getName(): ' + tab.getName());
                    Xrm.Page.getAttribute("defra_currentformtab").setValue(tab.getName());
                }
                
            
        }

      
    }

    catch (err) {
        console.error('Error captured in setLastUsedTab: ' + err);
    }

}

