namespace Lp.DataAccess
{
    using System;
    using Model.Entities;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public static class DataAccessUsers
    {

        /// <summary>
        /// Returns the maximum writeff allowed for the given users
        /// searches all the teams linked to the user
        /// </summary>
        /// <param name="service">CRM org service</param>
        /// <param name="systemUserId">User Id</param>
        /// <returns>Max write off value allowed</returns>
        public static Money GetMaximumUserCanWriteOff(this IOrganizationService service, Guid systemUserId)
        {
            Money maxWriteOffValue = new Money();

            // Get all teams linked to the user
            QueryExpression query = new QueryExpression(Team.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Team.MaximumWriteOffAllowedField)
            };
            LinkEntity link = query.AddLink(TeamMembership.EntityLogicalName, TeamMembership.TeamField, TeamMembership.TeamField);
            link.LinkCriteria.AddCondition(new ConditionExpression(TeamMembership.SystemUserField, ConditionOperator.Equal, systemUserId));
            EntityCollection teams = service.RetrieveMultiple(query);

            // Get the max write off value from the teams
            if (teams == null || teams.TotalRecordCount == 0)
            {
                return maxWriteOffValue;
            }

            foreach (var team in teams.Entities)
            {
                if (team.Contains(Team.MaximumWriteOffAllowedField))
                {
                    Money teamWriteOffValue = team[Team.MaximumWriteOffAllowedField] as Money;

                    if (teamWriteOffValue != null && teamWriteOffValue.Value > maxWriteOffValue.Value)
                    {
                        maxWriteOffValue = teamWriteOffValue;
                    }
                }
            }
            
            // Return max writeOff found   
            return maxWriteOffValue;
        }
    }
}