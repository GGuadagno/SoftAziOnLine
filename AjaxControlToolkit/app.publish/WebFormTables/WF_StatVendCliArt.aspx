<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatVendCliArt.aspx.vb" Inherits="SoftAziOnLine.WF_StatVendCliArt" %>

<%@ Register src="../WebUserControlTables/WUC_StatVendCliArt.ascx" tagname="WUC_StatVendCliArt" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatVendCliArt ID="WUC_StatVendCliArt1" runat="server" />

</asp:Content>
