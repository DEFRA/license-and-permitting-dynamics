// Application entity model at the L&P Organisation Level
namespace Model.Lp.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public partial class Application
    {
        /// <summary>
        /// Logical name for Application entity
        /// </summary>
        public const string EntityLogicalName = "defra_application";

        /// <summary>
        /// State field
        /// </summary>
        public const string State = "statecode";

        /// <summary>
        /// Nps Determination fiels
        /// </summary>
        public const string NpsDetermination = "defra_npsdetermination";

        /// <summary>
        /// Application Type OptionSet
        /// </summary>
        public const string ApplicationType = "defra_applicationtype";
    }
}
