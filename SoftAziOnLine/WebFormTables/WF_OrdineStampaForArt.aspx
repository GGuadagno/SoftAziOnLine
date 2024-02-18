<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdineStampaForArt.aspx.vb" Inherits="SoftAziOnLine.WF_OrdineStampaForArt" %>

<%@ Register src="../WebUserControlTables/WUC_OrdForArt.ascx" tagname="WUC_OrdForArt" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdForArt ID="WUC_OrdForArt1" runat="server" />

</asp:Content>
