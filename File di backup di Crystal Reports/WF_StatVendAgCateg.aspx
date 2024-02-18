<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatVendAgCateg.aspx.vb" Inherits="SoftAziOnLine.WF_StatVendAgCateg" %>
<%@ Register src="../WebUserControlTables/WUC_StatVendAgCateg.ascx" tagname="WUC_StatVendAgCateg" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatVendAgCateg ID="WUC_StatVendAgCateg1" runat="server" />

</asp:Content>