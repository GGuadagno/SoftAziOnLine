<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdineStampaCliArt.aspx.vb" Inherits="SoftAziOnLine.WF_OrdineStampaArtCli" %>

<%@ Register src="../WebUserControlTables/WUC_OrdCliArt.ascx" tagname="WUC_OrdCliArt" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdCliArt ID="WUC_OrdCliArt1" runat="server" />

</asp:Content>