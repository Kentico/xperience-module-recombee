using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;
using CMS.UIControls;

using Xperience.Recombee.Recommendation.Content.Admin;

[UIElement("Xperience.Recombee.Content.Recommendation.Admin", "Xperience.Recombee.Content.Recommendation")]
public partial class CMSModules_Recombee_setup : CMSPage
{
    private readonly ISettingsService settingsService;
    private readonly ISiteService siteService;
    private readonly IContentService contentService;
    private readonly IClientServiceProvider clientServiceProvider;
    private readonly IFieldMapper fieldMapper;

    public CMSModules_Recombee_setup()
    {
        settingsService = Service.Resolve<ISettingsService>();
        siteService = Service.Resolve<ISiteService>();
        contentService = Service.Resolve<IContentService>();
        clientServiceProvider = Service.Resolve<IClientServiceProvider>();
        fieldMapper = Service.Resolve<IFieldMapper>();
    }

    public void Page_Load()
    {
        if(!IsOnlineMarketingEnabled())
        {
            btnIntDbStructure.Enabled = false;
            btnResetDatabase.Enabled = false;
            pageTypesWrapper.Visible = false;

            ShowInformation("On-line marketing must be enabled to use the Recombee content recommendation.");
        }
        else if(!clientServiceProvider.IsAvailable(CurrentSiteName))
        {
            btnIntDbStructure.Enabled = false;
            btnResetDatabase.Enabled = false;
            pageTypesWrapper.Visible = false;

            ShowInformation("Recombee content recommendation is not available. Specify the Recombee Database ID and Private Token in the application configuration file.");
        }
        else if(!UserInfoProvider.IsAuthorizedPerResource(Xperience.Recombee.Recommendation.Content.Admin.Module.NAME, "Modify", CurrentSiteName, CurrentUser))
        {
            btnIntDbStructure.Enabled = false;
            btnResetDatabase.Enabled = false;

            ShowInformation("You are not authorized to modify Recombee database structure on this site.");
        }

        pageTypes.InnerText = String.Join(", ", fieldMapper.GetConfigurations(CurrentSiteName).Mappings.Keys);
    }


    protected void ResetDatabase_Click(object sender, EventArgs e)
    {
        try
        {
            contentService.Reset(CurrentSiteName);

            ShowConfirmation("Database reset.");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
            Service.Resolve<IEventLogService>().LogException("Recombee setup", "Reset", ex);
        }
    }

    protected void InitDatabase_Click(object sender, EventArgs e)
    {
        try
        {
            contentService.SendAll(CurrentSiteName);

            ShowConfirmation("Database initialized and populated with current pages.");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
            Service.Resolve<IEventLogService>().LogException("Recombee setup", "Init", ex);
        }
    }

    private bool IsOnlineMarketingEnabled()
    {
        return settingsService[siteService.CurrentSite?.SiteName + ".CMSEnableOnlineMarketing"].ToBoolean(false);
    }
}