namespace PermitLifecycle1.PermitLifecycleWorkflows.Helpers
{
    using System;
    using System.Linq;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public class CopyRelationship
    {
        private IOrganizationService _Service { get; set; }

        private string SourceEntityName { get; set; }

        private Guid SourceEntityId { get; set; }

        private EntityReference SourceEntityReference
        {
            get
            {
                return new EntityReference(this.SourceEntityName, this.SourceEntityId);
            }
        }

        private string TargetEntityName { get; set; }

        private Guid TargetEntityId { get; set; }

        private EntityReference TargetEntityReference
        {
            get
            {
                return new EntityReference(this.TargetEntityName, this.TargetEntityId);
            }
        }

        public CopyRelationship(IOrganizationService service, string sourceEntityName, Guid sourceEntityId, string targetEntityName, Guid targetEntityId)
        {
            this._Service = service;
            this.SourceEntityName = sourceEntityName;
            this.SourceEntityId = sourceEntityId;
            this.TargetEntityName = targetEntityName;
            this.TargetEntityId = targetEntityId;
        }

        public void Copy(string copiedEntityName, string copiedEntityLookupToSource, string copiedEntityLookupToTarget, bool nullOthersLookupToTarget)
        {
            //Get the existing entities
            QueryExpression queryExistingEntities = new QueryExpression(copiedEntityName)
            {
                ColumnSet = new ColumnSet(copiedEntityLookupToSource, copiedEntityLookupToTarget),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression(copiedEntityLookupToTarget, ConditionOperator.Equal, this.TargetEntityId)
                    }
                }
            };

            EntityCollection existingEntities = this._Service.RetrieveMultiple(queryExistingEntities);

            //Get the copied entities
            QueryExpression queryCopiedEntities = new QueryExpression(copiedEntityName)
            {
                ColumnSet = new ColumnSet(copiedEntityLookupToSource, copiedEntityLookupToTarget),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression(copiedEntityLookupToSource, ConditionOperator.Equal, this.SourceEntityId)
                    }
                }
            };

            EntityCollection copiedEntities = this._Service.RetrieveMultiple(queryCopiedEntities);

            //Copy
            foreach (Entity copied in copiedEntities.Entities)
                if (copied.Attributes.Contains(copiedEntityLookupToTarget) && copied.Attributes[copiedEntityLookupToTarget] != null && ((EntityReference)copied.Attributes[copiedEntityLookupToTarget]).Id == this.TargetEntityId)
                    continue;
                else
                {
                    copied[copiedEntityLookupToTarget] = this.TargetEntityReference;

                    this._Service.Update(copied);
                }

            if (nullOthersLookupToTarget)
                foreach (Entity existing in existingEntities.Entities)
                    if (copiedEntities.Entities.Count(e => e.Id == existing.Id) == 0)
                    {
                        existing[copiedEntityLookupToTarget] = null;

                        _Service.Update(existing);
                    }
        }

        public void CopyAs(string copiedEntityName, string copiedEntityLookupToSource, string[] copiedAttributes, string copiedAsEntityName, string copiedAsEntityLookupToTarget, bool deactivateOthers)
        {
            //Deactivate others if needed
            if (deactivateOthers)
            {
                QueryExpression queryOthers = new QueryExpression(copiedAsEntityName)
                {
                    ColumnSet = new ColumnSet(string.Format("{0}id", copiedAsEntityName)),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(copiedAsEntityLookupToTarget, ConditionOperator.Equal, this.TargetEntityId),
                            new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                        }
                    }
                };

                EntityCollection queryOthersResults = this._Service.RetrieveMultiple(queryOthers);

                foreach (Entity otherEntity in queryOthersResults.Entities)
                {
                    otherEntity["statecode"] = new OptionSetValue(1);
                    otherEntity["statuscode"] = new OptionSetValue(2);

                    this._Service.Update(otherEntity);
                }
            }

            QueryExpression queryCopiedEntities = new QueryExpression(copiedEntityName)
            {
                ColumnSet = new ColumnSet(copiedAttributes),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression(copiedEntityLookupToSource, ConditionOperator.Equal, this.SourceEntityId)
                    }
                }
            };

            EntityCollection copiedEntities = this._Service.RetrieveMultiple(queryCopiedEntities);

            //Create the copied entities
            foreach (Entity copied in copiedEntities.Entities)
            {
                Entity copiedAs = new Entity(copiedAsEntityName);
                copiedAs[copiedAsEntityLookupToTarget] = this.TargetEntityReference;

                foreach (string attribute in copiedAttributes)
                    if (copied.Attributes.Contains(attribute))
                        copiedAs[attribute] = copied[attribute];

                _Service.Create(copiedAs);
            }
        }
    }
}
