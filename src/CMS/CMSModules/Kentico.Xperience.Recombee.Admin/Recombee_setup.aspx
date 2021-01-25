<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Recombee_setup"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false"  Codebehind="Recombee_setup.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <style>
      .recombee .wrapper{
          margin-bottom:20px;
      }  

      .recombee .wrapper.history {
          margin-top:50px;
      }
    </style>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="recombee">
        <h2>Recombee content recommendation</h2>
        <h4>Recombee database structure</h4>
        <div class="wrapper">
            <div class="wrapper">
                <p>The integration incrementally reflects changes in pages in the Recombee database. However, after configuring the integration for the first time,
                    the database has to be populated with existing pages.
                </p>
                <cms:LocalizedButton runat="server" ID="btnIntDbStructure" OnClick="InitDatabase_Click" Text="Initialize database" CssClass="btn btn-primary" />
            </div>
            <div class="wrapper">
                <p>Performs a reset of the Recombee database (i.e. erases its data and structure)</p>
                <cms:LocalizedButton runat="server" ID="btnResetDatabase" OnClick="ResetDatabase_Click" Text="Reset database" ButtonStyle="Default" />
            </div>
            <div class="wrapper" runat="server" id="pageTypesWrapper">
                Note: The following page types are currently configured to be pushed to the Recombee database: <span runat="server" id="pageTypes"></span>
            </div>
        </div>
    </div>  
</asp:Content>
