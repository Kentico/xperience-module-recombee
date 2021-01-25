using CMS;
using CMS.Core;
using CMS.DocumentEngine;

using Kentico.Xperience.Recombee.Admin;

[assembly: AssemblyDiscoverable]

[assembly: RegisterModule(typeof(Module))]

namespace Kentico.Xperience.Recombee.Admin
{
    /// <summary>
    /// Represents the Recombee content recommendation module for the Xperience admin interface.
    /// </summary>
    public class Module : CMS.DataEngine.Module
    {
        public const string NAME = "Kentico.Xperience.Recombee.Admin";

        private IContentService mContentService;


        private IContentService ContentService
        {
            get
            {
                return mContentService ?? (mContentService = Service.Resolve<IContentService>());
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        public Module() : base(NAME)
        {
        }


        protected override void OnInit()
        {
            base.OnInit();

            DocumentEvents.Insert.After += PageCreated;
            DocumentEvents.Update.After += PageUpdated;
            DocumentEvents.Delete.After += PageDeleted;
        }


        private void PageCreated(object sender, DocumentEventArgs e)
        {
            ContentService.PageCreated(e.Node);
        }


        private void PageUpdated(object sender, DocumentEventArgs e)
        {
            ContentService.PageUpdated(e.Node);
        }


        private void PageDeleted(object sender, DocumentEventArgs e)
        {
            ContentService.PageDeleted(e.Node);
        }
    }
}
