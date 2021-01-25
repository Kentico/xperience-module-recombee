using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine;

using Recombee.ApiClient.ApiRequests;

namespace Kentico.Xperience.Recombee
{
    /// <summary>
    /// Manages content recommendation operations in the corresponding site's Recombee database.
    /// </summary>
    public class ContentService : IContentService
    {
        public const string PAGE_TYPE_PROPERTY_NAME = "_PageType";
        public const string CULTURE_PROPERTY_NAME = "_Culture";


        private readonly IClientServiceProvider clientServiceProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="ContentService"/> class.
        /// </summary>
        /// <param name="clientServiceProvider">Provider of <see cref="IClientService"/>s.</param>
        public ContentService(IClientServiceProvider clientServiceProvider)
        {
            this.clientServiceProvider = clientServiceProvider ?? throw new ArgumentNullException(nameof(clientServiceProvider));
        }


        /// <summary>
        /// Gets recommendation for pages that are suitable for the contact.
        /// </summary>
        /// <param name="siteName">Name of site for which to retrieve the recommendation.</param>
        /// <param name="contactGuid">GUID of the contact for which to recommend.</param>
        /// <param name="count">Number of items to return.</param>
        /// <param name="culture">Culture to be considered for recommendation.</param>
        /// <param name="pageTypes">Constrain on page types to be considered for recommendation, or null.</param>
        /// <returns>Returns an enumeration of identifiers of recommended pages.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Recombee client service is not configured for <paramref name="siteName"/>.</exception>
        public IEnumerable<PageIdentifier> GetPagesRecommendationForContact(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null)
        {
            var client = GetClientServiceOrThrow(siteName);
            var pageTypeFilter = (pageTypes != null) ? GetPageTypeFilter(pageTypes) : null;
            var cultureFilter = (culture != null) ? GetCultureFilter(culture) : null;
            var filter = ConcatFilterExpressions(pageTypeFilter, cultureFilter);

            var recommendation = client.Execute(rc => rc.Send(new RecommendItemsToUser(contactGuid.ToString(), count, filter: filter, cascadeCreate: true)));

            return recommendation.Recomms.Select(r => ParseItemId(r.Id));
        }


        /// <summary>
        /// Gets recommendation for pages which are related to given <paramref name="page"/> and are suitable for the contact.
        /// </summary>
        /// <param name="page">Page for which to retrieve related recommendation.</param>
        /// <param name="contactGuid">GUID of the contact for which to recommend.</param>
        /// <param name="count">Number of items to return.</param>
        /// <param name="cultures">Culture to be considered for recommendation.</param>
        /// <param name="pageTypes">Constrain on page types to be considered for recommendation, or null.</param>
        /// <returns>Returns an enumeration of identifiers of recommended pages.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Recombee client service is not configured for <paramref name="siteName"/>.</exception>
        public IEnumerable<PageIdentifier> GetPagesRecommendationForPage(TreeNode page, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null)
        {
            var client = GetClientServiceOrThrow(page.NodeSiteName);
            var pageTypeFilter = (pageTypes != null) ? GetPageTypeFilter(pageTypes) : null;
            var cultureFilter = (culture != null) ? GetCultureFilter(culture) : null;
            var filter = ConcatFilterExpressions(pageTypeFilter, cultureFilter);

            var recommendation = client.Execute(rc => rc.Send(new RecommendItemsToItem(GetItemId(page), contactGuid.ToString(), count, filter: filter, cascadeCreate: true)));

            return recommendation.Recomms.Select(r => ParseItemId(r.Id));
        }


        /// <summary>
        /// Logs contact's view of <paramref name="page"/> .
        /// </summary>
        /// <param name="page">Page whose view to log.</param>
        /// <param name="contactGuid">GUID of the contact who viewed the page.</param>
        /// <exception cref="InvalidOperationException">Thrown when the Recombee client service is not configured for <paramref name="siteName"/>.</exception>
        public void LogPageView(TreeNode page, Guid contactGuid)
        {
            var client = GetClientServiceOrThrow(page.NodeSiteName);

            client.Send(new AddDetailView(contactGuid.ToString(), GetItemId(page), cascadeCreate: true));
        }


        /// <summary>
        /// Gets filter based on <paramref name="pageTypes"/> for the <see cref="RecommendItemsToUser"/> query.
        /// </summary>
        /// <param name="pageTypes">Page types to filter.</param>
        /// <returns>Returns the filter expression for <paramref name="pageTypes"/>, or null if an empty enumeration of page types is provided.</returns>
        protected virtual string GetPageTypeFilter(IEnumerable<string> pageTypes)
        {
            return GetOrFilterExpression(PAGE_TYPE_PROPERTY_NAME, pageTypes.Select(pt => pt.ToLowerInvariant()));
        }


        /// <summary>
        /// Gets filter based on <paramref name="culture"/> for the <see cref="RecommendItemsToItem"/> query.
        /// </summary>
        /// <param name="cultures">Culture to filter.</param>
        /// <returns>Returns the filter expression for <paramref name="culture"/>.</returns>
        protected virtual string GetCultureFilter(string culture)
        {
            return GetFilterExpression(CULTURE_PROPERTY_NAME, culture.ToLowerInvariant());
        }


        /// <summary>
        /// Gets filter expression in the format <c>('propertyName' == "propertyValue1") or ('propertyName' == "propertyValue2") or ...</c>
        /// </summary>
        /// <param name="propertyName">Recombee DB property name.</param>
        /// <param name="propertyValues">Recombee DB property values.</param>
        /// <returns>Returns the resulting filter expression, or null if an empty enumeration of property values is provided.</returns>
        protected virtual string GetOrFilterExpression(string propertyName, IEnumerable<string> propertyValues)
        {
            var pageTypesFilter = propertyValues.Select(pv => GetFilterExpression(propertyName, pv));

            var result = String.Join(" or ", pageTypesFilter);

            return result.Length > 0 ? result : null;
        }


        /// <summary>
        /// Gets filter expression in the format <c>('propertyName' == "propertyValue")</c>.
        /// </summary>
        /// <param name="propertyName">Recombee DB property name.</param>
        /// <param name="propertyValue">Recombee DB property value.</param>
        /// <returns>Returns the resulting filter expression.</returns>
        protected virtual string GetFilterExpression(string propertyName, string propertyValue)
        {
            return $"('{propertyName}' == \"{propertyValue.Replace("\"", "\\\"")}\")";
        }


        /// <summary>
        /// Gets filter expression in the format <c>(firstExpression) and (otherExpression)</c>.
        /// If one of the expressions is null, returns the remaining expression. Returns null when both expressions are null.
        /// </summary>
        /// <param name="firstExpression">First expression to concatenate.</param>
        /// <param name="otherExpression">The other expression to concatenate.</param>
        /// <returns>Returns the resulting expression.</returns>
        protected virtual string ConcatFilterExpressions(string firstExpression, string otherExpression)
        {
            if(firstExpression == null)
            {
                return otherExpression;
            }
            if(otherExpression == null)
            {
                return firstExpression;
            }
            return $"({firstExpression}) and ({otherExpression})";
        }


        /// <summary>
        /// Gets Recombee database item identifier for <paramref name="page"/>.
        /// </summary>
        /// <param name="page">Page for which to return an identifier.</param>
        /// <returns>Returns the page's identifier.</returns>
        /// <remarks>
        /// The method returns identifier in format '&lt;NodeGUID&gt;:&lt;DocumentGUID&gt;'.
        /// </remarks>
        protected virtual string GetItemId(TreeNode page)
        {
            return $"{page.NodeGUID}:{page.DocumentGUID}";
        }


        /// <summary>
        /// Parses Recombee database item identifier of a page.
        /// </summary>
        /// <param name="id">Identifier to be parsed.</param>
        /// <returns>Returns the page's identifier.</returns>
        /// <remarks>
        /// The method parses identifier in format '&lt;NodeGUID&gt;:&lt;DocumentGUID&gt;'.
        /// </remarks>
        protected virtual PageIdentifier ParseItemId(string id)
        {
            var guids = id.Split(':');

            return new PageIdentifier(Guid.Parse(guids[0]), Guid.Parse(guids[1]));
        }


        private IClientService GetClientServiceOrThrow(string siteName)
        {
            return clientServiceProvider.Get(siteName) ?? throw new InvalidOperationException($"The Recombee client service is not available on site '{siteName}'. Specify the Recombee Database ID and Private Token in the application configuration file.");
        }
    }
}
