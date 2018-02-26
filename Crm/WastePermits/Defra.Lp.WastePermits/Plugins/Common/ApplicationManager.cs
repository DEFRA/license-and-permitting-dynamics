

namespace Defra.Lp.WastePermits.Plugins.Common
{
    using System;
    using Security;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Recprds Ownership Management Class
    /// </summary>
    public class ApplicationManager : OwnershipManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnershipManager"/> class.
        /// </summary>
        /// <param name="Service">The service.</param>
        /// <param name="Context">The context.</param>
        /// <param name="preImage">The pre image.</param>
        public ApplicationManager(IOrganizationService Service, IPluginExecutionContext Context, Entity preImage)
            : base(Service, Context, preImage)
        {
        }


        /// <summary>
        /// Gets the owner team.
        /// </summary>
        /// <returns></returns>
        private Guid? GetApplication(Guid applicationId)
        {
            var query = new QueryExpression("team")
            {
                ColumnSet = new ColumnSet("teamid"),

                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                      {
                        new ConditionExpression("businessunitid", ConditionOperator.Equal, this._Context.BusinessUnitId),
                        new ConditionExpression("name", ConditionOperator.Like, "% Owner")
                      }
                }
            };

            EntityCollection results = _Service.RetrieveMultiple(query);

            if (results.Entities.Count > 0)
                return results.Entities[0].Id;

            return null;
        }
    }
}
