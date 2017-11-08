var paymentNotificationID = 'PaymentReceivedWARNING';

function PaymentReceived(){
	if(Xrm.Page.ui.getFormType() != 1){
		var paymentRec = Xrm.Page.getAttribute('defra_paymentreceived');
		
		if(paymentRec != null && paymentRec.getValue() == false)
			Xrm.Page.ui.setFormNotification('The Application is not paid in full!', 'WARNING' , paymentNotificationID);
		else
			Xrm.Page.ui.clearFormNotification(paymentNotificationID);
	}
}
