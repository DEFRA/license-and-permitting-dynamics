using System;
using System.Collections.Generic;
using System.Linq;
using Core.Model;
using Core.Model.Entities;
using Microsoft.Xrm.Sdk.Messages;

namespace Core.Configuration
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Data access for all Secure Configuration related data access methods
    /// </summary>
    public static class DataAccessPayments
    {

        /// <summary>
        /// Returns the Secure Configuration record value matching the key
        /// </summary>
        /// <param name="service">CRM Org Service</param>
        /// <param name="key">Config record Key</param>
        /// <returns>Config value as string</returns>
        public static string GetPaymentIdFromPaymentReferenceNumber(this IOrganizationService service, string paymentReferenceNum, ITracingService tracingService)
        {
            var fetchXml = $@" <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                         <entity name='defra_payment'>
                                          <attribute name='defra_paymentid' /> 
                                         <filter type='and'>
                                          <condition attribute='defra_reference_number' operator='eq' value='{paymentReferenceNum}' /> 
                                          </filter>
                                         <link-entity name='defra_paymenttransaction' from='defra_paymenttransactionid' alias='t' to='defra_payment_transaction' visible='false' link-type='outer'>
                                          <attribute name='defra_responsepaymentid' /> 
                                          </link-entity>
                                          </entity>
                                          </fetch>";

            RetrieveMultipleRequest fetchRequest = new RetrieveMultipleRequest
            {
                Query = new FetchExpression(fetchXml)
            };

            var results = ((RetrieveMultipleResponse)service.Execute(fetchRequest)).EntityCollection;

            if (results == null || results.Entities.Count == 0)
            {
                tracingService.Trace("GetPaymentIdFromPaymentReferenceNumber no results found");
                return null;
            }

            foreach (var attribute in results.Entities[0].Attributes)
            {
                tracingService.Trace("GetPaymentIdFromPaymentReferenceNumber attribute {0} = {1}", attribute.Key, attribute.Value);
            }

            if (results.Entities[0].Contains("t.defra_responsepaymentid"))
            {
                var aliasValue = (AliasedValue)results.Entities[0]["t.defra_responsepaymentid"];
                return aliasValue.Value as string;

            }
            return null;
        }
    }
}