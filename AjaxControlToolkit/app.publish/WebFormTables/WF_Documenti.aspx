<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_Documenti.aspx.vb" Inherits="SoftAziOnLine.WF_Documenti" %>

<%@ Register src="../WebUserControlTables/WUC_Documenti.ascx" tagname="WUC_Documenti" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_Documenti ID="WUC_Documenti1" runat="server" />

</asp:Content>