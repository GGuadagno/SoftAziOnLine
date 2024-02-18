<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdinatoClienteOrdine.aspx.vb" Inherits="SoftAziOnLine.WF_OrdinatoClienteOrdine" %>

<%@ Register src="../WebUserControlTables/WUC_OrdinatoClienteOrdine.ascx" tagname="WUC_OrdinatoClienteOrdine" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdinatoClienteOrdine ID="WUC_OrdinatoClienteOrdine1" runat="server" />

</asp:Content>