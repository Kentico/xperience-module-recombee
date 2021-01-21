﻿using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatCore;
using CMS.Ecommerce;
using CMS.Search;

using DancingGoat.Models;
using DancingGoat.Services;

using Kentico.Content.Web.Mvc;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Provides methods for conversion from <see cref="SearchResultItem"/> to particular <see cref="SearchResultItemModel"/>.
    /// </summary>
    public class TypedSearchItemViewModelFactory
    {
        private readonly ICalculationService calculationService;
        private readonly IPageUrlRetriever pageUrlRetriever;

        private readonly HomeRepository homeRepository;
        private readonly AboutUsRepository aboutUsRepository;

        /// <summary>
        /// Creates a new instance of <see cref="TypedSearchItemViewModelFactory"/> class.
        /// </summary>
        /// <param name="calculationService">Service for price calculations.</param>
        /// <param name="pageUrlRetriever">Retriever for page URLs.</param>
        /// <param name="homeRepository">Repository representing home page sections.</param>
        /// <param name="aboutUsRepository">Repository representing about us stories.</param>
        public TypedSearchItemViewModelFactory(ICalculationService calculationService, IPageUrlRetriever pageUrlRetriever, HomeRepository homeRepository, AboutUsRepository aboutUsRepository)
        {
            this.calculationService = calculationService;
            this.pageUrlRetriever = pageUrlRetriever;
            this.homeRepository = homeRepository;
            this.aboutUsRepository = aboutUsRepository;
        }


        /// <summary>
        /// Creates a search view model according to the runtime type of <paramref name="searchResultItem"/>.
        /// </summary>
        public SearchResultItemModel GetTypedSearchResultItemModel(SearchResultItem searchResultItem)
        {
            var data = (dynamic)searchResultItem.Data;
            return GetViewModelForSearchItem(searchResultItem, data);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, TreeNode page)
        {
            return new SearchResultPageItemModel(resultItem, page, pageUrlRetriever);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, SKUTreeNode skuTreeNode)
        {
            var price = calculationService.CalculatePrice(skuTreeNode.SKU);
            return new SearchResultProductItemModel(resultItem, skuTreeNode, price, pageUrlRetriever);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, AboutUs aboutUs)
        {
            return new SearchResultAboutUsItemModel(resultItem, aboutUs, aboutUsRepository, pageUrlRetriever);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, Home home)
        {
            return new SearchResultHomeItemModel(resultItem, home, homeRepository, pageUrlRetriever);
        }
    }
}