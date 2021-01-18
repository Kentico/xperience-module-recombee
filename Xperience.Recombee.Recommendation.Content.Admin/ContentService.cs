using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Core;
using CMS.DocumentEngine;

using Recombee.ApiClient;
using Recombee.ApiClient.ApiRequests;
using Recombee.ApiClient.Bindings;

namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Manages pages in the corresponding site's Recombee database.
    /// </summary>
    public class ContentService : IContentService
    {
        private readonly IClientServiceProvider clientServiceProvider;
        private readonly IFieldMapper fieldMapper;
        private readonly IEventLogService eventLogService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ContentService"/> class.
        /// </summary>
        /// <param name="clientServiceProvider">Provider of <see cref="IClientService"/>s.</param>
        /// <param name="fieldMapper">Mapper for page fields.</param>
        /// <param name="eventLogService">Event log service.</param>
        public ContentService(IClientServiceProvider clientServiceProvider, IFieldMapper fieldMapper, IEventLogService eventLogService)
        {
            this.clientServiceProvider = clientServiceProvider ?? throw new ArgumentNullException(nameof(clientServiceProvider));
            this.fieldMapper = fieldMapper ?? throw new ArgumentNullException(nameof(fieldMapper));
            this.eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
        }


        /// <summary>
        /// Gets a value indicating whether <paramref name="page"/> is configured to be mapped and sent to the Recombee database.
        /// </summary>
        /// <param name="page">Page to be tested.</param>
        /// <returns>Returns true if the <paramref name="page"/> is configured for the Recombee database.</returns>
        /// <remarks>
        /// Processed pages are those whose culture matches the <see cref="IFieldMapper"/>'s <see cref="PageTypeMappings.IncludedCultures"/> set and whose site contains a corresponding page type mapping in <see cref="PageTypeMappings.Mappings"/>.
        /// </remarks>
        public bool IsProcessed(TreeNode page)
        {
            var configuration = fieldMapper.GetConfigurations(page.NodeSiteName);

            return configuration.IncludedCultures.Contains(page.DocumentCulture) && fieldMapper.GetConfigurations(page.NodeSiteName).Mappings.ContainsKey(page.TypeInfo.ObjectType);
        }


        /// <summary>
        /// Handles the event of page creation. If the <paramref name="page"/> matches the <see cref="IsProcessed(TreeNode)"/> and <see cref="IsPublished(TreeNode)"/>
        /// predicates, it is sent to the Recombee DB.
        /// </summary>
        /// <param name="page">Page which was created.</param>
        /// <seealso cref="DocumentEvents.Insert"/>
        public virtual void PageCreated(TreeNode page)
        {
            if (!IsProcessed(page) || !IsPublished(page))
            {
                return;
            }

            Send(page);
        }


        /// <summary>
        /// Handles the event of page update. If the <paramref name="page"/> matches the <see cref="IsProcessed(TreeNode)"/> predicate,
        /// it is either sent to the Recombee DB (if <see cref="IsPublished(TreeNode)"/> is true), or deleted from the Recombee DB.
        /// </summary>
        /// <param name="page">Page which was updated.</param>
        /// <seealso cref="DocumentEvents.Update"/>
        public virtual void PageUpdated(TreeNode page)
        {
            if (!IsProcessed(page))
            {
                return;
            }

            if (!IsPublished(page))
            {
                Delete(page);
            }
            else
            {
                Send(page);
            }
        }


        /// <summary>
        /// Handles the event of page delete. If the <paramref name="page"/> matches the <see cref="IsProcessed(TreeNode)"/> predicate,
        /// it is deleted from the Recombee DB.
        /// </summary>
        /// <param name="page">Page to be deleted.</param>
        /// <seealso cref="DocumentEvents.Delete"/>
        public virtual void PageDeleted(TreeNode page)
        {
            if (!IsProcessed(page))
            {
                return;
            }

            Delete(page);
        }


        /// <summary>
        /// Deletes <paramref name="page"/> from the Recombee database.
        /// The operation is void if the Recombee service is not configured for the page's site.
        /// </summary>
        /// <param name="page">Page to be deleted from the Recombee database.</param>
        public void Delete(TreeNode page)
        {
            var siteName = page.NodeSiteName;
            if (!clientServiceProvider.IsAvailable(siteName))
            {
                LogNoClientWarning(siteName);
                return;
            }

            var deleteItem = new DeleteItem(GetItemId(page));

            clientServiceProvider.Get(siteName).Send(deleteItem);
        }


        /// <summary>
        /// Sends all pages of configured page types to the Recombee database.
        /// The operation is void if the Recombee service is not configured for the page's site.
        /// </summary>
        /// <param name="siteName">Name of site whose pages are to be sent.</param>
        /// <remarks>
        /// <para>
        /// Only published versions of pages in included cultures are sent to the Recombee database.
        /// </para>
        /// <para>
        /// When overriding, the method's implementation must be consistent with the <see cref="IsProcessed(TreeNode)"/> and <see cref="IsPublished(TreeNode)"/> methods.
        /// </para>
        /// </remarks>
        /// <seealso cref="IsPublished(TreeNode)"/>
        /// <seealso cref="IsProcessed(TreeNode)"/>
        /// <seealso cref="PageTypeMappings.IncludedCultures"/>
        public void SendAll(string siteName)
        {
            if (!clientServiceProvider.IsAvailable(siteName))
            {
                LogNoClientWarning(siteName);
                return;
            }

            var clientService = clientServiceProvider.Get(siteName);

            var pageTypesMappings = fieldMapper.GetConfigurations(siteName);
            foreach(var pageType in pageTypesMappings.Mappings.Keys)
            {
                SendPageType(clientService, pageType, siteName, pageTypesMappings.IncludedCultures);
            }
        }


        /// <summary>
        /// Sends all pages of <paramref name="pageType"/> on the specified site to the Recombee database.
        /// </summary>
        /// <param name="clientService">Client service to be used.</param>
        /// <param name="pageType">Page type of pages to be sent to the database.</param>
        /// <param name="siteName">Name of site whose pages are to be sent.</param>
        /// <param name="includedCultures">Culture of pages to be sent. All cultures are sent if the set is empty.</param>
        /// <remarks>
        /// <para>
        /// Only published versions of pages in included cultures are sent to the Recombee database.
        /// </para>
        /// <para>
        /// When overriding, the method's implementation must be consistent with the <see cref="IsProcessed(TreeNode)"/> and <see cref="IsPublished(TreeNode)"/> methods.
        /// </para>
        /// </remarks>
        protected virtual void SendPageType(IClientService clientService, string pageType, string siteName, ISet<string> includedCultures)
        {
            var pages = GetPages(pageType, siteName, includedCultures);

            var itemValues = pages.Select(p => new SetItemValues(GetItemId(p), fieldMapper.Map(p), true));

            var response = clientService.Send(new Batch(itemValues));

            ValidateBatchResponse(response);
        }


        /// <summary>
        /// Gets all pages of <paramref name="pageType"/>  on the specified site.
        /// </summary>
        /// <param name="pageType">Page type of pages to be retrieved.</param>
        /// <param name="siteName">Name of site whose pages are to be retrieved.</param>
        /// <param name="includedCultures">Culture of pages to be retrieved. All cultures are retrieved if the set is empty.</param>
        /// <returns>Returns an enumeration of pages.</returns>
        /// <remarks>
        /// <para>
        /// Only published versions of pages in included cultures are sent to the Recombee database.
        /// </para>
        /// <para>
        /// When overriding, the method's implementation must be consistent with the <see cref="IsProcessed(TreeNode)"/> and <see cref="IsPublished(TreeNode)"/> methods.
        /// </para>
        /// </remarks>
        protected virtual IEnumerable<TreeNode> GetPages(string pageType, string siteName, ISet<string> includedCultures)
        {
            var query = DocumentHelper.GetDocuments(pageType).OnSite(siteName).PublishedVersion();

            if (includedCultures.Count == 0)
            {
                return query.AllCultures();
            }

            return includedCultures.SelectMany(culture => query.Clone().CombineWithDefaultCulture(false).Culture(culture));
        }


        /// <summary>
        /// Validates batch <paramref name="response"/> and throws an <see cref="AggregateException"/>
        /// if any of the requests within batch did not succeed (did not return 200 or 201).
        /// </summary>
        /// <param name="response">Batch response to be validated.</param>
        protected virtual void ValidateBatchResponse(BatchResponse response)
        {
            var responsesWithExceptions = response.Responses.Where(r => r is ResponseException).Select(r => r as ResponseException).ToList();

            if (responsesWithExceptions.Count > 0)
            {
                throw new AggregateException(responsesWithExceptions);
            }
        }


        /// <summary>
        /// Gets a value indicating whether <paramref name="page"/> in its current state is to be published to the Recombee DB.
        /// Non-published pages are deleted from the Recombee DB.
        /// </summary>
        /// <param name="page">Page for which to return the publishing status.</param>
        /// <returns>Returns true if the page should be sent to the Recombee DB. Otherwise returns false.</returns>
        /// <remarks>
        /// The implementation decides the publishing state based on the <see cref="TreeNode.IsPublished"/> property.
        /// </remarks>
        protected virtual bool IsPublished(TreeNode page)
        {
            return page.IsPublished;
        }


        /// <summary>
        /// Sends <paramref name="page"/> to the Recombee database.
        /// The operation is void if the Recombee service is not configured for the page's site.
        /// </summary>
        /// <param name="page">Page to be sent to the Recombee database.</param>
        public void Send(TreeNode page)
        {
            var siteName = page.NodeSiteName;
            if (!clientServiceProvider.IsAvailable(siteName))
            {
                LogNoClientWarning(siteName);
                return;
            }

            var itemValues = fieldMapper.Map(page);
            var setItemValues = new SetItemValues(GetItemId(page), itemValues, true);

            clientServiceProvider.Get(siteName).Send(setItemValues);
        }


        /// <summary>
        /// Resets the Recombee database content and structure.
        /// The operation is void if the Recombee service is not configured for the page's site.
        /// </summary>
        /// <param name="siteName">Name of site for which to performs the database reset.</param>
        public void Reset(string siteName)
        {
            if (!clientServiceProvider.IsAvailable(siteName))
            {
                LogNoClientWarning(siteName);
                return;
            }

            clientServiceProvider.Get(siteName).Reset();
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


        private void LogNoClientWarning(string siteName)
        {
            eventLogService.LogWarning("Recombee", "NOCLIENT", $"Recombee client service is not available for site '{siteName}'. The operation was cancelled.");
        }
    }
}
