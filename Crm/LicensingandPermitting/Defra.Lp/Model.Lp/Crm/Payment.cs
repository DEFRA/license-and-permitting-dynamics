// Application entity model at the L&P Organisation Level
namespace Lp.Model.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public partial class Payment
    {

        /// <summary>
        /// State field
        /// </summary>
        public const string State = "statecode";

        /// <summary>
        /// Status
        /// </summary>
        public static string Status = "statuscode";

        /// <summary>
        /// Record owner
        /// </summary>
        public static string Owner = "ownerid";

        /// <summary>
        /// Value
        /// </summary>
        public const string PaymentValue = "defra_paymentvalue";

        /// <summary>
        /// Lookup to the Application record
        /// </summary>
        public const string ApplicationId = "defra_applicationid";
    }
}
