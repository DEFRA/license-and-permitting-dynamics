namespace CardPayments.Interfaces
{
    using Model;
    
    /// <summary>
    /// Interface defines the methods used to process credit card payments
    /// </summary>
    interface ICardPaymentProvider
    {
        /// <summary>
        /// Method initiates the card payment process
        /// </summary>
        /// <param name="request">Card payment request including amount and reference</param>
        void CreatePayment(CreatePaymentRequest request);

        /// <summary>
        /// Method checks the status of a card payment
        /// </summary>
        /// <param name="request"></param>
        void FindPayment(FindPaymentRequest request);
    }
}