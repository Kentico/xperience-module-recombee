using System;

using Recombee.ApiClient;
using Recombee.ApiClient.ApiRequests;
using Recombee.ApiClient.Bindings;

namespace Xperience.Recombee.Recommendation.Content
{
    /// <summary>
    /// Encapsulates a <see cref="Recombee.ApiClient.RecombeeClient"/> instance for a site.
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly RecombeeClient recombeeClient;


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientService"/> class.
        /// </summary>
        /// <param name="recombeeClient">Recombee client to be managed.</param>
        public ClientService(RecombeeClient recombeeClient)
        {
            this.recombeeClient = recombeeClient ?? throw new ArgumentNullException(nameof(recombeeClient));
        }


        /// <summary>
        /// Sends a <see cref="Request"/>.
        /// </summary>
        /// <param name="request">Request to be sent.</param>
        /// <returns>Returns the request's response.</returns>
        public StringBinding Send(Request request)
        {
            return recombeeClient.Send(request);
        }


        /// <summary>
        /// Executes <paramref name="func"/> passing in the Recombee client.
        /// </summary>
        /// <typeparam name="TResult">Return type of the <paramref name="func"/>.</typeparam>
        /// <param name="func">Function to be executed on the Recombee client.</param>
        /// <returns>Returns the functions result.</returns>
        public TResult Execute<TResult>(Func<RecombeeClient, TResult> func)
        {
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            return func(recombeeClient);
        }
    }
}
