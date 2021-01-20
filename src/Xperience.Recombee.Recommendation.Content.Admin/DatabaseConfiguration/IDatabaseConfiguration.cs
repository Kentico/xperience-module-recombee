using CMS;

using System.Collections.Generic;

using Xperience.Recombee.Recommendation.Content.Admin;

[assembly: RegisterImplementation(typeof(IDatabaseConfiguration), typeof(DatabaseConfiguration), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Maintains the Recombee database configuration for individual sites.
    /// </summary>
    public interface IDatabaseConfiguration
    {
        /// <summary>
        /// Gets the Recombee database configuration (item properties) for a site.
        /// </summary>
        /// <param name="siteName">Name of site for which to retrieve the configuration.</param>
        /// <returns>Returns the site's configuration.</returns>
        IList<DatabaseProperty> Get(string siteName);

        
    }
}
