﻿using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[Breadcrumbs]
[Breadcrumb(0, "documenttype_edit_transformation_edit.transformations", "DocumentType_Edit_Transformation_List.aspx?parentobjectid={?parentobjectid?}", null)]
[Breadcrumb(1, "documenttype_edit_transformation_edit.newtransformation")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_New : CMSDesignPage
{
    #region "Constants"

    private const string HELP_TOPIC_LINK = "transformations_writing";

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, control is used in site manager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return filter.IsSiteManager;
        }
        set
        {
            filter.IsSiteManager = value;
        }
    }


    public bool DialogMode
    {
        get
        {
            return QueryHelper.GetBoolean("editonlycode", false);
        }
    }


    public int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("parentobjectid", 0);
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (DialogMode)
        {
            // Check hash
            if (!ValidationHelper.ValidateHash("?editonlycode=1", QueryHelper.GetString("hash", String.Empty), new HashSettings("")))
            {
                URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
            }
            // Setup help
            PageTitle.HelpTopicName = HELP_TOPIC_LINK;
        }

        editElem.OnAfterSave += editElem_OnAfterSave;

        var transformation = editElem.EditedObject as TransformationInfo;
        if (transformation != null)
        {
            // Set parent identifier
            transformation.TransformationClassID = (ClassID > 0) ? ClassID : filter.ClassId;
        }

        ucTransfCode.ClassID = ClassID;
        if (ClassID <= 0)
        {
            // Class is not defined, display doc type filter to select under which class the transformation should be crated
            filter.SelectedValue = QueryHelper.GetString("selectedvalue", null);
            filter.FilterMode = TransformationInfo.OBJECT_TYPE;

            ucTransfCode.ClassID = filter.ClassId;

            CurrentMaster.DisplayControlsPanel = true;
        }
        else
        {
            CurrentMaster.DisplayControlsPanel = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        int classId = ClassID;
        if (classId <= 0)
        {
            classId = filter.ClassId;
        }

        var classInfo = DataClassInfoProvider.GetDataClassInfo(classId);
        if ((classInfo != null) && (classInfo.ClassIsDocumentType && classInfo.ClassIsCoupledClass))
        {
            // Generate default
            HeaderAction generate = new HeaderAction
            {
                Text = GetString("DocumentType_Edit_Transformation_Edit.ButtonDefault"),
                Tooltip = GetString("transformationtypecode.generatetooltip"),
                OnClientClick = "GenerateDefaultCode('default'); return false;"
            };

            if (CurrentMaster.ObjectEditMenu != null)
            {
                CurrentMaster.ObjectEditMenu.AddExtraAction(generate);
            }
        }

        base.OnPreRender(e);

        if (DialogMode)
        {
            PageBreadcrumbs.Items.Clear();
            PageTitle.TitleText = GetString("DocumentType_Edit_Transformation_Edit.NewTransformation");
        }
    }


    protected void editElem_OnAfterSave(object sender, EventArgs e)
    {
        var ti = editElem.EditedObject as TransformationInfo;
        if (ti != null)
        {
            // If dialog mode redirect after, selector is updated
            var url = GetEditUrl("CMS.DocumentEngine", "EditTransformation", ti);


            URLHelper.Redirect(url);
        }
    }


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    /// <param name="elementName">Element name</param>
    /// <param name="transformation">Transformation object info</param>
    private String GetEditUrl(string resourceName, string elementName, TransformationInfo transformation)
    {
        var uiChild = UIElementInfoProvider.GetUIElementInfo(resourceName, elementName);
        if (uiChild != null)
        {
            string url = String.Empty;
            string query = RequestContext.CurrentQueryString;
            // Remove parentobjectid parameter to prevent from duplicating (URL generated by UIContextHelper.GetElementUrl already contains it).
            query = URLHelper.RemoveUrlParameter(query, "parentobjectid");

            if (!DialogMode)
            {
                url = UIContextHelper.GetElementUrl(uiChild, UIContext);
                url = URLHelper.AppendQuery(url, query);
                // Remove hash parameter as it's useless in non-dialog mode.
                url = URLHelper.RemoveParameterFromUrl(url, "hash");
            }
            else
            {
                url = ApplicationUrlHelper.GetElementDialogUrl(uiChild, 0, query);
            }


            return URLHelper.AppendQuery(url, "objectid=" + transformation.TransformationID);
        }

        return String.Empty;
    }

    #endregion
}