using System;
using System.Collections.Generic;
using System.Text;

using CMS;
using CMS.DocumentEngine;

using Kentico.Xperience.Recombee;

[assembly: RegisterImplementation(typeof(IContentService), typeof(ContentService), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.Recombee
{
    /// <summary>
    /// Manages content recommendation operations in the corresponding site's Recombee database.
    /// </summary>
    public interface IContentService
    {
        /// <summary>
        /// Gets recommendation for pages for a contact.
        /// </summary>
        /// <param name="siteName">Name of site for which to retrieve the recommendation.</param>
        /// <param name="contactGuid">GUID of the contact for which to recommend.</param>
        /// <param name="count">Number of items to return.</param>
        /// <param name="cultures">Culture to be considered for recommendation.</param>
        /// <param name="pageTypes">Constrain on page types to be considered for recommendation, or null.</param>
        /// <param name="scenario">Scenario defines a particular application of recommendations. It can be for example "homepage", "related" or "emailing". You can configure each scenario in the Recombee Admin UI.</param>
        /// <returns>Returns an enumeration of identifiers of recommended pages.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Recombee client service is not configured for <paramref name="siteName"/>.</exception>
        IEnumerable<PageIdentifier> GetPagesRecommendationForContact(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null, string scenario = null);


        /// <summary>
        /// Gets recommendation for pages which are related to given <paramref name="page"/> for a contact.
        /// </summary>
        /// <param name="page">Page for which to retrieve related recommendation.</param>
        /// <param name="contactGuid">GUID of the contact for which to recommend.</param>
        /// <param name="count">Number of items to return.</param>
        /// <param name="cultures">Culture to be considered for recommendation.</param>
        /// <param name="pageTypes">Constrain on page types to be considered for recommendation, or null.</param>
        /// <param name="scenario">Scenario defines a particular application of recommendations. It can be for example "homepage", "related" or "emailing". You can configure each scenario in the Recombee Admin UI.</param>
        /// <returns>Returns an enumeration of identifiers of recommended pages.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Recombee client service is not configured for <paramref name="siteName"/>.</exception>
        IEnumerable<PageIdentifier> GetPagesRecommendationForPage(TreeNode page, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null, string scenario = null);


        /// <summary>
        /// Logs contact's view of <paramref name="page"/> .
        /// </summary>
        /// <param name="page">Page whose view to log.</param>
        /// <param name="contactGuid">GUID of the contact who viewed the page.</param>
        /// <exception cref="InvalidOperationException">Thrown when the Recombee client service is not configured for <paramref name="siteName"/>.</exception>
        void LogPageView(TreeNode page, Guid contactGuid);
    }
}
