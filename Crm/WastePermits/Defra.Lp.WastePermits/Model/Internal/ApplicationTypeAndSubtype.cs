namespace WastePermits.Model.Internal
{
    using System;

    /// <summary>
    /// Class returns an application type and subtype
    /// </summary>
    public class ApplicationTypesAndOwners
    {
        public int? ApplicationType { get; set; }
        public Guid? ApplicationSubType { get; set; }
        public Guid? OwningUser { get; set; }
        public Guid? OwningTeam { get; set; }
    }
}
