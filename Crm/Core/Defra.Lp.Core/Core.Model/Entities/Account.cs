namespace Core.Model.Entities
{
    /// <summary>
    /// CRM Entity model for the Account Entity
    /// </summary>
    public class Account
    {
        // Entity
        public const string EntityLogicalName = "account";

        // Fields
        public static string AccountIdField = "accountid";
        public const string CompanyNumberField = "defra_companyhouseid";
        public const int CompanyNumberFieldLength = 8;
        public const string NameField = "name";
        public const int NameFieldLength = 160;
        public const string CompaniesHouseStatusField = "defra_companyhousestatus";
        public const string ValidatedField = "defra_validatedwithcompanyhouse";
        public const string StateField = "statecode";

        // Relationships
        public const string ParentChildAccountManyToManyRelationship = "defra_parent_child_account_relationship";
        public const string ParentChildAccountManyToManyRelationshipFromField = "accountidone";
        public const string ParentChildAccountManyToManyRelationshipToField = "accountidtwo";
        public const string ParentChildAccountManyToManyRelationshipAlias = "Members";
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