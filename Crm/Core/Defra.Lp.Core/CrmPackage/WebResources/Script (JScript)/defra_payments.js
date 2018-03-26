// ------------------------------------------------------
// JavaScript CRM Web Resource receives
// payment parameters such as amount, reference
// and initiates the payment process by calling the 
// CreatePayment action. It then redirects to the payment 
// portal url.
// ------------------------------------------------------


// 1. Receive payment parameters in Query String
// 2. Call CreatePayment action to prepare payment portal
// 3. Redirect to the URL returned by the action, or display error 