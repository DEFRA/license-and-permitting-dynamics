// Location entity model at the L&P Organisation Level
namespace Model.Lp.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public partial class Location
    {
        /// <summary>
        /// Logical name for Application entity
        /// </summary>
        public const string EntityLogicalName = "defra_location";

        /// <summary>
        /// State field
        /// </summary>
        public const string State = "statecode";

        /// <summary>
        /// Lookup to the Application
        /// </summary>
        public const string Application = "defra_applicationid";

        /// <summary>
        /// Lookup to the Permit Entity
        /// </summary>
        public const string Permit = "defra_permitid";
        
    }
}
