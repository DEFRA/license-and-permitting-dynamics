var companiesHouseNotificationID = 'CompanieHouseValidationWARNING';

function CompanyHouseValidated() {
    if (Xrm.Page.ui.getFormType() != 1) {
        var validated = Xrm.Page.getAttribute('defra_validatedwithcompanyhouse');
        var companyHouseId = Xrm.Page.getAttribute('defra_companyhouseid');

        if (validated != null && validated.getValue() == false) 
            if (companyHouseId && companyHouseId.getValue() != null && companyHouseId.getValue().length > 0)
                Xrm.Page.ui.setFormNotification('This Account has not been validated with Companies House!', 'WARNING', companiesHouseNotificationID);
            else
                Xrm.Page.ui.clearFormNotification(companiesHouseNotificationID);
        else
            Xrm.Page.ui.clearFormNotification(companiesHouseNotificationID);
    }
}