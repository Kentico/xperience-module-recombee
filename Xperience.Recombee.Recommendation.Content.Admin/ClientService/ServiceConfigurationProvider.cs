using System;

using CMS.Core;

namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Provides configuration of the Recombee service.
    /// </summary>
    public class ServiceConfigurationProvider : IServiceConfigurationProvider
    {
        private const string DATABASE_ID_KEY_NAME = "Recombee.ContentRecommendation.DatabaseId";
        private const string PRIVATE_TOKEN_KEY_NAME = "Recombee.ContentRecommendation.PrivateToken";


        private readonly IAppSettingsService appSettingsService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfigurationProvider"/> class.
        /// </summary>
        /// <param name="appSettingsService">Application settings service.</param>
        public ServiceConfigurationProvider(IAppSettingsService appSettingsService)
        {
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        }


        /// <summary>
        /// Gets the Recombee DB identifier (API identifier).
        /// </summary>
        /// <param name="siteName">Name of site for which to return the DB identifier.</param>
        /// <returns>Returns the DB identifier for <paramref name="siteName"/>, or null if not configured.</returns>
        /// <remarks>
        /// The method reads the '&lt;siteName&gt;.Recombee.ContentRecommendation.DatabaseId' configuration key from the application configuration file.
        /// </remarks>
        public string GetDatabaseId(string siteName)
        {
            var keyName = $"{siteName}.{DATABASE_ID_KEY_NAME}";

            return appSettingsService[keyName];
        }


        /// <summary>
        /// Gets the Recombee private token.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the token.</param>
        /// <returns>Returns the private token for <paramref name="siteName"/>, or null if not configured.</returns>
        /// <remarks>
        /// The method reads the '&lt;siteName&gt;.Recombee.ContentRecommendation.PrivateToken' configuration key from the application configuration file.
        /// </remarks>
        public string GetPrivateToken(string siteName)
        {
            var keyName = $"{siteName}.{PRIVATE_TOKEN_KEY_NAME}";

            return appSettingsService[keyName];
        }
    }
}
