namespace Core.Helpers.Extensions
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Crm.Sdk.Messages;
    using System.Collections.Generic;

    /// <summary>
    /// CRM specific helper extensions
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
            if (!entity.Contains(attribute))
            {
                return null;
            }

            var aliasedAttribute = entity.GetAttributeValue<AliasedValue>(attribute);

            if (aliasedAttribute?.Value == null)
            {
                return null;
            }

            if (aliasedAttribute.Value is Guid)
            {
                return (Guid) aliasedAttribute.Value;
            }

            if (aliasedAttribute.Value is EntityReference)
            {
                return ((EntityReference)aliasedAttribute.Value).Id;
            }

            return null;
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

        /// <summary>
        /// Grants shared access to a team for any entity
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityReference">Reference to the entity that will have access granted</param>
        /// <param name="principals">Entity Refs for Teams or Users that will be granted permissions </param>
        /// <param name="shareAppend">Share append</param>
        /// <param name="shareAppendTo">Share append to</param>
        /// <param name="shareAssign">Share assign</param>
        /// <param name="shareDelete">Share delete</param>
        /// <param name="shareRead">Share read</param>
        /// <param name="shareShare">Share share</param>
        /// <param name="shareWrite">Share write</param>
        public static void GrantAccess(this IOrganizationService service, EntityReference entityReference, List<EntityReference> principals, bool shareAppend,
            bool shareAppendTo, bool shareAssign, bool shareDelete, bool shareRead, bool shareShare, bool shareWrite)
        {
            var grantRequest = new GrantAccessRequest
            {
                Target = entityReference,
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = (AccessRights)GetMask(shareAppend, shareAppendTo, shareAssign, shareDelete, shareRead,
                        shareShare, shareWrite)
                }
            };
            foreach (EntityReference principal in principals)
            {
                grantRequest.PrincipalAccess.Principal = principal;
                var grantResponse = (GrantAccessResponse)service.Execute(grantRequest);
            }
        }

        /// <summary>
        /// Revoke shared access for any entity 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityReference">Reference to the entity that will have access revoked</param>
        /// <param name="principals">Entity Refs for Teams or Users that will be granted permissions </param>
        public static void RevokeAccess(this IOrganizationService service, EntityReference entityReference, List<EntityReference> principals)
        {
            var revokeRequest = new RevokeAccessRequest { Target = entityReference };
            foreach (EntityReference principalObject in principals)
            {
                revokeRequest.Revokee = principalObject;
                RevokeAccessResponse revokeResponse = (RevokeAccessResponse)service.Execute(revokeRequest);
            }
        }

        /// <summary>
        /// Calculates a mask for the supplied access rights
        /// </summary>
        /// <param name="shareAppend">Share append</param>
        /// <param name="shareAppendTo">Share append to</param>
        /// <param name="shareAssign">Share assign</param>
        /// <param name="shareDelete">Share delete</param>
        /// <param name="shareRead">Share read</param>
        /// <param name="shareShare">Share share</param>
        /// <param name="shareWrite">Share write</param>
        /// <returns>Unsigned integer mask</returns>
        private static uint GetMask(bool shareAppend, bool shareAppendTo, bool shareAssign, bool shareDelete, bool shareRead,
            bool shareShare, bool shareWrite)
        {
            uint mask = 0;
            if (shareAppend)
            {
                mask |= (uint)AccessRights.AppendAccess;
            }
            if (shareAppendTo)
            {
                mask |= (uint)AccessRights.AppendToAccess;
            }
            if (shareAssign)
            {
                mask |= (uint)AccessRights.AssignAccess;
            }
            if (shareDelete)
            {
                mask |= (uint)AccessRights.DeleteAccess;
            }
            if (shareRead)
            {
                mask |= (uint)AccessRights.ReadAccess;
            }
            if (shareShare)
            {
                mask |= (uint)AccessRights.ShareAccess;
            }
            if (shareWrite)
            {
                mask |= (uint)AccessRights.WriteAccess;
            }
            return mask;
        }

        /// <summary>
        /// Outout entity contents
        /// </summary>
        /// <param name="tracingService"></param>
        /// <param name="entity"></param>
        public static void TraceEntity(ITracingService tracingService, Entity entity)
        {
            if (entity == null)
            {
                tracingService.Trace($"Entity is null");
                return;
            }

            tracingService.Trace($"Entity LogicalName={entity.LogicalName}, id={entity.Id}");
            foreach (var attribute in entity.Attributes)
            {
                string value = attribute.Value?.ToString() ?? string.Empty;
                value = value.SafeSubstring(0, 50);
                tracingService.Trace($"{attribute.Key}={value}");
            }
        }
    }
}
