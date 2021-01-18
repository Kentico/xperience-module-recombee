using System;

using CMS.DocumentEngine;

namespace Xperience.Recombee.Recommendation.Content
{
    /// <summary>
    /// Identifier of a <see cref="TreeNode"/> used in the Recombee database.
    /// </summary>
    public class PageIdentifier
    {
        /// <summary>
        /// Gets the page's <see cref="TreeNode.NodeGUID"/>.
        /// </summary>
        public Guid NodeGuid { get; }


        /// <summary>
        /// Gets the page's <see cref="TreeNode.DocumentGUID"/>.
        /// </summary>
        public Guid DocumentGuid { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="PageIdentifier"/> class using the specified node and document GUIDs.
        /// </summary>
        /// <param name="nodeGuid">Node's GUID (<see cref="TreeNode.NodeGUID"/>).</param>
        /// <param name="documentGuid">Document's GUID (<see cref="TreeNode.DocumentGUID"/>).</param>
        public PageIdentifier(Guid nodeGuid, Guid documentGuid)
        {
            NodeGuid = nodeGuid;
            DocumentGuid = documentGuid;
        }
    }
}
