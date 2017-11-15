namespace Defra.Lp.WastePermits.Plugins.Common.Security
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Crm.Sdk.Messages;

    /// <summary>
    /// Recprds Ownership Management Class
    /// </summary>
    public class OwnershipManager : OwnershipManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnershipManager"/> class.
        /// </summary>
        /// <param name="Service">The service.</param>
        /// <param name="Context">The context.</param>
        /// <param name="preImage">The pre image.</param>
        public OwnershipManager(IOrganizationService Service, IPluginExecutionContext Context, Entity preImage)
            : base(Service, Context, preImage)
        {
        }

        /// <summary>
        /// Sets the owner.
        /// </summary>
        public void SetOwner()
        {
            //Manages the owner on record creation
            if (this._Context.MessageName == "Create")
                this.SetOwnerCreate();

            if (this._Context.MessageName == "Update")
                this.SetOwnerUpdate();
        }

        /// <summary>
        /// Sets the owner create.
        /// </summary>
        private void SetOwnerCreate()
        {
            Guid? teamId = this.GetOwnerTeam();

            if (teamId.HasValue && teamId.Value != Guid.Empty)
                this.AssignTheRecord(teamId.Value);
        }

        /// <summary>
        /// Gets the owner team.
        /// </summary>
        /// <returns></returns>
        private Guid? GetOwnerTeam()
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

        /// <summary>
        /// Sets the owner on update.
        /// </summary>
        private void SetOwnerUpdate()
        {
            // Enforcing Cascading security due to platform issue to be investigated before adding defra_examples to this method
            //switch (Target.LogicalName)
            //{
            //    case "defra_example":
            //        if (Target.Attributes.Contains("defra_exampleid") && Target.Attributes["defra_exampleid"] != null)
            //            this.Reparent((EntityReference)Target.Attributes["defra_exampleid"]);
            //        break;
            //    default:

            //        break;
            //}
        }

        private void Reparent(EntityReference parentER)
        {
            Entity parent = _Service.Retrieve(parentER.LogicalName, parentER.Id, new ColumnSet("ownerid"));

            if (parent.Attributes.Contains("ownerid"))
            {
                AssignRequest assignReq = new AssignRequest()
                {
                    Assignee = (EntityReference)parent.Attributes["ownerid"],
                    Target = new EntityReference(_Context.PrimaryEntityName, _Context.PrimaryEntityId)
                };

                _Service.Execute(assignReq);
            }
        }

        /// <summary>
        /// Assigns the record.
        /// </summary>
        /// <param name="teamid">The teamid.</param>
        private void AssignTheRecord(Guid teamid)
        {
            if (teamid != Guid.Empty && _Context.InputParameters.Contains("Target") && _Context.InputParameters["Target"] is Entity)
            {
                Entity target = (Entity)_Context.InputParameters["Target"];

                if (target.Attributes.Contains("ownerid"))
                    target.Attributes["ownerid"] = new EntityReference("team", teamid);
                else
                    target.Attributes.Add("ownerid", new EntityReference("team", teamid));
            }
        }
    }
}
