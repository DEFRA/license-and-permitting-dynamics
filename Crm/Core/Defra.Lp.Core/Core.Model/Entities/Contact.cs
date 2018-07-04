namespace Core.Model.Entities
{
    class Contact
    {
        public const string EntityLogicalName = "contact";
    }

    public enum ContactAccountRoleCode
    {
        Director = 910400000,
        LlpDesignatedMember = 910400001,
        LlpMember = 910400002
    }
}