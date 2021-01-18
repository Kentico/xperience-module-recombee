using CMS;

using Xperience.Recombee.Recommendation.Content;

[assembly: AssemblyDiscoverable]

[assembly: RegisterModule(typeof(Module))]

namespace Xperience.Recombee.Recommendation.Content
{
    /// <summary>
    /// Represents the Recombee content recommendation module.
    /// </summary>
    public class Module : CMS.DataEngine.Module
    {
        public const string NAME = "Xperience.Recombee.Recommendation.Content";
    
    
        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        public Module() : base(NAME)
        {
        }
    }
}
