<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ElencoOrdini.aspx.vb" Inherits="SoftAziOnLine.WF_ElencoOrdini" %>

<%@ Register src="../WebUserControlTables/WUC_ElencoOrdini.ascx" tagname="WUC_ElencoOrdini" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ElencoOrdini ID="WUC_ElencoOrdini1" runat="server" />

</asp:Content>
