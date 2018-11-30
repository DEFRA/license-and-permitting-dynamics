// Application entity model at the L&P Organisation Level
namespace Lp.Model.Crm
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
        /// Record owner
        /// </summary>
        public static string Owner = "ownerid";

        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "defra_name";

        /// <summary>
        /// Permit Type
        /// </summary>
        public const string PermitType = "defra_permittype";

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

        /// <summary>
        /// Line Cost
        /// </summary>
        public const string Value = "defra_value";

        /// <summary>
        /// Lookup
        /// </summary>
        public static string StandardRule = "defra_standardruleid";

        /// <summary>
        /// Application Line Type
        /// </summary>
        public const string LineType = "defra_linetype";

        /// <summary>
        /// Lookup
        /// </summary>
        public static string ItemId = "defra_itemid";

        /// <summary>
        /// Permit lookup
        /// </summary>
        public static string PermitId = "defra_permitid";
    }

    /// <summary>
    /// Possible state codes for an application line
    /// </summary>
    public enum ApplicationLineStates
    {
        Active = 0,
        Inactive = 1
    }

    /// <summary>
    /// Possible option set values for Lien Type
    /// </summary>
    public enum ApplicationLineTypeValues
    {
        RegulatedFacility = 910400000,
        PositiveAdjustment = 910400001,
        NegativeAdjustment = 910400002
    }
}
