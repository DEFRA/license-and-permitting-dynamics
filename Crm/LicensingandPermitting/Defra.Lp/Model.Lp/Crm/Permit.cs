// Permit entity model at the L&P Organisation Level
namespace Model.Lp.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public partial class Permit
    {
        /// <summary>
        /// Logical name for Application entity
        /// </summary>
        public const string EntityLogicalName = "defra_permit";

        /// <summary>
        /// State field
        /// </summary>
        public const string State = "statecode";

        /// <summary>
        /// Name field
        /// </summary>
        public const string Name = "defra_name";

        /// <summary>
        /// Permit Nuber field
        /// </summary>
        public const string PermitNumber = "defra_permitnumber";
    }
}