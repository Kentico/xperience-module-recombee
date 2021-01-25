using System;

using Recombee.ApiClient;
using Recombee.ApiClient.ApiRequests;
using Recombee.ApiClient.Bindings;

namespace Kentico.Xperience.Recombee
{
    /// <summary>
    /// Encapsulates a <see cref="RecombeeClient"/> instance for a site.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Sends a <see cref="Request"/>.
        /// </summary>
        /// <param name="request">Request to be sent.</param>
        /// <returns>Returns the request's response.</returns>
        StringBinding Send(Request request);


        /// <summary>
        /// Executes <paramref name="func"/> passing in the Recombee client.
        /// </summary>
        /// <typeparam name="TResult">Return type of the <paramref name="func"/>.</typeparam>
        /// <param name="func">Function to be executed on the Recombee client.</param>
        /// <returns>Returns the functions result.</returns>
        TResult Execute<TResult>(Func<RecombeeClient, TResult> func);
    }
}
