<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatOrdinatoClienteOrdine.aspx.vb" Inherits="SoftAziOnLine.WF_StatOrdinatoClienteOrdine" %>
<%@ Register src="../WebUserControlTables/WUC_StatOrdinatoClienteOrdine.ascx" tagname="WUC_StatOrdinatoClienteOrdine" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <uc1:WUC_StatOrdinatoClienteOrdine ID="WUC_StatOrdinatoClienteOrdine1" runat="server" />
</asp:Content>
