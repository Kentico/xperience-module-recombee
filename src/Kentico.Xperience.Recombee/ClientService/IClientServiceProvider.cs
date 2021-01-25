using CMS;

using Kentico.Xperience.Recombee;

[assembly: RegisterImplementation(typeof(IClientServiceProvider), typeof(ClientServiceProvider), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.Recombee
{
    /// <summary>
    /// Provider of site-specific <see cref="IClientService"/>.
    /// </summary>
    public interface IClientServiceProvider
    {
        /// <summary>
        /// Gets a value indicating whether the Recombee client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        bool IsAvailable(string siteName);


        /// <summary>
        /// Gets the Recombee client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the client service, or null if client service is not available for the site.</returns>
        IClientService Get(string siteName);
    }
}
