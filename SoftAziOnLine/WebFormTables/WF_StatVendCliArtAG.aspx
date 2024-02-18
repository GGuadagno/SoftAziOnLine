<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatVendCliArtAG.aspx.vb" Inherits="SoftAziOnLine.WF_StatVendCliArtAG" %>

<%@ Register src="../WebUserControlTables/WUC_StatVendCliArtAG.ascx" tagname="WUC_StatVendCliArtAG" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatVendCliArtAG ID="WUC_StatVendCliArtAG1" runat="server" />

</asp:Content>