// <copyright file="GetNextPermitApplicationNumber.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>3/18/2018 3:24:12 PM</date>
// <summary>Implements the GetNextPermitApplicationNumber Workflow Activity.</summary>
namespace Defra.Lp.Workflows
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using Microsoft.Xrm.Sdk.Query;

    public sealed class GetNextPermitApplicationNumber : CodeActivity
    {
        /// <summary>
        /// Input parameter - Permit Number
        /// </summary>
        [Input("Permit Number"), RequiredArgument]
        [Default("")]
        public InArgument<string> PermitNumber { get; set; }

        /// <summary>
        /// Input parameter - Application Type
        /// </summary>
        [Input("Application Type"), RequiredArgument]
        [Default("")]
        public InArgument<string> ApplicationType { get; set; }

        /// <summary>
        /// Output parameter - Permit Application Number
        /// </summary>
        [Output("Application Number")]
        [Default("ERROR")]
        public InOutArgument<string> ApplicationNumber { get; set; }

        /// <summary>
        /// Executes the workflow activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            // Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered GetNextPermitApplicationNumber.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("GetNextPermitApplicationNumber.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            string permitNumber = PermitNumber.Get(executionContext);
            string appType = ApplicationType.Get(executionContext);

            if (permitNumber == string.Empty || appType == string.Empty)
                throw new InvalidPluginExecutionException("Not all input parameters have valid value!");


            if (appType == "A") //Application for a new Permit
            {
                string ApplicationOnlyNumber = string.Format("A{0:D3}", 1);
                ApplicationNumber.Set(executionContext, string.Format("{0}/{1}", permitNumber, ApplicationOnlyNumber));
            }
            else
            {
                //Retrieve the permit record
                QueryExpression query = new QueryExpression("defra_permit")
                {
                    ColumnSet = new ColumnSet("defra_locked"),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression("defra_permitnumber", ConditionOperator.Equal, permitNumber)
                    }
                    }
                };
                EntityCollection results = service.RetrieveMultiple(query);

                //Throw an exception if the permit record does not exist
                if (results.Entities.Count == 0)
                    throw new InvalidPluginExecutionException("The permit record cannot be found!");

                //Pre-lock the autonumbering table. Refer to the Microsoft Scalability White Paper for more details https://www.microsoft.com/en-us/download/details.aspx?id=45905
                Entity autoNum = new Entity(results.Entities[0].LogicalName) { Id = results.Entities[0].Id };
                autoNum["defra_locked"] = true;
                service.Update(autoNum);

                //Retrieve safely the autonumbering record
                var lockedAutonumber = service.Retrieve(autoNum.LogicalName, autoNum.Id, new ColumnSet("defra_currentnumber"));
                int currentNumber = (lockedAutonumber.Attributes.Contains("defra_currentnumber") && lockedAutonumber["defra_currentnumber"] != null) ? (int)lockedAutonumber["defra_currentnumber"] : 1;
                currentNumber++;

                //Update the application number only string
                string ApplicationOnlyNumber = string.Format("{0:D3}", currentNumber);

                switch (appType)
                {
                    //case "A":
                    //    ApplicationOnlyNumber = string.Format("A{0:D3}", currentNumber);
                    //    break;
                    case "V":
                        ApplicationOnlyNumber = string.Format("V{0:D3}", currentNumber);
                        break;
                    case "T":
                        ApplicationOnlyNumber = string.Format("T{0:D3}", currentNumber);
                        break;
                    case "S":
                        ApplicationOnlyNumber = string.Format("S{0:D3}", currentNumber);
                        break;
                    default:
                        ApplicationOnlyNumber = string.Format("_{0:D3}", currentNumber);
                        break;
                }

                //Update the sequence number
                var counterUpdater = new Entity(autoNum.LogicalName);
                counterUpdater.Id = autoNum.Id;
                counterUpdater["defra_currentnumber"] = currentNumber;
                counterUpdater["defra_locked"] = false;
                service.Update(counterUpdater);

                //Set the Application Number Output value
                ApplicationNumber.Set(executionContext, string.Format("{0}/{1}", permitNumber, ApplicationOnlyNumber));
            }



            try
            {
                // TODO: Implement your custom Workflow business logic.
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting GetNextPermitApplicationNumber.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}