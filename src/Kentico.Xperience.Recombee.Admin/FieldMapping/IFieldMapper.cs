using System;
using System.Collections.Generic;

using CMS;
using CMS.DocumentEngine;

using Kentico.Xperience.Recombee.Admin;

[assembly: RegisterImplementation(typeof(IFieldMapper), typeof(FieldMapper), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.Recombee.Admin
{
    /// <summary>
    /// Performs mapping of page type fields to Recombee database item properties.
    /// </summary>
    public interface IFieldMapper
    {
        /// <summary>
        /// Gets the <see cref="PageTypeMappings"/> for a site.
        /// </summary>
        /// <param name="siteName">Name of site for which to retrieve the configuration.</param>
        /// <returns>Returns the site's configurations of page type mappings.</returns>
        PageTypeMappings GetConfigurations(string siteName);


        /// <summary>
        /// Maps a page to Recombee database item.
        /// </summary>
        /// <param name="page">Page to be mapped.</param>
        /// <returns>Returns the mapped Recombee database item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the page's site does not contain the configuration for page's type.</exception>
        Dictionary<string, object> Map(TreeNode page);
    }
}
