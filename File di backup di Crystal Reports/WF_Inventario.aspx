<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_Inventario.aspx.vb" Inherits="SoftAziOnLine.WF_Inventario" %>

<%@ Register src="../WebUserControlTables/WUC_Inventario.ascx" tagname="WUC_Inventario" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_Inventario ID="WUC_Inventario1" runat="server" />

</asp:Content>
