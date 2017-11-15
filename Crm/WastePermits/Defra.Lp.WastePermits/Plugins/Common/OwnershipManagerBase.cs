namespace Defra.Lp.WastePermits.Plugins.Common.Security
{
    using System;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Record Ownership Base Class
    /// </summary>
    public abstract class OwnershipManagerBase
    {
        /// <summary>
        /// Gets or sets the CRM Service.
        /// </summary>
        /// <value>
        /// The _ service.
        /// </value>
        protected IOrganizationService _Service { get; set; }

        /// <summary>
        /// Gets or sets the Plug in Context.
        /// </summary>
        /// <value>
        /// The _ context.
        /// </value>
        protected IPluginExecutionContext _Context { get; set; }

        /// <summary>
        /// Gets or sets the pre image.
        /// </summary>
        /// <value>
        /// The pre image.
        /// </value>
        protected Entity preImage { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        protected Entity Target { get; set; }

        public OwnershipManagerBase(IOrganizationService Service, IPluginExecutionContext Context, Entity preImage)
        {
            this._Service = Service;
            this._Context = Context;
            this.preImage = preImage;

            //Target cast
            if (this._Context.InputParameters.Contains("Target") && this._Context.InputParameters["Target"] is Entity)
                this.Target = (Entity)this._Context.InputParameters["Target"];
        }
    }
}
