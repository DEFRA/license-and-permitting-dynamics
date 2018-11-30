namespace Lp.DataAccess
{
    using Core.DataAccess.Base;
    using Model.EarlyBound;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public class DataAccessAutoNumber : DataAccessBase
    {

        public DataAccessAutoNumber(IOrganizationService organisationService, ITracingService tracingService) : base(organisationService, tracingService)
        {
        }

        public string GetNextAutoNumber(string autoNumberName)
        {
            //Retrieve the autonumbering record
            QueryExpression query = new QueryExpression(defra_autonumbering.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(defra_autonumbering.Fields.defra_locked),
                Criteria = new FilterExpression()
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(defra_autonumbering.Fields.StateCode, ConditionOperator.Equal, (int)defra_autonumberingState.Active),
                        new ConditionExpression(defra_autonumbering.Fields.defra_name, ConditionOperator.Equal, autoNumberName)
                    }
                }
            };

            EntityCollection results = OrganisationService.RetrieveMultiple(query);

            //Throw an exception if the autonumbering record does not exist
            if (results.Entities.Count == 0)
            {
                throw new InvalidPluginExecutionException("The autonumbering record cannot be found!");
            }

            //Pre-lock the autonumbering table. Refer to the Microsoft Scalability White Paper for more details https://www.microsoft.com/en-us/download/details.aspx?id=45905
            Entity autoNum = new Entity(results.Entities[0].LogicalName)
            {
                Id = results.Entities[0].Id,
                [defra_autonumbering.Fields.defra_locked] = true
            };
            OrganisationService.Update(autoNum);

            //Retrieve safely the autonumbering record
            var lockedAutonumber = OrganisationService.Retrieve(
                autoNum.LogicalName, 
                autoNum.Id, 
                new ColumnSet(
                    defra_autonumbering.Fields.defra_prefix, 
                    defra_autonumbering.Fields.defra_suffix, 
                    defra_autonumbering.Fields.defra_currentnumber,
                    defra_autonumbering.Fields.defra_numberlength));

            var currentNumber = (int)lockedAutonumber[defra_autonumbering.Fields.defra_currentnumber];
            var numberLength = lockedAutonumber.Contains(defra_autonumbering.Fields.defra_numberlength) ? 
                (int)lockedAutonumber[defra_autonumbering.Fields.defra_numberlength] : 4;
            var prefix = lockedAutonumber.GetAttributeValue<string>(defra_autonumbering.Fields.defra_prefix);
            var suffix = lockedAutonumber.Contains(defra_autonumbering.Fields.defra_suffix) ? 
                lockedAutonumber.GetAttributeValue<string>(defra_autonumbering.Fields.defra_suffix) : string.Empty;

            // Increment suffix#
            currentNumber++;
            if (currentNumber.ToString().Length > numberLength)
            {
                if (!string.IsNullOrWhiteSpace(suffix))
                {
                    currentNumber = 1;
                    suffix = GetNextSuffix(suffix);
                }
                else
                {
                    throw new InvalidPluginExecutionException($"Autonumber for {autoNumberName} has reached it's maximum length of {numberLength}");
                }
            }
            string paddedNumber = currentNumber.ToString("D" + numberLength);
            TracingService.Trace("CurrentNumber: {0}", paddedNumber);
            TracingService.Trace("Suffix: {0}", suffix);
            TracingService.Trace("Max Digits: {0}, ready from db: {1} ", numberLength, lockedAutonumber.Contains(defra_autonumbering.Fields.defra_numberlength));

            var nextPermitNumber = $"{prefix}{paddedNumber}{suffix}";

            //Update the sequence number
            var counterUpdater = new Entity(autoNum.LogicalName)
            {
                Id = autoNum.Id,
                [defra_autonumbering.Fields.defra_currentnumber] = currentNumber,
                [defra_autonumbering.Fields.defra_suffix] = suffix,
                [defra_autonumbering.Fields.defra_locked] = false
            };

            OrganisationService.Update(counterUpdater);
            return nextPermitNumber;
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
    }
}