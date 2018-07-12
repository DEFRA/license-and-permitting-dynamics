namespace Model.Lp.Crm
{
    public class Case
    {
        /// <summary>
        /// Logical name for annotation entity
        /// </summary>
        public const string EntityLogicalName = "incident";

        /// <summary>
        /// Regarding Object Id field name
        /// </summary>
        public const string RegardingObjectId = "regardingobjectid";

        /// <summary>
        /// Case primary key field name
        /// </summary>
        public const string IncidentId = "incidentid";

        /// <summary>
        /// Case Type option set field name
        /// </summary>
        public const string CaseType = "casetypecode";

        /// <summary>
        /// Title field name
        /// </summary>
        public const string Title = "title";

        /// <summary>
        /// Field name of lookup to Application
        /// </summary>
        public const string Application = "defra_application";
    }
}