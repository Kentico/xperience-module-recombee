using Recombee.ApiClient;
using Recombee.ApiClient.ApiRequests;
using Recombee.ApiClient.Bindings;

namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Encapsulates a <see cref="RecombeeClient"/> instance for a site.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Sends a <see cref="Batch"/> request.
        /// </summary>
        /// <param name="request">Request to be sent.</param>
        /// <returns>Returns the batch response.</returns>
        /// <remarks>
        /// The method makes sure the Recombee database structure is initialized, if not initialized previously.
        /// </remarks>
        BatchResponse Send(Batch request);


        /// <summary>
        /// Sends a <see cref="Request"/>.
        /// </summary>
        /// <param name="request">Request to be sent.</param>
        /// <returns>Returns the request's response.</returns>
        /// <remarks>
        /// The method makes sure the Recombee database structure is initialized, if not initialized previously.
        /// </remarks>
        StringBinding Send(Request request);


        /// <summary>
        /// Resets the Recombee database.
        /// </summary>
        /// <remarks>
        /// Always use this method to perform the database reset. This notifies the system the database structure has to be re-initialized
        /// before subsequent requests.
        /// </remarks>
        void Reset();
    }
}
