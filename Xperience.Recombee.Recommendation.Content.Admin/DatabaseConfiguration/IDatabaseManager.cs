using CMS;

using Xperience.Recombee.Recommendation.Content.Admin;

using Recombee.ApiClient;

[assembly: RegisterImplementation(typeof(IDatabaseManager), typeof(DatabaseManager), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Performs initialization and reset of the Recommbee database.
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// Initializes the Recombee database based on its <see cref="IDatabaseConfiguration"/>.
        /// The initialization is performed only once per application lifetime unless <see cref="Reset(string)"/> is performed.
        /// </summary>
        /// <param name="recombeeClient">Recombee client to be used for the Recombee database initialization.</param>
        /// <param name="siteName">Name of site to which the <see cref="RecombeeClient"/> belongs.</param>
        void EnsureDatabaseStructure(RecombeeClient recombeeClient, string siteName);


        /// <summary>
        /// Resets the Recombee database content and structure.
        /// </summary>
        /// <param name="recombeeClient">Recombee client to be used for the reset.</param>
        /// <param name="siteName">Name of site to which the <see cref="RecombeeClient"/> belongs.</param>
        void Reset(RecombeeClient recombeeClient, string siteName);
    }
}
