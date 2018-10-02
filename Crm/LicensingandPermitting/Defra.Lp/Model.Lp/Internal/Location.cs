// Location entity model at the L&P Organisation Level

using System;

namespace Model.Lp.Internal
{
    /// <summary>
    /// CRM Application entity model
    /// </summary>
    public partial class LocationModel
    {

        /// <summary>
        /// State field
        /// </summary>
        public int State;

        /// <summary>
        /// Primary name field
        /// </summary>
        public string Name;

        /// <summary>
        /// Lookup to the Application
        /// </summary>
        public Guid ApplicationId;

        /// <summary>
        /// Lookup to the Permit Entity
        /// </summary>
        public Guid PermitId;

        /// <summary>
        /// Yes/No
        /// </summary>
        public bool HighPublicInterest;
    }
}
