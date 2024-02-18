<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdinatoPerCliente.aspx.vb" Inherits="SoftAziOnLine.WF_OrdinatoPerCliente" %>

<%@ Register src="../WebUserControlTables/WUC_OrdinatoPerCliente.ascx" tagname="WUC_OrdinatoPerCliente" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdinatoPerCliente ID="WUC_OrdinatoPerCliente1" runat="server" />

</asp:Content>