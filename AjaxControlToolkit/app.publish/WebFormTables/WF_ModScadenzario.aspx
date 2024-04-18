<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ModScadenzario.aspx.vb" Inherits="SoftAziOnLine.WF_ModScadenzario" %>

<%@ Register src="../WebUserControlTables/WUC_ModScadenzario.ascx" tagname="WUC_ModScadenzario" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ModScadenzario ID="WUC_ModScadenzario1" runat="server" />

    </asp:Content>
