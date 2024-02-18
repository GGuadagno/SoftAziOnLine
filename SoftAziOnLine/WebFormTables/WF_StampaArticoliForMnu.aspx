<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StampaArticoliForMnu.aspx.vb" Inherits="SoftAziOnLine.WF_StampaArticoliForMnu" %>
<%@ Register src="../WebUserControlTables/WUC_StampaArticoliFornitori.ascx" tagname="WUC_StampaArticoliFornitori" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StampaArticoliFornitori ID="WUC_StampaArticoliFornitori1" runat="server" />

</asp:Content>
