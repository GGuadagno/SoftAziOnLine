<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ElencoBloccati.aspx.vb" Inherits="SoftAziOnLine.WF_ElencoBloccati" %>

<%@ Register src="../WebUserControlTables/WUC_ElencoBloccati.ascx" tagname="WUC_ElencoBloccati" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ElencoBloccati ID="WUC_ElencoBloccati1" runat="server" />

</asp:Content>
