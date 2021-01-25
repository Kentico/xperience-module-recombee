using System;

using CMS.DocumentEngine;

namespace Kentico.Xperience.Recombee.Admin
{
    /// <summary>
    /// Represents a page type field to Recombee database item property mapping.
    /// </summary>
    /// <seealso cref="FieldMapper"/>
    public class FieldMapping
    {
        /// <summary>
        /// Gets the name of the source field within a <see cref="TreeNode"/>.
        /// </summary>
        public string SourceName { get; }


        /// <summary>
        /// Gets the function extracting source value from a <see cref="TreeNode"/>.
        /// </summary>
        public Func<TreeNode, object> SourceValue { get; }


        /// <summary>
        /// Gets the name of the target item property in the Recombee DB.
        /// </summary>
        public string TargetName { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMapping"/> class using the name of the page's source field.
        /// </summary>
        /// <param name="sourceName">Name of the page's source field.</param>
        /// <param name="targetName">Name of the target property in the Recombee DB.</param>
        public FieldMapping(string sourceName, string targetName)
        {
            if (string.IsNullOrEmpty(sourceName))
            {
                throw new ArgumentException($"'{nameof(sourceName)}' cannot be null or empty", nameof(sourceName));
            }

            if (string.IsNullOrEmpty(targetName))
            {
                throw new ArgumentException($"'{nameof(targetName)}' cannot be null or empty", nameof(targetName));
            }

            SourceName = sourceName;
            TargetName = targetName;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMapping"/> class using the page's source value provider.
        /// </summary>
        /// <param name="sourceValue">Function accepting a <see cref="TreeNode"/> instance and providing the source value.</param>
        /// <param name="targetName">Name of the target property in the Recombee DB.</param>
        public FieldMapping(Func<TreeNode, object> sourceValue, string targetName)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                throw new ArgumentException($"'{nameof(targetName)}' cannot be null or empty", nameof(targetName));
            }

            SourceValue = sourceValue ?? throw new ArgumentNullException(nameof(sourceValue));
            TargetName = targetName;
        }
    }
}
