# xperience-module-recombee-recommendations
Recombee recommendations for Kentico Xperience

This repository contains source code for integration of Kentico Xperience and Recombee Artificial Intelligence Powered Recommender as a Service.

## Description

This project consists of two modules, Administration and live site, each for one instance. 
After installation of the Administration module, you can access the Recombee application from the application menu.
The Recombee application provides you with an interface for initialization of the Recombee database. A properly initialized database 
is necessary for the Administration module to work correctly. 

The live site module contains API to load recommended pages for a current contact.

## Requirements and prerequisites

1. *Kentico 13* installed.
1. The integration uses online marketing features (e.g. Contacts), so the *Xperience Enterprise* license is required.
1. The *Online marketing* module has to be enabled in the *Settings* application.
1. You need to have an account on Recombee.com 
   - you can create your own free account on Recombee.com or have your project registered in the Kentico organization.
   
> This module is used for demonstrating [AI product recommendation possibilities](https://xperience.io/discover/blog/2019-10/artificial-intelligence-ai-is-here-to-help-you-w). Further adjustments are possibly needed for production usage to fit particular use cases. 

## Setting up the environment
### Administration package installation 
1. Open the solution with your administration project (WebApp.sln)
1. Navigate to the *NuGet Package Manager Console*. 
1. Run *Install-Package xxx.Name.Admin.xxx*
1. Into web.config add following keys
    ```XML
    <add key="DancingGoatCore.Recombee.ContentRecommendation.DatabaseId" value="##YourDatabaseID##" />
    <add key="DancingGoatCore.Recombee.ContentRecommendation.PrivateToken" value="##YourPrivateToken##" />
    ```
1. Build the project.
1. Go to the *Settings* application and navigate to *Integration* -> *Recombee*. Fill in the *Recombee database identifier* and *Secret token* fields.
### Package for MVC site installation
1. Open the solution with your MVC Core project (eg. DancingGoatCore.sln)
1. Navigate to the *NuGet Package Manager Console*. 
1. Run *Install-Package xxx.Namexxx*
1. Into packages.config add following keys
   ```JSON
   "DancingGoatCore.Recombee.ContentRecommendation.DatabaseId" : "##YourDatabaseID##"
   "DancingGoatCore.Recombee.ContentRecommendation.PrivateToken" : "##YourPrivateToken##"
   ```
1. Build the project.

### Setup Recombee database structure
By default system automatically maps page type name and culture for individual pages into Recombee database. To ensure mapping custom field run following code on application start:
```c#
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
```
> This can be also done in custom module. See example in [Xperience.Recombee.Recommendation.Content.Admin.SampleConfiguration](Xperience.Recombee.Recommendation.Content.Admin.SampleConfiguration) folder.
### Recombee database initialization
1. Once previous steps are done open administration iterface and navigate to _Recombee content recommendation_ application.
![Module user interface](images/AdministrationInterface.png)
1. Hit initialize database button to ensure Recombee database is properly set up.
> Reset button completelly removes data and structure

## Available API 
### Get recommendations for contact
```c#
IContentService. GetPagesRecommendationForContact(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null);
```

### Get recommendations for a page (similar pages)
```c#
IContentService.GetPagesRecommendationForPage(TreeNode page, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null);
```

### How to log page views
```c#
IContentService.LogPageView(TreeNode page, Guid contactGuid);
```





![Analytics](https://kentico-ga-beacon.azurewebsites.net/api/UA-69014260-4/Kentico/xperience-module-recombee-recommendations?pixel)