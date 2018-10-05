// PermitLine entity model at the L&P Organisation Level
namespace Lp.Model.Crm
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
        /// Record owner
        /// </summary>
        public static string Owner = "ownerid";

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

        /// <summary>
        /// Type of line, optionset
        /// </summary>
        public static string LineType = "defra_linetype";
    }

    /// <summary>
    /// LineType Optionset used in Permit and Application Lines
    /// </summary>
    public enum LineTypes
    {
        RegulatedFacility = 910400000,
        PositiveAdjustment = 910400001,
        NegativeAdjustment = 910400002
    }
}
