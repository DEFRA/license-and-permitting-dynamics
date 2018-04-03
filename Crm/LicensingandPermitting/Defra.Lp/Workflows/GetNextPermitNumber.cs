// <copyright file="GetNextPermitNumber.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>3/18/2018 1:12:58 PM</date>
// <summary>Implements the GetNextPermitNumber Workflow Activity.</summary>
namespace Defra.Lp.Workflows
{
    using System;
    using System.Activities;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using Microsoft.Xrm.Sdk.Query;

    public sealed class GetNextPermitNumber : CodeActivity
    {
        /// <summary>
        /// Output Permit Number
        /// </summary>
        [Output("Next Permit Number")]
        [Default("ERROR")]
        public InOutArgument<String> NextPermitNumber { get; set; }


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

            tracingService.Trace("Entered GetNextPermitNumber.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("GetNextPermitNumber.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            //Retrieve the autonumbering record
            QueryExpression query = new QueryExpression("defra_autonumbering")
            {
                ColumnSet = new ColumnSet("defra_locked"),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                        //new ConditionExpression("defra_name", ConditionOperator.Equal, "Waste");
                    }
                }
            };
            EntityCollection results = service.RetrieveMultiple(query);

            //Throw an exception if the autonumbering record does not exist
            if (results.Entities.Count == 0)
                throw new InvalidPluginExecutionException("The autonumbering record cannot be found!");

            //Pre-lock the autonumbering table. Refer to the Microsoft Scalability White Paper for more details https://www.microsoft.com/en-us/download/details.aspx?id=45905
            Entity autoNum = new Entity(results.Entities[0].LogicalName) { Id = results.Entities[0].Id };
            autoNum["defra_locked"] = true;
            service.Update(autoNum);

            //Retrieve safely the autonumbering record
            var lockedAutonumber = service.Retrieve(autoNum.LogicalName, autoNum.Id, new ColumnSet(new string[] { "defra_prefix", "defra_suffix", "defra_currentnumber" }));
            var currentNumber = (int)lockedAutonumber["defra_currentnumber"];
            var prefix = lockedAutonumber.GetAttributeValue<string>("defra_prefix");
            var suffix = lockedAutonumber.GetAttributeValue<string>("defra_suffix");

            // Increment suffix
            if (currentNumber == 9999)
            {
                currentNumber = 1;
                suffix = GetNextSuffix(suffix);
            }
            else
            {
                ++currentNumber;
            }
            tracingService.Trace("CurrentNumber: {0}", currentNumber.ToString("0000"));
            tracingService.Trace("Suffix: {0}", suffix);

            var nextPermitNumber = string.Format("{0}{1}{2}", prefix, currentNumber.ToString("0000"), suffix);

            //Update the next permit number
            NextPermitNumber.Set(executionContext, nextPermitNumber);
            //NextPermitNumber.Set(executionContext, GetNextNumber(++currentNumber));

            //Update the sequence number
            var counterUpdater = new Entity(autoNum.LogicalName);
            counterUpdater.Id = autoNum.Id;
            counterUpdater["defra_currentnumber"] = currentNumber;
            counterUpdater["defra_suffix"] = suffix;
            counterUpdater["defra_locked"] = false;
            service.Update(counterUpdater);

            tracingService.Trace("Exiting GetNextPermitNumber.Execute(), Correlation Id: {0}", context.CorrelationId);
        }

        private string GetNextSuffix(string suffix)
        {
            // AA, AB ... AZ, BA, BB ... BZ, CA ... CZ ...
            var chars = suffix.ToCharArray();
            if (chars[1] == 'Z')
            {
                chars[0]++;
                chars[1] = 'A';
            }
            else
            {
                chars[1]++;
            }
            return new string(chars);
        }

        //
        //  This is the SciSys algorithm that it was decided we would no longer use.
        //
        //private string GetNextNumber(int WorkNumber)
        //{
        //    //OriginalNumber is unique number to be used to generate Permit Number

        //    // Set up static arrays
        //    int[] NumberArray = { 3, 0, 9, 5, 8, 4, 2, 6, 1, 7 };
        //    string[] AlphaArray =
        //    {
        //            "P", "B", "S", "L", "M", "U", "X", "G", "K", "T", "H", "F", "C", "Z", "N", "E", "V",
        //            "W", "A", "R", "D", "Y", "J", "Q"
        //        };

        //    // Set up working arrays
        //    int[] WorkArray = new int[7];
        //    string[] CodeArray = new string[7];

        //    //  Populate the WorkArray using calculations on the WorkNumber
        //    WorkArray[0] = WorkNumber / 13824000;
        //    WorkNumber = WorkNumber - WorkArray[0] * 13824000;
        //    WorkArray[1] = WorkNumber / 576000;
        //    WorkNumber = WorkNumber - WorkArray[1] * 576000;
        //    WorkArray[2] = WorkNumber / 57600;
        //    WorkNumber = WorkNumber - WorkArray[2] * 57600;
        //    WorkArray[3] = WorkNumber / 2400;
        //    WorkNumber = WorkNumber - WorkArray[3] * 2400;
        //    WorkArray[4] = WorkNumber / 240;
        //    WorkNumber = WorkNumber - WorkArray[4] * 240;
        //    WorkArray[5] = WorkNumber / 10;
        //    WorkNumber = WorkNumber - WorkArray[5] * 10;
        //    WorkArray[6] = WorkNumber;
        //    WorkNumber = WorkNumber - WorkArray[6];


        //    //  Populate the CodeArray with characters and numbers from the pre-defined lists
        //    //   using the WorkArray
        //    CodeArray[0] = AlphaArray[WorkArray[5]];
        //    CodeArray[1] = AlphaArray[WorkArray[1]];
        //    CodeArray[2] = NumberArray[WorkArray[0]].ToString();
        //    CodeArray[3] = NumberArray[WorkArray[6]].ToString();
        //    CodeArray[4] = NumberArray[WorkArray[2]].ToString();
        //    CodeArray[5] = NumberArray[WorkArray[4]].ToString();
        //    CodeArray[6] = AlphaArray[WorkArray[3]];

        //    //  Calculate and populate the check digit
        //    int C;
        //    C = Encoding.ASCII.GetBytes(CodeArray[0])[0]
        //        + Encoding.ASCII.GetBytes(CodeArray[1])[0] * 2
        //        + Encoding.ASCII.GetBytes(CodeArray[2])[0]
        //        + Encoding.ASCII.GetBytes(CodeArray[3])[0] * 2
        //        + Encoding.ASCII.GetBytes(CodeArray[4])[0]
        //        + Encoding.ASCII.GetBytes(CodeArray[5])[0] * 2
        //        + Encoding.ASCII.GetBytes(CodeArray[6])[0];

        //    int modulo = (C % 24);

        //    string CheckDigit = AlphaArray[modulo];

        //    //  Construct the final permit number from the CodeArray
        //    string PermitNumber = CodeArray[0]
        //                          + CodeArray[1]
        //                          + CodeArray[2]
        //                          + CodeArray[3]
        //                          + CodeArray[4]
        //                          + CodeArray[5]
        //                          + CodeArray[6]
        //                          + CheckDigit;

        //    return PermitNumber;
        //}
    }
}