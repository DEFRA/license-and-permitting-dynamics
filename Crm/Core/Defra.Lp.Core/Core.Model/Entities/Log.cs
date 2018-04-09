namespace Core.Model.Entities
{
    public class Log
    {
        public const string EntityLogicalName = "defra_log";

        public const string Status = "statuscode";

        /// <summary>
        /// Summary field 2000 characters long
        /// </summary>
        public const string Summary = "defra_summary"; 

        /// <summary>
        /// Multiline text field 10000 characters long
        /// </summary>
        public const string Detail = "defra_detail";


        public const string SourceSystem = "defra_sourcesystem";

        public const string TargetSystem = "defra_targetsystem";

        public const string Type = "defra_type";

        public const string NextAction = "defra_nextaction";

        public const string Importance = "defra_importance";

        public const string RelatedEntityName = "defra_relatedentityname";

        public const string RelatedEntityId= "defra_relatedentityid";

        public const string RelatedEntityReference = "defra_relatedentityreference";
    }
}
