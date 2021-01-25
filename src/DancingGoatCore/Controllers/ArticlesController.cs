using System;
using System.Linq;

using CMS.ContactManagement;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatCore;

using DancingGoat.Controllers;
using DancingGoat.Models;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using Microsoft.AspNetCore.Mvc;

using Kentico.Xperience.Recombee;

[assembly: RegisterPageRoute(ArticleSection.CLASS_NAME, typeof(ArticlesController))]
[assembly: RegisterPageRoute(Article.CLASS_NAME, typeof(ArticlesController), ActionName = "Detail")]

namespace DancingGoat.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ArticleRepository articleRepository;
        private readonly IContentService contentService;
        private readonly IEventLogService eventLogService;

        public ArticlesController(ArticleRepository articleRepository, IContentService contentService, IEventLogService eventLogService)
        {
            this.articleRepository = articleRepository;
            this.contentService = contentService;
            this.eventLogService = eventLogService;
        }


        public IActionResult Index([FromServices] IPageDataContextRetriever dataContextRetriever,
                                   [FromServices] IPageUrlRetriever pageUrlRetriever,
                                   [FromServices] IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            var section = dataContextRetriever.Retrieve<TreeNode>().Page;
            var articles = articleRepository.GetArticles(section.NodeAliasPath);
                
            return View(articles.Select(article => ArticleViewModel.GetViewModel(article, pageUrlRetriever, attachmentUrlRetriever)));
        }


        public IActionResult Detail([FromServices] ArticleRepository articleRepository)
        {
            var article = articleRepository.GetCurrent();

            var contactGuid = ContactManagementContext.GetCurrentContact()?.ContactGUID ?? Guid.Empty;

            try
            {
                contentService.LogPageView(article, contactGuid);
            }
            catch (Exception ex)
            {
                eventLogService.LogWarning("Recombee", "LOGPAGEVIEW", ex);
            }

            return new TemplateResult(article);
        }
    }
}