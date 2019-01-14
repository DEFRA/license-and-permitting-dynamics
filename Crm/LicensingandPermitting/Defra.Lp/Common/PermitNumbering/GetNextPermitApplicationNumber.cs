// <copyright file="GetNextPermitApplicationNumber.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>3/18/2018 3:24:12 PM</date>
// <summary>Implements the GetNextPermitApplicationNumber Workflow Activity.</summary>
namespace Common.PermitNumbering
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public class PermitApplicationNumbering
    {
        /// <summary>
        /// Permit Number
        /// </summary>
        private string PermitNumber { get; set; }

        /// <summary>
        /// Application Type
        /// </summary>
        private string ApplicationType { get; set; }

        private IPluginExecutionContext Context { get; set; }

        private ITracingService TracingService { get; set; }

        private IOrganizationService Service { get; set; }

        public PermitApplicationNumbering(IPluginExecutionContext context, ITracingService tracingService, IOrganizationService service, string permitNumber, string applicationType)
        {
            this.Context = context;
            this.TracingService = tracingService;
            this.Service = service;

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered PermitApplicationNumbering");

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve the context.");
            }

            tracingService.Trace("PermitApplicationNumbering, Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            this.PermitNumber = permitNumber;
            this.ApplicationType = applicationType;

            tracingService.Trace("Exiting GetNextPermitApplicationNumber.Execute(), Correlation Id: {0}", context.CorrelationId);
        }

        public string GetNextPermitApplicationNumber()
        {
            TracingService.Trace("GetNextPermitApplicationNumber() Start...");

            if (PermitNumber == string.Empty || ApplicationType == string.Empty)
                throw new InvalidPluginExecutionException("Not all input parameters have valid value!");

            if (ApplicationType == "A") //Application for a new Permit
            {
                return GenerateApplicationNumber(ApplicationType, 1);
            }


            //Retrieve the permit record
            QueryExpression query = new QueryExpression("defra_permit")
            {
                ColumnSet = new ColumnSet("defra_locked"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression("defra_permitnumber", ConditionOperator.Equal, PermitNumber)
                    }
                }
            };
            EntityCollection results = Service.RetrieveMultiple(query);

            // If the permit record does not exist, it is for a new application or a partial transfer
            if (results.Entities.Count == 0)
            {
                return GenerateApplicationNumber(ApplicationType, 1);
            }

            //Pre-lock the autonumbering table. Refer to the Microsoft Scalability White Paper for more details https://www.microsoft.com/en-us/download/details.aspx?id=45905
            Entity autoNum = new Entity(results.Entities[0].LogicalName) { Id = results.Entities[0].Id };
            autoNum["defra_locked"] = true;
            Service.Update(autoNum);

            //Retrieve safely the autonumbering record
            var lockedAutonumber = Service.Retrieve(autoNum.LogicalName, autoNum.Id, new ColumnSet("defra_currentnumber"));
            int currentNumber = (lockedAutonumber.Attributes.Contains("defra_currentnumber") && lockedAutonumber["defra_currentnumber"] != null) ? (int)lockedAutonumber["defra_currentnumber"] : 1;
            currentNumber++;

            //Update the application number only string
            string ApplicationOnlyNumber = string.Format("{0:D3}", currentNumber);

            switch (ApplicationType)
            {
                case "V":
                case "T":
                case "S":
                    ApplicationOnlyNumber = GenerateApplicationNumber(ApplicationType, currentNumber);
                    break;
                default:
                    ApplicationOnlyNumber = GenerateApplicationNumber("_", currentNumber);
                    break;
            }

            //Update the sequence number
            var counterUpdater = new Entity(autoNum.LogicalName);
            counterUpdater.Id = autoNum.Id;
            counterUpdater["defra_currentnumber"] = currentNumber;
            counterUpdater["defra_locked"] = false;
            Service.Update(counterUpdater);

            //Set the Application Number Output value
            return ApplicationOnlyNumber;
        }

        /// <summary>
        /// Gets a new application number
        /// </summary>
        /// <param name="appNumber">Number to use as the postfix</param>
        /// <param name="prefix">The prefix to use</param>
        /// <returns>Generated application number</returns>
        private string GenerateApplicationNumber(string prefix, int appNumber)
        {
            string applicationOnlyNumber = $"{prefix}{appNumber:D3}";
            return $"{PermitNumber}/{applicationOnlyNumber}";
        }
    }
}