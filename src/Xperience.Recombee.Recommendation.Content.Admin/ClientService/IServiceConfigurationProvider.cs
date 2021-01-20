using CMS;

using Xperience.Recombee.Recommendation.Content.Admin;

[assembly: RegisterImplementation(typeof(IServiceConfigurationProvider), typeof(ServiceConfigurationProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Provides configuration of the Recombee service.
    /// </summary>
    public interface IServiceConfigurationProvider
    {
        /// <summary>
        /// Gets the Recombee DB identifier (API identifier).
        /// </summary>
        /// <param name="siteName">Name of site for which to return the DB identifier.</param>
        /// <returns>Returns the DB identifier for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetDatabaseId(string siteName);


        /// <summary>
        /// Gets the Recombee private token.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the token.</param>
        /// <returns>Returns the private token for <paramref name="siteName"/>, or null if not configured.</returns>
        string GetPrivateToken(string siteName);
    }
}
