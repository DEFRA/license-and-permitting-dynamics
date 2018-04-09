// Application Price entity model at the L&P Organisation Level
namespace Model.Lp.Crm
{
    /// <summary>
    /// CRM Application Price entity model
    /// </summary>
    public class ApplicationPrice
    {
        /// <summary>
        /// Logical name for Application Price entity
        /// </summary>
        public const string EntityLogicalName = "defra_applicationprice";

        /// <summary>
        /// State field
        /// </summary>
        public const string State = "statecode";

        /// <summary>
        /// Cost
        /// </summary>
        public const string Price = "defra_price";

        /// <summary>
        /// Application Type OptionSet
        /// </summary>
        public const string ApplicationType = "defra_applicationtype";
    }
}
