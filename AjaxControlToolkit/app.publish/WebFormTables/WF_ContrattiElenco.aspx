<%@ Page Language="vb"  MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ContrattiElenco.aspx.vb" Inherits="SoftAziOnLine.WF_ContrattiElenco" %>

<%@ Register src="../WebUserControlTables/WUC_ContrattiElenco.ascx" tagname="WUC_ContrattiElenco" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ContrattiElenco ID="WUC_ContrattiElenco1" runat="server" />

</asp:Content>