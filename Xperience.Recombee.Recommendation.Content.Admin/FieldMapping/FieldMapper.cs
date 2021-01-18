using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using CMS.DocumentEngine;

namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Performs mapping of page type fields to Recombee database item properties.
    /// </summary>
    public class FieldMapper : IFieldMapper
    {
        /// <summary>
        /// An ordered collection of individual field mappings for a page type.
        /// </summary>
        private readonly ConcurrentDictionary<string, PageTypeMappings> configurations = new ConcurrentDictionary<string, PageTypeMappings>(4, 4, StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Gets the <see cref="PageTypeMappings"/> for a site.
        /// </summary>
        /// <param name="siteName">Name of site for which to retrieve the configuration.</param>
        /// <returns>Returns the site's configurations of page type mappings.</returns>
        public PageTypeMappings GetConfigurations(string siteName)
        {
            if (configurations.TryGetValue(siteName, out var configuration))
            {
                return configuration;
            }

            var newConfiguration = new PageTypeMappings();
            if (configurations.TryAdd(siteName, newConfiguration))
            {
                return newConfiguration;
            }

            configurations.TryGetValue(siteName, out var configuration2);

            return configuration2;
        }


        /// <summary>
        /// Maps a page to Recombee database item.
        /// </summary>
        /// <param name="page">Page to be mapped.</param>
        /// <returns>Returns the mapped Recombee database item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the page's site does not contain the configuration for page's type.</exception>
        public Dictionary<string, object> Map(TreeNode page)
        {
            var pageType = page.TypeInfo.ObjectType;
            var siteName = page.NodeSiteName;

            if (!configurations.TryGetValue(siteName, out var fieldTypesMappings))
            {
                throw new InvalidOperationException($"Could not find page type mappings container for site '{siteName}'. Use the {typeof(IFieldMapper)}.{nameof(IFieldMapper.GetConfigurations)} to initialize new mappings container for the site.");
            }

            if (!fieldTypesMappings.Mappings.TryGetValue(pageType, out var mapping))
            {
                throw new InvalidOperationException($"Page type '{pageType}' has no mapping configured. Use the {typeof(IFieldMapper)}.{nameof(IFieldMapper.GetConfigurations)} property to define the page type mapping.");
            }

            var result = new Dictionary<string, object>();

            MapSystemFields(page, result);

            foreach(var field in mapping)
            {
                if (result.ContainsKey(field.TargetName))
                {
                    result[field.TargetName] = MergeValues(pageType, field.SourceName, GetMappedValue(field, page), field.TargetName, result[field.TargetName]); 
                }
                else
                {
                    result[field.TargetName] = GetMappedValue(field, page);
                }
            }

            return result;
        }


        /// <summary>
        /// Gets source value from <paramref name="page"/>.
        /// </summary>
        /// <param name="fieldMapping">Descriptor of the field mapping.</param>
        /// <param name="page">Page to be mapped.</param>
        /// <returns>Returns the value to be mapped to the Recombee DB.</returns>
        protected virtual object GetMappedValue(FieldMapping fieldMapping, TreeNode page)
        {
            if (fieldMapping.SourceValue != null)
            {
                return fieldMapping.SourceValue(page);
            }
            return page.GetValue(fieldMapping.SourceName);
        }


        /// <summary>
        /// Maps system fields of <paramref name="page"/> to the resulting Recombee item.
        /// </summary>
        /// <param name="page">Page to be mapped.</param>
        /// <param name="result">Recombee database item to map into.</param>
        /// <seealso cref="DatabaseConfiguration.PAGE_TYPE_PROPERTY_NAME"/>
        /// <seealso cref="DatabaseConfiguration.CULTURE_PROPERTY_NAME"/>
        protected virtual void MapSystemFields(TreeNode page, Dictionary<string, object> result)
        {
            result[DatabaseConfiguration.PAGE_TYPE_PROPERTY_NAME] = page.TypeInfo.ObjectType.ToLowerInvariant();
            result[DatabaseConfiguration.CULTURE_PROPERTY_NAME] = page.DocumentCulture.ToLowerInvariant();
        }


        /// <summary>
        /// Merges values when multiple page fields are mapped to a single Recombee item property.
        /// </summary>
        /// <param name="pageType">Page type whose values are being merged.</param>
        /// <param name="fieldName">Field name in the page type.</param>
        /// <param name="fieldValue">Page field value to be merged with existing value in <paramref name="targetPropertyValue"/>.</param>
        /// <param name="targetPropertyName">Target property name in the Recombee database.</param>
        /// <param name="targetPropertyValue">Current property value to be merged with <paramref name="fieldValue"/>.</param>
        /// <returns>Returns the merged value for <paramref name="fieldValue"/> and <paramref name="targetPropertyValue"/>.</returns>
        /// <remarks>
        /// The default implementation merges only string values by concatenating them together, separated by a single space character.
        /// For all other values throws <see cref="InvalidOperationException"/>.
        /// </remarks>
        protected virtual object MergeValues(string pageType, string fieldName, object fieldValue, string targetPropertyName, object targetPropertyValue)
        {
            if (fieldValue is string fieldString && targetPropertyValue is string targetPropertyString)
            {
                return $"{targetPropertyString} {fieldString}";
            }

            throw new InvalidOperationException($"No merging is defined for field '{fieldName}' with value of type '{fieldValue?.GetType().FullName ?? "null"}' (page type '{pageType}')." +
                $" The system does not know how to merge it together with existing value in target Recombee item property '{targetPropertyName}' of type '{targetPropertyValue?.GetType().FullName ?? "null"}'." +
                $" Override the {typeof(FieldMapper).FullName}.{nameof(MergeValues)}() method and register it as {typeof(IFieldMapper).FullName} implementation to define a custom merging algorithm. The values of the page field and target Recombee item property follow:{Environment.NewLine}" +
                $"Page field value: {fieldValue}{Environment.NewLine}{Environment.NewLine}" +
                $"Target item property value: {targetPropertyValue}");
        }
    }
}
