namespace Core.Model.Entities
{
    /// <summary>
    /// CRM Entity model for the Contact entity
    /// </summary>
    public class Contact
    {
        public const string EntityLogicalName = "contact";

        public const string FromCompaniesHouseField = "defra_fromcompanieshouse";

        public const string AccountRoleCodeField = "accountrolecode";

        public const string FirstNameField = "firstname";

        public const int FirstNameFieldLength = 50;

        public const string LastNameField = "lastname";

        public const int LastNameFieldLength = 50;

        public const string JobTitleField = "jobtitle";

        public const int JobTitleFieldLength = 100;

        public const string DobMonthCompaniesHouseField = "defra_dobmonthcompanieshouse";

        public const string DobYearCompaniesHouseField = "defra_dobyearcompanieshouse_text";

        public const string ResignedOnCompaniesHouseField = "defra_resignedoncompanieshouse";

        public const string ParentCustomerIdField = "parentcustomerid";
    }

    /// <summary>
    /// accountrolecode optionset
    /// </summary>
    public enum ContactAccountRoleCode
    {
        Director = 910400000,
        LlpDesignatedMember = 910400001,
        LlpMember = 910400002
    }
}