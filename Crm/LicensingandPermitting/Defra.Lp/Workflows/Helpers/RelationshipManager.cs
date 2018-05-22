// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <summary>Class is responsible for relinking and copying entities from a source parent to a target parent</summary>

namespace Defra.Lp.Workflows.Helpers
{
    using System;
    using System.Linq;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Class is in charge of linking/copying child entities from a source parent to a target parent
    /// </summary>
    public class RelationshipManager
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

        public RelationshipManager(IOrganizationService service, string sourceEntityName, Guid sourceEntityId, string targetEntityName, Guid targetEntityId)
        {
            this._Service = service;
            this.SourceEntityName = sourceEntityName;
            this.SourceEntityId = sourceEntityId;
            this.TargetEntityName = targetEntityName;
            this.TargetEntityId = targetEntityId;
        }

        /// <summary>
        /// Copies entties from the source to the target.
        /// E.g. LinkEntitiesToTarget(Location.EntityLogicalName, Location.Application, Location.Permit, true);
        /// </summary>
        /// <param name="entityToLinkLogicalName">Entity to be linked to target</param>
        /// <param name="entityToLinkLookupToSource">Field that has a lookup to the source parent entity</param>
        /// <param name="entityToLinkLookupToTarget">Field that has a lookup to the target parent entity</param>
        /// <param name="unlinkRecordsWhereRequired">flag that indicates whether</param>
        public void LinkEntitiesToTarget(string entityToLinkLogicalName, string entityToLinkLookupToSource, string entityToLinkLookupToTarget, bool unlinkRecordsWhereRequired)
        {
            //Get the entities linked to the source that need to be linked to the target
            QueryExpression queryEntitiesToLink = new QueryExpression(entityToLinkLogicalName)
            {
                ColumnSet = new ColumnSet(entityToLinkLookupToSource, entityToLinkLookupToTarget),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression(entityToLinkLookupToSource, ConditionOperator.Equal, this.SourceEntityId)
                    }
                }
            };
            EntityCollection entitiesToLink = this._Service.RetrieveMultiple(queryEntitiesToLink);

            //  Iterate through each record, ensuring it is linked to the target
            foreach (Entity entityToLink in entitiesToLink.Entities)
            {
                // Does the record need linking to the target?
                if (!entityToLink.Attributes.Contains(entityToLinkLookupToTarget) ||
                    entityToLink.Attributes[entityToLinkLookupToTarget] == null ||
                    ((EntityReference) entityToLink.Attributes[entityToLinkLookupToTarget]).Id != this.TargetEntityId)
                {
                    // Link it to the target
                    entityToLink[entityToLinkLookupToTarget] = this.TargetEntityReference;
                    this._Service.Update(entityToLink);
                }
            }

            // Do we need to blank out records linked to the target entity that shouldn't be linked?
            if (unlinkRecordsWhereRequired)
            {
                //Get all records linked to the target entity
                QueryExpression queryExistingEntities = new QueryExpression(entityToLinkLogicalName)
                {
                    ColumnSet = new ColumnSet(entityToLinkLookupToSource, entityToLinkLookupToTarget),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression(entityToLinkLookupToTarget, ConditionOperator.Equal, this.TargetEntityId)
                    }
                    }
                };
                EntityCollection existingEntities = this._Service.RetrieveMultiple(queryExistingEntities);

                foreach (Entity existing in existingEntities.Entities)
                {
                    if (entitiesToLink.Entities.Count(e => e.Id == existing.Id) == 0)
                    {
                        // Record shouldn't be linked to the target entity, clear the link.
                        existing[entityToLinkLookupToTarget] = null;
                        _Service.Update(existing);
                    }
                }
            }
        }

        /// <summary>
        /// Copies records from source to target, using matching fields.
        /// e.g.:
        /// copier.CopyAs(
        /// ApplicationLine.EntityLogicalName,
        ///                 ApplicationLine.ApplicationId, 
        /// new []
        ///                 {
        /// PermitLine.Name, PermitLine.PermitType, PermitLine.StandardRule
        /// }, 
        /// PermitLine.EntityLogicalName, 
        ///  PermitLine.Permit, 
        ///                 true);
        /// </summary>
        /// <param name="copiedEntityName"></param>
        /// <param name="copiedEntityLookupToSource"></param>
        /// <param name="copiedAttributes"></param>
        /// <param name="copiedAsEntityName"></param>
        /// <param name="copiedAsEntityLookupToTarget"></param>
        /// <param name="deactivateOthers"></param>
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
