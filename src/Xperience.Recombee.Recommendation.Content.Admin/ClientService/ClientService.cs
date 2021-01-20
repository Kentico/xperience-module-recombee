using System;

using Recombee.ApiClient;
using Recombee.ApiClient.ApiRequests;
using Recombee.ApiClient.Bindings;

namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Encapsulates a <see cref="Recombee.ApiClient.RecombeeClient"/> instance for a site.
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly IDatabaseManager databaseManager;
        private readonly RecombeeClient recombeeClient;
        private readonly string siteName;


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientService"/> class.
        /// </summary>
        /// <param name="databaseManager">Recombee database manager.</param>
        /// <param name="recombeeClient">Recombee client to be managed.</param>
        /// <param name="siteName">Name of site the Recombee client belongs to.</param>
        public ClientService(IDatabaseManager databaseManager, RecombeeClient recombeeClient, string siteName)
        {
            this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            this.recombeeClient = recombeeClient ?? throw new ArgumentNullException(nameof(recombeeClient));
            this.siteName = siteName ?? throw new ArgumentNullException(nameof(siteName));
        }


        /// <summary>
        /// Sends a <see cref="Batch"/> request.
        /// </summary>
        /// <param name="request">Request to be sent.</param>
        /// <returns>Returns the batch response.</returns>
        /// <remarks>
        /// The method makes sure the Recombee database structure is initialized, if not initialized previously.
        /// </remarks>
        public BatchResponse Send(Batch request)
        {
            databaseManager.EnsureDatabaseStructure(recombeeClient, siteName);

            return recombeeClient.Send(request);
        }


        /// <summary>
        /// Sends a <see cref="Request"/>.
        /// </summary>
        /// <param name="request">Request to be sent.</param>
        /// <returns>Returns the request's response.</returns>
        /// <remarks>
        /// The method makes sure the Recombee database structure is initialized, if not initialized previously.
        /// </remarks>
        public StringBinding Send(Request request)
        {
            databaseManager.EnsureDatabaseStructure(recombeeClient, siteName);

            return recombeeClient.Send(request);
        }


        /// <summary>
        /// Resets the Recombee database.
        /// </summary>
        /// <remarks>
        /// Always use this method to perform the database reset. This notifies the system the database structure has to be re-initialized
        /// before subsequent requests.
        /// </remarks>
        public void Reset()
        {
            databaseManager.Reset(recombeeClient, siteName);
        }
    }
}
