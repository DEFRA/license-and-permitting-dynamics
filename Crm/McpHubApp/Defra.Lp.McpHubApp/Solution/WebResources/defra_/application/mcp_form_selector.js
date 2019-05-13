//  created by Kassim Hassan
// To switch form according to Status 
// For MCP Applications 


var DULY_MAKING_FORM = "MCP SR Duly Making";
var STANDARD_FORM = "MCP SR Application";


function OnLoad() {
  //  try {



        var statusCode = Xrm.Page.getAttribute("statuscode").getValue();
       
        // If status is not draft or received then show advance form
        switch (statusCode) {
            case 1:
                changeForm(STANDARD_FORM);
                return;
                break;
            case 910400000:
                changeForm(STANDARD_FORM);
                return;
                break;
            default:
                changeForm(DULY_MAKING_FORM);
        }

 if (Xrm.Page.getControl("processid")) {
var processId = 'B25365DD-9C97-4117-B443-AA75E25732D9' // Xrm.Page.getAttribute("processid").getValue();
alert(processId);
       if (processId != null) {
           Xrm.Page.ui.process.setVisible(false);
       }
   }

  Xrm.Page.ui.process.setVisible(false);

   // }

  //  catch (err) {
 //       console.error('Error captured in OnLoad: ', err);
 //   }
}


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