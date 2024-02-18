<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ArticoliInstallati.aspx.vb" Inherits="SoftAziOnLine.WF_ArticoliInstallati" %>

<%@ Register src="../WebUserControlTables/WUC_ArticoliInstallati.ascx" tagname="WUC_ArticoliInstallati" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ArticoliInstallati ID="WUC_ArticoliInstallati1" runat="server" />

</asp:Content>