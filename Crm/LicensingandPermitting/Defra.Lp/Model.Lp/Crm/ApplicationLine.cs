// Application entity model at the L&P Organisation Level
namespace Model.Lp.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public partial class ApplicationLine
    {
        /// <summary>
        /// Logical name for Application entity
        /// </summary>
        public const string EntityLogicalName = "defra_applicationline";

        /// <summary>
        /// State field
        /// </summary>
        public const string State = "statecode";

        /// <summary>
        /// Entiy Id
        /// </summary>
        public const string ApplicationLineId = "defra_applicationlineid";

        /// <summary>
        /// Nps Determination fiels
        /// </summary>
        public const string NpsDetermination = "defra_npsdetermination";

        /// <summary>
        /// Lookup to the Application record
        /// </summary>
        public const string ApplicationId = "defra_applicationid";
    }

    /// <summary>
    /// Possible state codes for an application line
    /// </summary>
    public enum ApplicationLineStates
    {
        Active = 0,
        Inactive = 1
    }
}
