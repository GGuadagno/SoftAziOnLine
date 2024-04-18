<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_DocumentiElencoFor.aspx.vb" Inherits="SoftAziOnLine.WF_DocumentiElencoFor" %>

<%@ Register src="../WebUserControlTables/WUC_DocumentiElenco.ascx" tagname="WUC_DocumentiElenco" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_DocumentiElenco ID="WUC_DocumentiElenco1" runat="server" />

</asp:Content>
