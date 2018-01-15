var paymentNotificationID = 'PaymentReceivedWARNING';
var dulymadechecklistNotificationID = 'DulymadeChecklistWARNING';

function PaymentReceived() {
    if (Xrm.Page.ui.getFormType() != 1) {
        var paymentRec = Xrm.Page.getAttribute('defra_paymentreceived');

        if (paymentRec != null && paymentRec.getValue() == false)
            Xrm.Page.ui.setFormNotification('The Application is not paid in full!', 'WARNING', paymentNotificationID);
        else
            Xrm.Page.ui.clearFormNotification(paymentNotificationID);

        // Show notification if Duly Made Checklist not completed but intelligence checks have been completed.
        //var completedDulyMade = Xrm.Page.getAttribute('defra_completeddulymade');
        //var raguCompleted = Xrm.Page.getAttribute('defra_raguscore');
        //if (raguCompleted != null) {
        //    if (completedDulyMade != null && completedDulyMade.getValue() == false && raguCompleted.getValue() != null)
        //        Xrm.Page.ui.setFormNotification('The Duly Made Checklist still needs to be completed!', 'WARNING', dulymadechecklistNotificationID);
        //    else
        //        Xrm.Page.ui.clearFormNotification(dulymadechecklistNotificationID);
        //}
    }
}