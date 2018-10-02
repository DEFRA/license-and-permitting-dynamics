// Location Detail entity model at the L&P Organisation Level
namespace Model.Lp.Crm
{
    /// <summary>
    /// CRM Location Detail entity model
    /// </summary>
    public partial class LocationDetail
    {
        /// <summary>
        /// Logical name for entity
        /// </summary>
        public const string EntityLogicalName = "defra_locationdetails";

        /// <summary>
        /// Alias to be used when linking
        /// </summary>
        public const string Alias = "locdetail";

        /// <summary>
        /// State field
        /// </summary>
        public const string State = "statecode";

        /// <summary>
        /// Primary name field
        /// </summary>
        public const string Name = "defra_name";

        /// <summary>
        /// Text Grid Ref
        /// </summary>
        public const string GridReference = "defra_gridreferenceid";

        /// <summary>
        /// Lookup to the Location Entity
        /// </summary>
        public const string Location = "defra_locationid";

        /// <summary>
        /// Lookup to the Address Entity
        /// </summary>
        public const string Adress = "defra_addressid";
    }
}
