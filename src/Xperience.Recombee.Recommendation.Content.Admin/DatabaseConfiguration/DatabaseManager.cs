using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Core;

using Recombee.ApiClient;
using Recombee.ApiClient.ApiRequests;


namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Performs initialization and reset of the Recommbee database.
    /// </summary>
    public class DatabaseManager : IDatabaseManager
    {
        private readonly HashSet<string> structureEnsured = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly object syncLock = new object();


        private readonly IDatabaseConfiguration databaseConfiguration;
        private readonly IEventLogService eventLogService;


        public DatabaseManager(IDatabaseConfiguration databaseConfiguration, IEventLogService eventLogService)
        {
            this.databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
        }


        /// <summary>
        /// Initializes the Recombee database based on its <see cref="IDatabaseConfiguration"/>.
        /// The initialization is performed only once per application lifetime unless <see cref="Reset(string)"/> is performed.
        /// </summary>
        /// <param name="recombeeClient">Recombee client to be used for the Recombee database initialization.</param>
        /// <param name="siteName">Name of site to which the <see cref="RecombeeClient"/> belongs.</param>
        public void EnsureDatabaseStructure(RecombeeClient recombeeClient, string siteName)
        {
            if (structureEnsured.Contains(siteName))
            {
                return;
            }

            lock (syncLock)
            {
                if (structureEnsured.Contains(siteName))
                {
                    return;
                }

                InitializeItemProperties(recombeeClient, siteName);

                structureEnsured.Add(siteName);
            }
        }


        /// <summary>
        /// Resets the Recombee database content and structure.
        /// </summary>
        /// <param name="recombeeClient">Recombee client to be used for the reset.</param>
        /// <param name="siteName">Name of site to which the <see cref="RecombeeClient"/> belongs.</param>
        public void Reset(RecombeeClient recombeeClient, string siteName)
        {
            lock (syncLock)
            {
                structureEnsured.Remove(siteName);
            }

            recombeeClient.Send(new ResetDatabase());
        }


        protected virtual void InitializeItemProperties(RecombeeClient recombeeClient, string siteName)
        {
            var itemProperties = databaseConfiguration.Get(siteName);

            if (itemProperties.Count == 0)
            {
                eventLogService.LogWarning("Recombee.Recommendation.Content", "Item properties init", $"No Recombee database item properties are configured for site '{siteName}'. Use the {typeof(IDatabaseConfiguration).FullName}.{nameof(IDatabaseConfiguration.Get)} method to configure them.");

                return;
            }

            try
            {
                var batch = new Batch(itemProperties.Select(ip => new AddItemProperty(ip.Name, ip.Type)));

                recombeeClient.Send(batch);

            }
            catch (Exception ex)
            {
                eventLogService.LogException("Recombee.Recommendation.Content", "Item properties init", ex);

                throw;
            }
        }
    }
}
