<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ContrattiElScadPag.aspx.vb" Inherits="SoftAziOnLine.WF_ContrattiElScadPag" %>

<%@ Register src="../WebUserControlTables/WUC_ContrattiElScadPag.ascx" tagname="WUC_ContrattiElScadPag" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ContrattiElScadPag ID="WUC_ContrattiElScadPag1" runat="server" />

</asp:Content>
