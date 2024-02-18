<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatVendCliForArt.aspx.vb" Inherits="SoftAziOnLine.WF_StatVendCliForArt" %>
<%@ Register src="../WebUserControlTables/WUC_StatVendCliForArt.ascx" tagname="WUC_StatVendCliForArt" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatVendCliForArt ID="WUC_StatVendCliForArt1" runat="server" />

</asp:Content>