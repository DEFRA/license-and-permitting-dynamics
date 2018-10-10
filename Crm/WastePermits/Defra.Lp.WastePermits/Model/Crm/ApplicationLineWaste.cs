// Application entity model at the Wate Organisation Level
namespace WastePermits.Model.Crm
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public class ApplicationLineWaste : Lp.Model.Crm.ApplicationLine
    {
        /// <summary>
        /// Location Screening Required Boolean field
        /// </summary>
        public const string LocationScreeningRequired = "defra_locationscreeningrequired";
    }
}
