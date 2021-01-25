using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Kentico.Xperience.Recombee.Admin
{
    /// <summary>
    /// Maintains the Recombee database configuration for individual sites.
    /// </summary>
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        /// <summary>
        /// Gets the name of the item property the system uses to store the page type.
        /// </summary>
        public const string PAGE_TYPE_PROPERTY_NAME = "_PageType";


        /// <summary>
        /// Gets the name of the item property the system uses to store the culture of a page.
        /// </summary>
        public const string CULTURE_PROPERTY_NAME = "_Culture";


        private readonly ConcurrentDictionary<string, IList<DatabaseProperty>> itemProperties = new ConcurrentDictionary<string, IList<DatabaseProperty>>(4, 4, StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Gets the Recombee database configuration (item properties) for a site.
        /// </summary>
        /// <param name="siteName">Name of site for which to retrieve the configuration.</param>
        /// <returns>Returns the site's configuration.</returns>
        public IList<DatabaseProperty> Get(string siteName)
        {
            if (itemProperties.TryGetValue(siteName, out var properties))
            {
                return properties;
            }

            var newProperties = CreateProperties();
            if (itemProperties.TryAdd(siteName, newProperties))
            {
                return newProperties;
            }

            itemProperties.TryGetValue(siteName, out var properties2);

            return properties2;
        }


        /// <summary>
        /// Creates a list of the Recombee database item properties.
        /// The list is pre-populated with system properties.
        /// </summary>
        /// <returns>Returns a new list of the item properties.</returns>
        /// <seealso cref="PAGE_TYPE_PROPERTY_NAME"/>
        /// <seealso cref="CULTURE_PROPERTY_NAME"/>
        protected virtual IList<DatabaseProperty> CreateProperties()
        {
            var properties = new List<DatabaseProperty>();

            properties.Add(new DatabaseProperty(PAGE_TYPE_PROPERTY_NAME, "string"));
            properties.Add(new DatabaseProperty(CULTURE_PROPERTY_NAME, "string"));

            return properties;
        }
    }
}
