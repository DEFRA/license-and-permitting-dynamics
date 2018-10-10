// Application entity model at the Wate Organisation Level
namespace WastePermits.Model.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public class ApplicationWaste : Lp.Model.Crm.Application
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
