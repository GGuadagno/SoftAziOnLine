<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ArticoliUbicazione.aspx.vb" Inherits="SoftAziOnLine.WF_ArticoliUbicazione" %>

<%@ Register src="../WebUserControlTables/WUC_ArticoliUbicazione.ascx" tagname="WUC_ArticoliUbicazione" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ArticoliUbicazione ID="WUC_ArticoliUbicazione1" runat="server" />

</asp:Content>
