# The Kentico Xperience Recombee module
[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white)](https://stackoverflow.com/tags/kentico)

|  | Package |
| ------------- |:-------------:|
| Admin | [![NuGet](https://img.shields.io/nuget/v/Kentico.Xperience.Recombee.Admin.KX13.svg)](https://www.nuget.org/packages/Kentico.Xperience.Recombee.Admin.KX13/0.0.1-preview) |
| MVC client | [![NuGet](https://img.shields.io/nuget/v/Kentico.Xperience.Recombee.KX13.svg)](https://www.nuget.org/packages/Kentico.Xperience.Recombee.KX13/0.0.1-preview) |

[Recombee Recommender](https://www.recombee.com/) for [Kentico Xperience](https://xperience.io/)

This repository contains source code for integration of Kentico Xperience with Recombee Artificial Intelligence Powered Recommender as a Service.

## Description

This project consists of two modules:
* Administration module allows you to access the *Recombee content recommendation* application from the application menu, where you can initialize the Recombee database. A properly initialized database is necessary for the Recombee integration to work correctly.
* Live site module contains API to track contect activities and load recommended pages for the current contact or based on the content of current page.

## Requirements and prerequisites

1. *Kentico Xperience 13* installed.
1. *Xperience Enterprise* license is required, as the integration uses online marketing features (e.g. Contacts).
1. The *Online marketing* module needs to be enabled in the *Settings* application.
1. You need to have an account on Recombee.com
   - you can create your own free account on Recombee.com or have your project registered in the Kentico organization.

> This module is used for demonstrating [AI recommendation possibilities](https://xperience.io/discover/blog/2019-10/artificial-intelligence-ai-is-here-to-help-you-w). Further adjustments are needed for production usage to fit particular use cases.

## Setting up the environment
### Administration package installation
1. Open the solution with your administration project (*~/WebApp.sln*).
1. Navigate to the *NuGet Package Manager Console*.
1. Run *Install-Package Kentico.Xperience.Recombee.Admin.KX13 -Version 0.0.1-preview*
1. Add the following keys into the *web.config* file of the administration project:
    ```XML
    <add key="DancingGoatCore.Recombee.ContentRecommendation.DatabaseId" value="##YourDatabaseID##" />
    <add key="DancingGoatCore.Recombee.ContentRecommendation.PrivateToken" value="##YourPrivateToken##" />
    ```
    > Replace *DancingGoatCore* with the code name of your site.
1. Build the project.
1. Go to the *Settings* application and navigate to *Integration* -> *Recombee*. Fill in the *Recombee database identifier* and *Secret token* fields.

### Live site package installation
1. Open the solution with your MVC Core project (e.g. *~/DancingGoatCore.sln*).
1. Navigate to the *NuGet Package Manager Console*.
1. Run *Install-Package Kentico.Xperience.Recombee.KX13 -Version 0.0.1-preview*.
1. Add the following keys into the *appsettings.json* file of the live site project:
   ```JSON
   "DancingGoatCore.Recombee.ContentRecommendation.DatabaseId" : "##YourDatabaseID##",
   "DancingGoatCore.Recombee.ContentRecommendation.PrivateToken" : "##YourPrivateToken##"
   ```
   > Replace *DancingGoatCore* with the code name of your site.

1. Build the project.

### Setup Recombee database structure
By default, the system automatically maps page type name and culture for individual pages into Recombee database. To ensure mapping of fields from your page types to fields within the Recombee database, run following code on application start:
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
> This can be also done in custom module. See example in [Kentico.Xperience.Recombee.Admin.SampleConfiguration](src/Kentico.Xperience.Recombee.Admin.SampleConfiguration) folder.

### Recombee database initialization

1. Once previous steps are done, open the administration iterface and navigate to the *Recombee content recommendation* application.
![Module user interface](Images/AdministrationInterface.png)
1. Click the *Initialize database* button to set up the Recombee database.
> Reset button completelly removes data and database structure, use it only when you want to remove Recombee from the system or completelly reset the Recombee integration.

## Available API

To work with the data from Recombee on the live site, you can use one of two approaches:
* Log page visits by contacts and then recommend pages to contacts according to their previous activity.
* Recommend pages to visitors based on the content of the page they are currently viewing.

### Recommending pages based on contact activity
Firstly, you need to log page views of your contacts:
```c#
IContentService.LogPageView(TreeNode page, Guid contactGuid);
```
Afterwards, you can get a list of recommended pages based on the contact's previous page views:
```c#
IContentService.GetPagesRecommendationForContact(string siteName, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null);
```
### Recommending pages based on the current page
You can retrieve a list of pages that are chosen based on the content of the current page:
```c#
IContentService.GetPagesRecommendationForPage(TreeNode page, Guid contactGuid, int count, string culture, IEnumerable<string> pageTypes = null);
```

![Analytics](https://kentico-ga-beacon.azurewebsites.net/api/UA-69014260-4/Kentico/xperience-module-recombee-recommendations?pixel)
