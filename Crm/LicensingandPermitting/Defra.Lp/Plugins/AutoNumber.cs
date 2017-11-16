// <copyright file="AutoNumber.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>10/17/2017 3:18:59 PM</date>
// <summary>Implements the AutoNumber Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>

using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Xml.Linq;
using Microsoft.Xrm.Sdk.Client;

namespace Defra.Lp.Plugins
{

    /// <summary>
    /// AutoNumber Plugin.
    /// </summary>    
    public class AutoNumber: PluginBase
    {
        private ITracingService TracingService { get; set; }
        private IPluginExecutionContext Context { get; set; }
        private IOrganizationService Service { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoNumber"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information. 
        /// When using Microsoft Dynamics 365 for Outlook with Offline Access, 
        /// the secure string is not passed to a plug-in that executes while the client is offline.</param>
        public AutoNumber(string unsecure, string secure)
            : base(typeof(AutoNumber))
        {
            
           // TODO: Implement your custom configuration handling.
        }


        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics 365 caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }

            TracingService = localContext.TracingService;
            Context = localContext.PluginExecutionContext;
            Service = localContext.OrganizationService;

            if (Context.InputParameters.Contains("Target") && Context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)Context.InputParameters["Target"];
                if (entity.LogicalName == "defra_application")
                {
                    var qe = new QueryExpression("defra_autonumbering");
                    qe.ColumnSet = new ColumnSet(new string[] { "defra_prefix", "defra_suffix", "defra_currentnumber", "defra_numberlength" });
                    qe.Criteria = new FilterExpression();
                    qe.Criteria.AddCondition("defra_name", ConditionOperator.Equal,  "Waste");
                    var autonumbers = Service.RetrieveMultiple(qe);
                    if (autonumbers.Entities != null && autonumbers.Entities.Count > 0)
                    {
                        var autonumber = autonumbers[0];
                        var current = (int)autonumber["defra_currentnumber"];
                        var next = current + 1;

                        var prefix = autonumber["defra_prefix"];
                        var suffix = autonumber["defra_suffix"];
                        var code = current.ToString("000");
                        entity["defra_name"] = string.Format("{0}{1}{2}", prefix, code, suffix);
                        entity["defra_applicationnumber"] = entity["defra_name"];

                        autonumber["defra_currentnumber"] = next;
                        Service.Update(autonumber);
                    }
                }
            }
        }
    }
}