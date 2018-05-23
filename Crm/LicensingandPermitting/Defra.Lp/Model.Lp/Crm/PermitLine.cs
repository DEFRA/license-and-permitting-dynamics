// PermitLine entity model at the L&P Organisation Level
namespace Model.Lp.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public partial class PermitLine
    {
        /// <summary>
        /// Logical name for Application entity
        /// </summary>
        public const string EntityLogicalName = "defra_permit_lines";

        /// <summary>
        /// State field
        /// </summary>
        public const string State = "statecode";

        /// <summary>
        /// Permit Lookup
        /// </summary>
        public const string Permit = "defra_permitid";

        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "defra_name";

        /// <summary>
        /// Permit Type
        /// </summary>
        public const string PermitType = "defra_permittype";

        /// <summary>
        /// Standard Rule
        /// </summary>
        public const string StandardRule = "defra_standardruleid";
    }
}
