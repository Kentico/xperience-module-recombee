using System;
using System.Collections.Generic;

using CMS.Core;

using Recombee.ApiClient;

namespace Kentico.Xperience.Recombee.Admin
{
    /// <summary>
    /// Provider of site-specific <see cref="IClientService"/>.
    /// </summary>
    public class ClientServiceProvider : IClientServiceProvider
    {
        private readonly IDatabaseManager databaseManager;
        private readonly IServiceConfigurationProvider serviceConfigurationProvider;
        private readonly IEventLogService eventLogservice;

        private readonly object initLock = new object();
        private readonly Dictionary<string, IClientService> clientServices = new Dictionary<string, IClientService>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientServiceProvider"/> class.
        /// </summary>
        /// <param name="databaseManager">Recombee database manager.</param>
        /// <param name="serviceConfigurationProvider">Provider of the Recombee service configuration.</param>
        /// <param name="eventLogservice">Event log service.</param>
        public ClientServiceProvider(IDatabaseManager databaseManager, IServiceConfigurationProvider serviceConfigurationProvider, IEventLogService eventLogservice)
        {
            this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            this.serviceConfigurationProvider = serviceConfigurationProvider ?? throw new ArgumentNullException(nameof(serviceConfigurationProvider));
            this.eventLogservice = eventLogservice ?? throw new ArgumentNullException(nameof(eventLogservice));
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

            lock(initLock)
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

                    eventLogservice.LogWarning("Recombee", "MISSINGCREDENTIALS", $"Administration site app settings do not contain Recombee database ID or private token for site '{siteName}'.");

                    return null;
                }

                var recombeeClient = new RecombeeClient(databaseId, privateToken);

                var clientService = new ClientService(databaseManager, recombeeClient, siteName);
                clientServices.Add(siteName, clientService);

                return clientService;
            }
        }
    }
}
