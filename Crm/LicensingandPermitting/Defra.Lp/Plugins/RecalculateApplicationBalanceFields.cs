using System;
using Lp.DataAccess;
using Lp.Model.Crm;
using Lp.Model.EarlyBound;
using Microsoft.Xrm.Sdk;

namespace Defra.Lp.Plugins
{

    /// <summary>
    /// Plugin responsible for recalculating the application balance fields when
    /// an application line or payment changes. Designed to handle Create/Update/Delete/Status change messages
    /// for both entities
    /// </summary>
    public class RecalculateApplicationBalanceFields : PluginBase
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// </summary>
        private const string PreImageAlias = "preImage";

        public RecalculateApplicationBalanceFields(string unsecure, string secure)
            : base(typeof(RecalculateApplicationBalanceFields))
        {
        }

        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }

            var serviceFactory = (IOrganizationServiceFactory)localContext.ServiceProvider.GetService(typeof(IOrganizationServiceFactory));
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracingService = localContext.TracingService;
            IOrganizationService adminService = serviceFactory.CreateOrganizationService(null);

            tracingService.Trace($"RecalculateApplicationBalanceFields Plugin - Start - Message {context.MessageName}");

            // 1. Get the target entity (Application Line or Payment)
            Entity targetRecord = GetTargetRecord(context, tracingService, adminService);

            // Get the application id from the target entity
            Guid? applicationId = GetApplicationId(targetRecord);

            if (applicationId.HasValue)
            {
                // Recalculate Application Fields for linked application
                DataAccessApplication.RecalculateApplicationBalances(adminService, tracingService, applicationId.Value);
            }
            else
            {
                tracingService.Trace("RecalculateApplicationBalanceFields Plugin - No Application Id, nothing to do.");
            }

            tracingService.Trace("RecalculateApplicationBalanceFields Plugin - End");
        }

        /// <summary>
        /// Finds the related application id from the target record
        /// </summary>
        /// <param name="targetRecord">Target record that triggered the plugin message</param>
        /// <returns>Related Application Id if found</returns>
        private static Guid? GetApplicationId(Entity targetRecord)
        {
            if (targetRecord == null)
            {
                return null;
            }

            if (targetRecord.Contains(ApplicationLine.ApplicationId))
            {
                EntityReference applicationEntityReference = targetRecord[ApplicationLine.ApplicationId] as EntityReference;

                if (applicationEntityReference != null)
                {
                    return applicationEntityReference.Id;
                }
            }

            if (targetRecord.Contains(Payment.ApplicationId))
            {
                EntityReference applicationEntityReference = targetRecord[Payment.ApplicationId] as EntityReference;

                if (applicationEntityReference != null)
                {
                    return applicationEntityReference.Id;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the entity that triggered the plugin message, could be a payment or application line
        /// </summary>
        /// <param name="context">Plugin context</param>
        /// <param name="tracingService">CRM Tracing Service</param>
        /// <param name="adminService">CRM Org admin service</param>
        /// <returns>Target entity that triggered the plugin</returns>
        private static Entity GetTargetRecord(IPluginExecutionContext context, ITracingService tracingService, IOrganizationService adminService)
        {
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Use the Target input parameter if it exists
                tracingService.Trace("RecalculateApplicationBalanceFields Plugin - Using Target Record");
                Entity targetRecord = (Entity) context.InputParameters["Target"];

                if (targetRecord.Contains(ApplicationLine.ApplicationId))
                {
                    // Found our target record with an application id attribute
                    return targetRecord;
                }

                // If applicationId attribute not in target record, try and retrieve it. 
                if (targetRecord.LogicalName == defra_applicationline.EntityLogicalName)
                {
                    return DataAccessApplicationLine.GetApplicationLine(adminService, targetRecord.Id);
                }

                if (targetRecord.LogicalName == defra_payment.EntityLogicalName)
                {
                    return  DataAccessPayments.GetPayment(adminService, targetRecord.Id);
                }
            }
            else if (context.PreEntityImages != null && context.PreEntityImages.Contains(PreImageAlias))
            {
                // If target not found, try to use the pre-image
                Entity targetRecord = context.PreEntityImages[PreImageAlias];
                tracingService.Trace($"RecalculateApplicationBalanceFields Plugin - PreImage logical name {targetRecord?.LogicalName}");
                return targetRecord;
            }
            else if (context.InputParameters.Contains("EntityMoniker") && context.InputParameters["EntityMoniker"] is EntityReference)
            {
                // If target entity and pre image entity not available, try the entity moniker (for status updates)
                tracingService.Trace("RecalculateApplicationBalanceFields Plugin - Using EntityMoniker");
                if (context.PrimaryEntityName == defra_applicationline.EntityLogicalName)
                {
                    return DataAccessApplicationLine.GetApplicationLine(adminService, context.PrimaryEntityId);
                }

                if (context.PrimaryEntityName == defra_payment.EntityLogicalName)
                {
                    return DataAccessPayments.GetPayment(adminService, context.PrimaryEntityId);
                }
            }

            // No suitable target found
            return null;
        }
    }
}
