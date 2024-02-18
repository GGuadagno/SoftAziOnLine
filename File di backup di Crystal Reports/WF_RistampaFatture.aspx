<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_RistampaFatture.aspx.vb" Inherits="SoftAziOnLine.WF_RistampaFatture" %>

<%@ Register src="../WebUserControlTables/WUC_RistampaFatture.ascx" tagname="WUC_RistampaFatture" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_RistampaFatture ID="WUC_RistampaFatture1" runat="server" />

</asp:Content>
