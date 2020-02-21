using Core.Helpers.Extensions;
using Lp.Model.Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WastePermits.Model.EarlyBound;

namespace Defra.Lp.WastePermits.Workflows
{
   
    public class CreateDiscountLines : WorkFlowActivityBase
    {

        public class MyAppLine

        {
            public Guid ItemId { get; set; }
            public Money Value { get; set; }

            public string ItemName { get; set; }
            public Guid ApplicationLineId { get; set; }

            public Guid ApplicationId { get; set; }
            public int? LineType { get; set; }

            public Guid ParentLine { get; set; }



        }
        private ITracingService _TracingService { get; set; }
        private IPluginExecutionContext _Context { get; set; }
        private IOrganizationService _Service { get; set; }

        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext executionContext, LocalWorkflowContext crmWorkflowContext)
        {
            // 1. Validate
            if (crmWorkflowContext == null)
            {
                throw new ArgumentNullException(nameof(crmWorkflowContext));
            }

            // 2. Count lines
            _TracingService = executionContext.GetExtension<ITracingService>();
             _Service = crmWorkflowContext.OrganizationService;
            var context = crmWorkflowContext.WorkflowExecutionContext;

            CreateDiscountLinesGivenApplicationId(context.PrimaryEntityId);

        }


        private void CreateDiscountLinesGivenApplicationId(Guid applicationId)
        {

            _TracingService.Trace("Inside CreateDiscountLines method");
            var appLines = GetAllApplicationLines(applicationId).Entities.ToList();

            _TracingService.Trace("Got GetAllApplicationLines count: " + appLines.Count().ToString());


            //var disLines = GetAllDiscountLines(appLines);

            //BulkDiscountDelete(_Service, disLines);



            UpdateApplicationLinePriceForAssociatedActivities(appLines);


            var retVal = appLines
                    .Select(e => new MyAppLine
                    {
                        ParentLine = e.GetAttributeIdOrDefault(defra_applicationline.Fields.defra_parentapplicationline),
                        ApplicationId = e.GetAttributeId(defra_applicationline.Fields.defra_applicationId).Value,
                        ApplicationLineId = e.Id,
                        ItemId = e.GetAttributeIdOrDefault(defra_applicationline.Fields.defra_itemid),
                        Value = e.GetAttributeValue<Money>(defra_applicationline.Fields.defra_value),
                        LineType = e.GetAttributeValue<OptionSetValue>(defra_applicationline.Fields.defra_linetype).Value,
                        ItemName = e.GetAttributeValue<EntityReference>(defra_applicationline.Fields.defra_itemid) != null ? (e.GetAttributeValue<EntityReference>(defra_applicationline.Fields.defra_itemid)).Name : null

                    }).ToList().OrderByDescending(x => x.Value.Value).GroupBy(x => x.ItemId, (key, g) => new { id = key, lines = g.ToList() }).ToList();

            _TracingService.Trace("appLines grouped by");

            var g1 = 0;
            foreach (var parentLine in retVal)
            {
                var i = 0;
                foreach (var line in parentLine.lines)
                {
                    if (line.Value.Value == 0 || line.LineType == (int)ApplicationLineTypeValues.Discount)
                        continue;

                    if (parentLine.lines.Count() > 1)
                    {
                        if (g1 == 0 && i > 0)
                        {
                            if (!appLines.Select(x => x.GetAttributeIdOrDefault(defra_applicationline.Fields.defra_parentapplicationline)).Contains(line.ApplicationLineId))
                                // moset expensive one 90% 
                                CreateDiscountEntity("- 90% discount for a duplicate activity", line.Value.Value, 90, line.ApplicationLineId, ApplicationLineDiscountTypeValues.D90, line.ApplicationId,line.ItemId);

                        }

                        if (g1 > 0 && i == 0)
                        {
                            if (!appLines.Select(x => x.GetAttributeIdOrDefault(defra_applicationline.Fields.defra_parentapplicationline)).Contains(line.ApplicationLineId))

                                // multiple 50% 
                                CreateDiscountEntity("-50% discount for an associated activity", line.Value.Value, 50, line.ApplicationLineId, ApplicationLineDiscountTypeValues.D50, line.ApplicationId,line.ItemId);
                        }
                        if (g1 > 0 && i > 0)
                        {
                            if (!appLines.Select(x => x.GetAttributeIdOrDefault(defra_applicationline.Fields.defra_parentapplicationline)).Contains(line.ApplicationLineId))

                                // duplicate additional activity 90%
                                CreateDiscountEntity("-90% discount for a duplicate activity", line.Value.Value, 90, line.ApplicationLineId, ApplicationLineDiscountTypeValues.D90, line.ApplicationId, line.ItemId);
                        }

                    }
                    else if (parentLine.lines.Count() == 1 && (g1 > 0))
                    {
                        if (!appLines.Select(x => x.GetAttributeIdOrDefault(defra_applicationline.Fields.defra_parentapplicationline)).Contains(line.ApplicationLineId))

                            CreateDiscountEntity("-50% discount for an associated activity", line.Value.Value, 50, line.ApplicationLineId, ApplicationLineDiscountTypeValues.D50, line.ApplicationId, line.ItemId);

                    }

                    i++;
                }

                g1++;

            }
            _TracingService.Trace("finihsed foreach loop to create discounts");

            SetDisplayOrderForAppLines(applicationId);

        }

        private void UpdateApplicationLinePriceForAssociatedActivities(List<Entity> appLines)
        {
            _TracingService.Trace("Inside UpdateApplicationLinePriceForAssociatedActivities");
            var appLinesFound =
                    appLines.Select(e => new MyAppLine
                    {
                        LineType = e.GetAttributeValue<OptionSetValue>(defra_applicationline.Fields.defra_linetype).Value,
                        ApplicationLineId = e.Id,
                        ItemId = e.GetAttributeIdOrDefault(defra_applicationline.Fields.defra_itemid),
                    }).ToList().Where(x => x.LineType != (int)ApplicationLineTypeValues.Discount);

            var inCon = "";
            foreach (var item in appLinesFound)
            {
                inCon += @"<value  uitype='defra_item'>{" + item.ItemId.ToString() + @"}</value>";
            }

            var fetch = @"<fetch>
  <entity name='defra_itemsassociated'>
   
    <attribute name='defra_associateditem' />
   
  
    <link-entity name='defra_item' from='defra_itemid' to='defra_parentitem' link-type='inner' alias='ad'>
      <filter type='and'>
        <condition attribute='defra_itemid' operator='in'>"
          + inCon+@"
        </condition>
      </filter>
    </link-entity>
  </entity>
</fetch>";
        
            var parentItems = _Service.RetrieveMultiple(new FetchExpression(fetch)).Entities;

            if (parentItems.Count() > 0)
            {
                foreach (var associate in parentItems)
                {
                    var appLine = appLinesFound.FirstOrDefault(x => x.ItemId == (associate.GetAttributeIdOrDefault("defra_associateditem")));
                    if (appLine != null)
                    {

                        var ent = new Entity(defra_applicationline.EntityLogicalName);
                        ent.Id = appLine.ApplicationLineId;
                        ent.Attributes.Add(defra_applicationline.Fields.defra_value, new Money(0));
                        _Service.Update(ent);
                        appLines.Where(x => x.Id == appLine.ApplicationLineId).FirstOrDefault().Attributes[defra_applicationline.Fields.defra_value] = new Money(0);


                    }

                }
            }
            _TracingService.Trace("Finished UpdateApplicationLinePriceForAssociatedActivities");


        }



        private IEnumerable<Entity> GetAllDiscountLines(List<Entity> appLines)
        {
            var discounts = appLines.Where(x => x.GetAttributeValue<OptionSetValue>(defra_applicationline.Fields.defra_linetype).Value == (int)ApplicationLineTypeValues.Discount);
            //_Service.RetrieveMultiple(new FetchExpression(fetch));
            return discounts;
        }

        private void CreateDiscountEntity(string name, decimal val, int dis, Guid appLineId, ApplicationLineDiscountTypeValues disType, Guid appId, Guid itemId)
        {

            var item= _Service.Retrieve(defra_item.EntityLogicalName, itemId, new ColumnSet(defra_item.Fields.defra_itemtypeid));
            
            // Not assessment
            if (item != null && item.GetAttributeId(defra_item.Fields.defra_itemtypeid) != Guid.Parse("288C1ACD-34D9-E811-A96E-000D3A23443B"))
            {

                _TracingService.Trace("Inside CreateDiscountEntity item name is:" + name);

                var disEnt = new Entity(defra_applicationline.EntityLogicalName);
                disEnt.Attributes.Add(defra_applicationline.Fields.defra_name, name);
                var m = new Money(-dis * val / 100);
                disEnt.Attributes.Add(defra_applicationline.Fields.defra_value, m);
                disEnt.Attributes.Add(defra_applicationline.Fields.defra_linetype, new OptionSetValue((int)ApplicationLineTypeValues.Discount));
                disEnt.Attributes.Add(defra_applicationline.Fields.defra_parentapplicationline, new EntityReference(defra_applicationline.EntityLogicalName, appLineId));
                disEnt.Attributes.Add(defra_applicationline.Fields.defra_discounttype, new OptionSetValue((int)disType));
                disEnt.Attributes.Add(defra_applicationline.Fields.defra_applicationId, new EntityReference(defra_application.EntityLogicalName, appId));
                disEnt.Attributes.Add(defra_applicationline.Fields.defra_itemid, new EntityReference(defra_item.EntityLogicalName, itemId));


                _Service.Create(disEnt);
            }
            _TracingService.Trace("Finished CreateDiscountEntity");


        }

        private void SetDisplayOrderForAppLines(Guid appId)
        {
            var appLines = GetApplicationLinesOnlyRegulatedFacilities(appId).Entities.ToList(); //.Select(x=>x.Attributes["defra_itemid"].Equals(""));
            var retVal = appLines
                    .Select(e => new MyAppLine
                    {
                        LineType = e.GetAttributeValue<OptionSetValue>(defra_applicationline.Fields.defra_linetype) != null ? e.GetAttributeValue<OptionSetValue>(defra_applicationline.Fields.defra_linetype).Value : -1,
                        ApplicationId = e.GetAttributeId(defra_applicationline.Fields.defra_applicationId).Value,
                        ApplicationLineId = e.Id,
                        ItemId = e.GetAttributeIdOrDefault(defra_applicationline.Fields.defra_itemid),
                        Value = e.GetAttributeValue<Money>(defra_applicationline.Fields.defra_value),
                        ItemName = e.GetAttributeValue<EntityReference>(defra_applicationline.Fields.defra_itemid) != null ? (e.GetAttributeValue<EntityReference>(defra_applicationline.Fields.defra_itemid)).Name : null

                    }).ToList().Where(x => x.Value.Value != 0).OrderByDescending(x => x.Value.Value).ToList();

            var order = 1;
            foreach (var line in retVal)
            {
                var fetch = @"<fetch >
                                  <entity name='defra_applicationline'>
                                    <attribute name='defra_applicationlineid' />
   
                                    <filter type='and'>
                                      <condition attribute='defra_parentapplicationline' operator='eq'  uitype='defra_applicationline' value='{" + line.ApplicationLineId + @"}' />
                                    </filter>
                                  </entity>
                                </fetch>";
                var lineHasDiscountChild = _Service.RetrieveMultiple(new FetchExpression(fetch)).Entities.FirstOrDefault();

                var ent = new Entity(defra_applicationline.EntityLogicalName);
                ent.Id = line.ApplicationLineId;
                ent.Attributes.Add(defra_applicationline.Fields.defra_displayorder, order);
                _Service.Update(ent);


                // has child
                if (lineHasDiscountChild != null)
                {

                    ent = new Entity(defra_applicationline.EntityLogicalName);
                    ent.Id = lineHasDiscountChild.Id;
                    ent.Attributes.Add(defra_applicationline.Fields.defra_displayorder, ++order);
                    _Service.Update(ent);


                }
                order++;

            }

            var retVal1 = appLines
                  .Select(e => new MyAppLine
                  {
                      LineType = e.GetAttributeValue<OptionSetValue>(defra_applicationline.Fields.defra_linetype) != null ? e.GetAttributeValue<OptionSetValue>(defra_applicationline.Fields.defra_linetype).Value : -1,
                      ApplicationId = e.GetAttributeId(defra_applicationline.Fields.defra_applicationId).Value,
                      ApplicationLineId = e.Id,
                      ItemId = e.GetAttributeIdOrDefault(defra_applicationline.Fields.defra_itemid),
                      Value = e.GetAttributeValue<Money>(defra_applicationline.Fields.defra_value),
                      ItemName = e.GetAttributeValue<EntityReference>(defra_applicationline.Fields.defra_itemid) != null ? (e.GetAttributeValue<EntityReference>(defra_applicationline.Fields.defra_itemid)).Name : null

                  }).ToList().Where(x => x.Value.Value == 0).OrderByDescending(x => x.Value.Value).ToList();


            foreach (var line in retVal1)
            {

                var ent = new Entity(defra_applicationline.EntityLogicalName);
                ent.Id = line.ApplicationLineId;
                ent.Attributes.Add(defra_applicationline.Fields.defra_displayorder, order);
                _Service.Update(ent);
                order++;


            }
        }

        private EntityCollection GetApplicationLinesOnlyRegulatedFacilities(Guid applicationId, Guid? excludeApplicationLineId = null)
        {
            // Get all the other application lines linked to the same application
            QueryExpression appLinesRulesQuery = new QueryExpression(ApplicationLine.EntityLogicalName)
            {
                ColumnSet =
                    new ColumnSet(
                        defra_applicationline.Fields.defra_npsdetermination,
                        defra_applicationline.Fields.defra_locationscreeningrequired,
                        defra_applicationline.Fields.StateCode,
                        defra_applicationline.Fields.defra_linetype,
                        defra_applicationline.Fields.defra_standardruleId,
                        defra_applicationline.Fields.defra_itemid,
                        defra_applicationline.Fields.defra_item_type,
                        defra_applicationline.Fields.defra_value,
                        defra_applicationline.Fields.defra_applicationId,
                         defra_applicationline.Fields.defra_parentapplicationline),
                Criteria = new FilterExpression()
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(defra_applicationline.Fields.defra_applicationId, ConditionOperator.Equal, applicationId),
                        new ConditionExpression(defra_applicationline.Fields.StateCode, ConditionOperator.Equal, (int)ApplicationLineStates.Active),
                        new ConditionExpression(defra_applicationline.Fields.defra_linetype, ConditionOperator.Equal, (int)ApplicationLineTypeValues.RegulatedFacility)
                    }
                }
            };

            if (excludeApplicationLineId.HasValue)
            {
                appLinesRulesQuery.Criteria.Conditions.Add(new ConditionExpression(ApplicationLine.ApplicationLineId,
                    ConditionOperator.NotEqual, _Context.PrimaryEntityId));
            }

            return _Service.RetrieveMultiple(appLinesRulesQuery);
        }

        private EntityCollection GetAllApplicationLines(Guid applicationId, Guid? excludeApplicationLineId = null)
        {
            // Get all the other application lines linked to the same application
            QueryExpression appLinesRulesQuery = new QueryExpression(ApplicationLine.EntityLogicalName)
            {
                ColumnSet =
                    new ColumnSet(
                        defra_applicationline.Fields.defra_npsdetermination,
                        defra_applicationline.Fields.defra_locationscreeningrequired,
                        defra_applicationline.Fields.StateCode,
                        defra_applicationline.Fields.defra_linetype,
                        defra_applicationline.Fields.defra_standardruleId,
                        defra_applicationline.Fields.defra_itemid,
                        defra_applicationline.Fields.defra_item_type,
                        defra_applicationline.Fields.defra_value,
                        defra_applicationline.Fields.defra_applicationId,
                         defra_applicationline.Fields.defra_parentapplicationline),
                Criteria = new FilterExpression()
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(defra_applicationline.Fields.defra_applicationId, ConditionOperator.Equal, applicationId),
                        new ConditionExpression(defra_applicationline.Fields.StateCode, ConditionOperator.Equal, (int)ApplicationLineStates.Active),
                    }
                }
            };

            if (excludeApplicationLineId.HasValue)
            {
                appLinesRulesQuery.Criteria.Conditions.Add(new ConditionExpression(ApplicationLine.ApplicationLineId,
                    ConditionOperator.NotEqual, _Context.PrimaryEntityId));
            }

            return _Service.RetrieveMultiple(appLinesRulesQuery);
        }

    }
}
