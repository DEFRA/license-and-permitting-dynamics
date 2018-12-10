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
        /// <returns></returns>
        public static Guid? GetAttributeId(this Entity entity, string attribute)
        {
            return entity.Contains(attribute) ? ((EntityReference)entity[attribute]).Id : (Guid?)null;
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
    }
}
