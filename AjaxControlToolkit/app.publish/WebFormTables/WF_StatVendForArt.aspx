<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatVendForArt.aspx.vb" Inherits="SoftAziOnLine.WF_StatVendForArt" %>
<%@ Register src="../WebUserControlTables/WUC_StatVendForArt.ascx" tagname="WUC_StatVendForArt" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatVendForArt ID="WUC_StatVendForArt1" runat="server" />

</asp:Content>
