// Copyright (c) DEFRA 2018 All Rights Reserved
// </copyright>
// <author>Hugo Herrera</author>
// <summary>Implements the CompaniesHouseValidation Plugin.</summary>
using System;
using System.Collections.Generic;
using System.Net;
using CardPayments;
using Core.Configuration;
using Defra.Lp.Core.CardPayments.Workflow.Constants;
using Microsoft.Xrm.Sdk;

namespace Defra.Lp.Core.CardPayments.Workflow
{
    public class ConfigurationManager
    {
        public RestServiceConfiguration RetrieveCardPaymentServiceConfiguration(IOrganizationService organizationService, string configurationPrefix)
        {
            if (string.IsNullOrWhiteSpace(configurationPrefix))
            {
                configurationPrefix = String.Empty;
            }

            // Read the settings
            IDictionary<string, string> configSettings = organizationService.GetConfigurationStringValues(
                $"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.ApiKey}",
                $"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.TargetUrl}",
                $"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.TargetHost}",
                $"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.SecurityProtocol}",
                $"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.SecurityHeader}");

            // Parse the settings
            SecurityProtocolType protocolType;
            Enum.TryParse(
                configSettings[$"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.SecurityProtocol}"],
                out protocolType);

            RestServiceConfiguration config = new RestServiceConfiguration
            {
                ApiKey = configSettings[$"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.ApiKey}"],
                SecurityHeader = configSettings[$"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.SecurityHeader}"],
                SecurityProtocol = protocolType,
                TargetHost = configSettings[$"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.TargetHost}"],
                TargetUrl = configSettings[$"{configurationPrefix}{CardPaymentServiceSecureConfigurationKeys.TargetUrl}"]
            };
            return config;
        }
    }
}