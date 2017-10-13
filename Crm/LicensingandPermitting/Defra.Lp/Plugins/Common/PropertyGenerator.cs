using Defra.Lp.Common.ProxyClasses;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace Defra.Lp.Common
{
    internal class PropertyGenerator
    {
        private IOrganizationService AdminService { get; set; }

        public PropertyGenerator(IOrganizationService adminService)
        {
            AdminService = adminService;
        }

        internal void AddProperty<T>(List<FieldMetadata> properties, string crmEntityAlias, string crmFieldName, string sharePointFieldName, Entity entity)
        {
            AddProperty<T>(properties, crmEntityAlias, crmFieldName, false, sharePointFieldName, entity, false);
        }

        internal void AddProperty<T>(List<FieldMetadata> properties, string crmFieldName, string sharePointFieldName, Entity entity)
        {
            AddProperty<T>(properties, null, crmFieldName, true, sharePointFieldName, entity, false);
        }

        internal void AddProperty<T>(List<FieldMetadata> properties, string crmFieldName, bool ignoreAlias, string sharePointFieldName, Entity entity, bool managedMetaDataType)
        {
            AddProperty<T>(properties, null, crmFieldName, ignoreAlias, sharePointFieldName, entity, managedMetaDataType);
        }

        internal void AddProperty<T>(List<FieldMetadata> properties, string crmEntityAlias, string crmFieldName, string sharePointFieldName, Entity entity, bool managedMetaDataType)
        {
            AddProperty<T>(properties, crmEntityAlias, crmFieldName, false, sharePointFieldName, entity, managedMetaDataType);
        }

        internal void AddProperty<T>(List<FieldMetadata> properties, string crmFieldName, string sharePointFieldName, Entity entity, bool managedMetaDataType)
        {
            AddProperty<T>(properties, null, crmFieldName, true, sharePointFieldName, entity, managedMetaDataType);
        }

        internal void AddProperty<T>(List<FieldMetadata> properties, string crmEntityAlias, string crmFieldName, bool ignoreAlias, string sharePointFieldName, Entity entity)
        {
            AddProperty<T>(properties, crmEntityAlias, crmFieldName, ignoreAlias, sharePointFieldName, entity, false);
        }

        internal void AddProperty<T>(List<FieldMetadata> properties, string crmEntityAlias, string crmFieldName, bool ignoreAlias, string sharePointFieldName, Entity entity, bool managedMetaDataType)
        {
            FieldMetadata propertyValue = CreateFieldMetadataInstance(properties, sharePointFieldName, managedMetaDataType);

            string fieldName = crmFieldName;

            if (!string.IsNullOrEmpty(crmEntityAlias) && !ignoreAlias)
            {
                fieldName = crmEntityAlias + "." + crmFieldName;
            }

            if (entity.Attributes.Contains(fieldName))
            {
                object value = entity.Attributes[fieldName];

                if (value != null)
                {
                    object sendValue = ConvertCRMValueToSharePointValue<T>(crmEntityAlias, crmFieldName, value);

                    propertyValue.Value = sendValue;
                }
            }
        }

        internal void AddProperty<T>(List<FieldMetadata> properties, string sharePointFieldName, object value, bool managedMetaDataType)
        {
            FieldMetadata propertyValue = CreateFieldMetadataInstance(properties, sharePointFieldName, managedMetaDataType);
            propertyValue.Value = ConvertCRMValueToSharePointValue<T>(value);
        }

        private static FieldMetadata CreateFieldMetadataInstance(List<FieldMetadata> properties, string sharePointFieldName, bool managedMetaDataType)
        {
            FieldMetadata propertyValue = new FieldMetadata();

            propertyValue.Name = sharePointFieldName;
            propertyValue.Value = null;
            propertyValue.ManagedMetadataType = managedMetaDataType;

            properties.Add(propertyValue);
            return propertyValue;
        }

        private object ConvertCRMValueToSharePointValue<T>(object value)
        {
            return ConvertCRMValueToSharePointValue<T>(null, null, value);
        }

        private object ConvertCRMValueToSharePointValue<T>(string crmEntityAlias, string crmFieldName, object value)
        {
            object result = null;

            if (value is AliasedValue)
            {
                AliasedValue aliasedValue = value as AliasedValue;

                result = (T)aliasedValue.Value;
            }
            else
            {
                result = (T)value;
            }

            //Conversions to support SharePoint
            if (result is OptionSetValue)
            {
                if (string.IsNullOrEmpty(crmEntityAlias) || string.IsNullOrEmpty(crmFieldName))
                {
                    throw new Exception("Error converting CRM Value to SharePoint Value; CRM Entity Alias or CRM Field Name not specified");
                }

                result = Query.GetCRMOptionsetText(AdminService, crmEntityAlias, crmFieldName, ((OptionSetValue)result).Value);
            }
            else if (result is bool)
            {
                result = (bool)result ? "Yes" : "No";
            }

            return result;
        }
    }
}