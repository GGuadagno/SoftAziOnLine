<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdineStampaCliArtAG.aspx.vb" Inherits="SoftAziOnLine.WF_OrdineStampaCliArtAG" %>

<%@ Register src="../WebUserControlTables/WUC_OrdCliArtAG.ascx" tagname="WUC_OrdCliArtAG" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdCliArtAG ID="WUC_OrdCliArtAG1" runat="server" />

</asp:Content>