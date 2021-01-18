using System.Collections.Generic;

using CMS;
using CMS.Core;

using Xperience.Recombee.Recommendation.Content.Admin.SampleConfiguration;

[assembly: RegisterModule(typeof(SampleModule))]

namespace Xperience.Recombee.Recommendation.Content.Admin.SampleConfiguration
{
    /// <summary>
    /// Represents a sample that shows how to configure page type mappings for DancingGoatCore site for the Recombee content recommendation module for the Xperience admin interface.
    /// </summary>
    public class SampleModule : CMS.DataEngine.Module
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleModule"/> class.
        /// </summary>
        public SampleModule() : base("Xperience.Recombee.Recommendation.Content.Admin.SampleConfiguration")
        {
        }


        protected override void OnInit()
        {
            base.OnInit();

            ConfigurePageTypes();
        }


        private void ConfigurePageTypes()
        {
            const string SITE_NAME = "DancingGoatCore";

            var itemProperties = Service.Resolve<IDatabaseConfiguration>().Get(SITE_NAME);

            itemProperties.Add(new DatabaseProperty("Title", "string"));
            itemProperties.Add(new DatabaseProperty("Summary", "string"));
            itemProperties.Add(new DatabaseProperty("Text", "string"));


            var fieldMapper = Service.Resolve<IFieldMapper>();
            var configurations = fieldMapper.GetConfigurations(SITE_NAME);
            configurations.IncludedCultures.Add("en-us");
            configurations.Mappings.Add("cms.document.DancingGoatCore.Article", new List<FieldMapping>
            {
                new FieldMapping("ArticleTitle", "Title"),
                new FieldMapping("ArticleSummary", "Summary"),
                
                // Example illustrating how advanced field mapping can be achieved. This way even page tags, categories or images (URLs) can be mapped
                new FieldMapping(article => article.GetValue("ArticleText"), "Text")
            });
        }
    }
}
