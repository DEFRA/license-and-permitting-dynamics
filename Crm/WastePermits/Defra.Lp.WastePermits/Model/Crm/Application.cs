// Application entity model at the Wate Organisation Level
namespace Model.Waste.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public class Application : Lp.Crm.Application
    {
        /// <summary>
        /// Location Screening Required Boolean field
        /// </summary>
        public const string LocationScreeningRequired = "defra_locationscreeningrequired";

        /// <summary>
        /// App Type OptionSet
        /// </summary>
        public const string ApplicationType = "defra_applicationtype";
    }
}
