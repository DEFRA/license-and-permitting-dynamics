namespace Core.Model.Entities
{
    /// <summary>
    /// CRM Entity model for the Account Entity
    /// </summary>
    class Account
    {
        public const string EntityLogicalName = "account";
        public static string AccountIdField = "accountid";
        public const string CompanyNumberField = "defra_companyhouseid";
        public const string NameField = "name";
        public const string CompaniesHouseStatusField = "defra_companyhousestatus";
        public const string ValidatedField = "defra_validatedwithcompanyhouse";
        public const string StateField = "statecode";

        public const string ParentChildAccountManyToManyRelationship = "defra_parent_child_account_relationship";
        
    }

    /// <summary>
    /// Account entity Organisation Type optionset values
    /// </summary>
    public enum AccountOrganisationTypes
    {
        LimitedCompany = 910400000,
        LimitedLiabilityPartnership = 910400005
    }
}