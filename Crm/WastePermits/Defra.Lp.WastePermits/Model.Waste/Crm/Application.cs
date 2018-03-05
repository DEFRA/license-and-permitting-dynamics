// Application entity model at the Wate Organisation Level
namespace Model.Waste.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public class Application : Lp.Crm.Application
    {
        /// <summary>
        /// Boolean field indicating lines exist
        /// </summary>
        public const string ActiveLinesExist = "defra_activelinesexist";

        /// <summary>
        /// Location Screening Required Boolean field
        /// </summary>
        public const string LocationScreeningRequired = "defra_locationscreeningrequired";
    }
}
