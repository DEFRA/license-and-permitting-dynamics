namespace Lp.DataAccess.Extensions
{
    using System;
    using Microsoft.Xrm.Sdk;

    public static class CrmExtensions
    {
        public static Guid? GetAttributeId(this Entity entity, string attribute)
        {
            return entity.Contains(attribute) ? ((EntityReference)entity[attribute]).Id : (Guid?)null;
        }

        public static string GetAttributeText(this Entity entity, string attribute)
        {
            return entity.Contains(attribute) ? entity[attribute].ToString() : null;
        }
    }
}
