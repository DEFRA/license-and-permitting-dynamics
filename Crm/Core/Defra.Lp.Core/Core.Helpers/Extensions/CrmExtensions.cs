namespace Core.Helpers.Extensions
{
    using System;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// CRM specific helper extentensions
    /// </summary>
    public static class CrmExtensions
    {
        /// <summary>
        /// Returns an Entity Reference attribute's Guid if it exists
        /// </summary>
        /// <param name="entity">The Entity that contains the attribute</param>
        /// <param name="attribute">Attribute name to retrieve</param>
        /// <returns>Guid</returns>
        public static Guid? GetAttributeId(this Entity entity, string attribute)
        {
            return entity.Contains(attribute) ? ((EntityReference)entity[attribute]).Id : (Guid?)null;
        }

        /// <summary>
        /// Returns an attribute as aGuid if it exists
        /// </summary>
        /// <param name="entity">The Entity that contains the attribute</param>
        /// <param name="attribute">Attribute name to retrieve</param>
        /// <returns>Guid</returns>
        public static Guid? GetAttributeGuid(this Entity entity, string attribute)
        {
            return entity.Contains(attribute) ? ((Guid)entity[attribute]) : (Guid?)null;
        }

        /// <summary>
        /// Returns an Aliased Entity Ref attribute's Guid if it exists
        /// </summary>
        /// <param name="entity">The Entity that contains the attribute</param>
        /// <param name="attribute">Attribute name to retrieve</param>
        /// <returns>Guid</returns>
        public static Guid? GetAliasedAttributeId(this Entity entity, string attribute)
        {
            return entity.Contains(attribute) ? (Guid)entity.GetAttributeValue<AliasedValue>(attribute).Value : (Guid?)null;
        }

        /// <summary>
        /// Returns an Aliased OptionSet attribute'Value Guid if it exists
        /// </summary>
        /// <param name="entity">The Entity that contains the attribute</param>
        /// <param name="attribute">Attribute name to retrieve</param>
        /// <returns>Optionset Int Value</returns>
        public static int? GetAliasedOptionSetValue(this Entity entity, string attribute)
        {
            return entity.Contains(attribute) ? ((OptionSetValue)entity
                .GetAttributeValue<AliasedValue>(attribute)
                .Value).Value : (int?)null;
        }

        /// <summary>
        /// Returns an Entity Reference attribute's text if it exists
        /// </summary>
        /// <param name="entity">The Entity that contains the attribute</param>
        /// <param name="attribute">Attribute name to retrieve</param>
        /// <returns></returns>
        public static string GetAttributeText(this Entity entity, string attribute)
        {
            return entity.Contains(attribute) ? entity[attribute].ToString() : null;
        }

        /// <summary>
        /// Returns an aliased Entity Reference attribute's text if it exists
        /// </summary>
        /// <param name="entity">The Entity that contains the attribute</param>
        /// <param name="attribute">Attribute name to retrieve</param>
        /// <returns>Attribute text</returns>
        public static string GetAliasedAttributeText(this Entity entity, string attribute)
        {
            return entity.Contains(attribute) ? (string)entity.GetAttributeValue<AliasedValue>(attribute).Value : string.Empty;
        }
    }
}
