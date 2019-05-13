// ** DRAFT CODE ** Created by Kassim Hassan for OnSave and OnLOad to show relevent sections

function OnLoad() {
    defineresponsection();
}

function defineresponsection() {
    // ***** MCP SR service**************************************************************************
    // *** Created by Kassim Hassan@DEFRA (04/05/2019)
    // ***** Setting UI business rules when a new application is created.
    // ******* NEEDS CODE REFACTORING ******************
    // **********************************************************************************************
    try {
        var tabOperator = Xrm.Page.ui.tabs.get("tab_operator");
        var tabRuleset = Xrm.Page.ui.tabs.get("tab_ruleset");
        var tabPayment = Xrm.Page.ui.tabs.get("tab_finance");
        var tabAdmincheck = Xrm.Page.ui.tabs.get("tab_admincheck");
        var tabDulyMaking = Xrm.Page.ui.tabs.get("tab_dulymaking");
        var tabDetermination = Xrm.Page.ui.tabs.get("tab_determination");
        var tabApproval = Xrm.Page.ui.tabs.get("tab_approval");
        var tabIssuePrmit = Xrm.Page.ui.tabs.get("tab_issuepermit");

        var statusCode = Xrm.Page.getAttribute("statuscode").getValue();
        var submittedOn = Xrm.Page.getAttribute("defra_submittedon").getValue();
        var paymentRequired = Xrm.Page.getAttribute("defra_payment_required").getValue();
        var paymentToPay = Xrm.Page.getAttribute("defra_balance_payments").getValue();

        switch (statusCode) {
            case 910400000:  //Received
                if (!submittedOn) {
                    tabOperator.setVisible(false);
                    tabRuleset.setVisible(false);
                    tabPayment.setVisible(false);
                    tabAdmincheck.setVisible(false);
                    tabDulyMaking.setVisible(false);
                    tabDetermination.setVisible(false);
                    tabApproval.setVisible(false);
                    tabIssuePrmit.setVisible(false);
                }
                else {
                    tabOperator.setVisible(true);
                    tabRuleset.setVisible(true);
                    tabPayment.setVisible(true);
                    tabRuleset.setFocus();

                    if (!paymentRequired && paymentToPay > 0) {
                        Xrm.Page.getAttribute("statuscode").setValue(910400015);
                        tabAdmincheck.setVisible(true);
                        tabAdmincheck.setFocus();
                    }
                    else if (!paymentRequired) {
                        tabRuleset.setFocus();
                    }
                    else {
                        tabPayment.setFocus();
                    }
                }
                break;
            case 910400015:  //Admin check
                tabOperator.setVisible(true);
                tabRuleset.setVisible(true);
                tabPayment.setVisible(true);
                tabAdmincheck.setVisible(true);
                tabDulyMaking.setVisible(true);
                tabDetermination.setVisible(true);
                tabAdmincheck.setFocus();
                break;
            case 910400001:  //Duly making
                tabOperator.setVisible(true);
                tabRuleset.setVisible(true);
                tabPayment.setVisible(true);
                tabAdmincheck.setVisible(true);
                tabDulyMaking.setVisible(true);
                tabDetermination.setVisible(true);
                tabApproval.setVisible(true);
                tabIssuePrmit.setVisible(true);
                tabDulyMaking.setFocus();
                break;
            case 910400002:  //Determination
                tabOperator.setVisible(true);
                tabRuleset.setVisible(true);
                tabPayment.setVisible(true);
                tabAdmincheck.setVisible(true);
                tabDulyMaking.setVisible(true);
                tabDetermination.setVisible(true);
                tabApproval.setVisible(true);
                tabIssuePrmit.setVisible(true);
                tabDetermination.setFocus();
                break;
            default:
                tabOperator.setVisible(true);
                tabRuleset.setVisible(true);
                tabPayment.setVisible(true);
                tabAdmincheck.setVisible(true);
                tabDulyMaking.setVisible(true);
                tabDetermination.setVisible(true);
                tabApproval.setVisible(true);
                tabIssuePrmit.setVisible(true);
                tabOperator.setFocus();
        }

    }
    catch (err) {
        console.error('Error captured in define response action: ', err);
    }
}