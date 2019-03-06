
namespace Lp.DataAccess
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Lp.Model.Crm;
    using Lp.Model.EarlyBound;

    public static class DataAccessEmail
    {
        public static EntityCollection GetEmailAndAttachmentsForId(this IOrganizationService service, Guid emailId)
        {
            // Instantiate QueryExpression QEemail
            var queryEmail = new QueryExpression(Email.EntityLogicalName);

            // Add columns to QEemail.ColumnSet
            queryEmail.ColumnSet.AddColumns(Email.Fields.ActivityId, Email.Fields.defra_uploadedtosharepoint, Email.Fields.Subject);

            // Define filter QEemail.Criteria. Only want specific email
            queryEmail.Criteria.AddCondition(Email.Fields.ActivityId, ConditionOperator.Equal, emailId);

            // Add link-entity QEemail_activitymimeattachment
            var queryEmailAttachment = queryEmail.AddLink(ActivityMimeAttachment.EntityLogicalName, Email.Fields.ActivityId, ActivityMimeAttachment.ObjectId, JoinOperator.LeftOuter);
            queryEmailAttachment.EntityAlias = "attachment";

            // Add columns to Attachments
            queryEmailAttachment.Columns.AddColumns(ActivityMimeAttachment.Id, ActivityMimeAttachment.Filename, ActivityMimeAttachment.Filesize);

            // Define filter for Attachments. Only want those greater than zero
            queryEmailAttachment.LinkCriteria.AddCondition(ActivityMimeAttachment.Filesize, ConditionOperator.GreaterThan, 0);

            return service.RetrieveMultiple(queryEmail);
        }
    }
}