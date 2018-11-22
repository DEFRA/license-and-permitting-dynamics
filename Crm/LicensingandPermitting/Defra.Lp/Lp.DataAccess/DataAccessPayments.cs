namespace Lp.DataAccess
{
    using System;
    using Model.EarlyBound;
    using Microsoft.Xrm.Sdk.Query;
    using Model.Crm;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Data access layer for Payments
    /// </summary>
    public static class DataAccessPayments
    {
        /// <summary>
        /// Returns the payemntvalues, for all active payments linked to the application
        /// </summary>
        /// <param name="service">CRM service</param>
        /// <param name="applicationId">Linked application</param>
        /// <param name="includedStatusCodes">White list status filter</param>
        /// <returns>EntityCollection containing active payments</returns>
        public static EntityCollection GetPaymentValues(IOrganizationService service, Guid applicationId, defra_payment_StatusCode[] includedStatusCodes)
        {
            // Query active application lines linked to the given application
            // using nsted filter expressions to handle status filters late on
            QueryExpression query = new QueryExpression(defra_payment.EntityLogicalName)
            {
                
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Filters = 
                    {
                        new FilterExpression {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression(Payment.ApplicationId, ConditionOperator.Equal, applicationId),
                                new ConditionExpression(Payment.State, ConditionOperator.Equal, (int)defra_paymentState.Active)
                            }
                        }
                    }
                },
                ColumnSet = new ColumnSet(Payment.PaymentValue)
            };

            // Optional Status filter
            if (includedStatusCodes != null && includedStatusCodes.Length > 0)
            {
                var statusFilter = new FilterExpression(LogicalOperator.Or);

                foreach (defra_payment_StatusCode statusCode in includedStatusCodes)
                {
                    statusFilter.AddCondition(new ConditionExpression(Payment.Status, ConditionOperator.Equal, (int)statusCode));
                }
                query.Criteria.Filters.Add(statusFilter);
            }

            // Query CRM
            return service.RetrieveMultiple(query);
        }

        /// <summary>
        /// Reads a payment entity from CRM, includes just the application id if no columns passed in
        /// </summary>
        /// <param name="service">CRM service</param>
        /// <param name="paymentId">Payment ID</param>
        /// <param name="columnsToRetrieve">CRM columsn to retrieve</param>
        /// <returns>Payment entity if found, otherwise null</returns>
        public static Entity GetPayment(IOrganizationService service, Guid paymentId, ColumnSet columnsToRetrieve = null)
        {
            return service.Retrieve(defra_payment.EntityLogicalName, paymentId, columnsToRetrieve ?? new ColumnSet(Payment.ApplicationId));
        }
    }
}