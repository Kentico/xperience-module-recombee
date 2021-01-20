using System;
using System.Collections.Generic;

using CMS.Core;

using Recombee.ApiClient;

namespace Xperience.Recombee.Recommendation.Content
{
    /// <summary>
    /// Provider of site-specific <see cref="IClientService"/>.
    /// </summary>
    public class ClientServiceProvider : IClientServiceProvider
    {
        private readonly IServiceConfigurationProvider serviceConfigurationProvider;
        private readonly IEventLogService eventLogService;

        private readonly object initLock = new object();
        private readonly Dictionary<string, IClientService> clientServices = new Dictionary<string, IClientService>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientServiceProvider"/> class.
        /// </summary>
        /// <param name="serviceConfigurationProvider">Provider of the Recombee service configuration.</param>
        /// <param name="eventLogService">Event log service.</param>
        public ClientServiceProvider(IServiceConfigurationProvider serviceConfigurationProvider, IEventLogService eventLogService)
        {
            this.serviceConfigurationProvider = serviceConfigurationProvider ?? throw new ArgumentNullException(nameof(serviceConfigurationProvider));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
        }


        /// <summary>
        /// Gets a value indicating whether the Recombee client service is available for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to test the availability.</param>
        /// <returns>Returns true if the client service is available for <paramref name="siteName"/>, otherwise returns false.</returns>
        public bool IsAvailable(string siteName)
        {
            return Get(siteName) != null;
        }


        /// <summary>
        /// Gets the Recombee client services for the specified site.
        /// </summary>
        /// <param name="siteName">Name of site for which to return the client service.</param>
        /// <returns>Returns the client service, or null if client service is not available for the site.</returns>
        public IClientService Get(string siteName)
        {
            if (clientServices.TryGetValue(siteName, out var cs))
            {
                return cs;
            }

            lock (initLock)
            {
                if (clientServices.TryGetValue(siteName, out var cs2))
                {
                    return cs2;
                }


                var databaseId = serviceConfigurationProvider.GetDatabaseId(siteName);
                var privateToken = serviceConfigurationProvider.GetPrivateToken(siteName);
                if (String.IsNullOrEmpty(databaseId) || privateToken == null)
                {
                    clientServices.Add(siteName, null);

                    eventLogService.LogWarning("Recombee", "MISSINGCREDENTIALS", $"Live site app settings do not contain Recombee database ID or private token for site '{siteName}'.");

                    return null;
                }

                var recombeeClient = new RecombeeClient(databaseId, privateToken);

                var clientService = new ClientService(recombeeClient);
                clientServices.Add(siteName, clientService);

                return clientService;
            }
        }
    }
}
