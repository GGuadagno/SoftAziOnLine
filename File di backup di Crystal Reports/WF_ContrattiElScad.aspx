<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ContrattiElScad.aspx.vb" Inherits="SoftAziOnLine.WF_ContrattiElScad" %>

<%@ Register src="../WebUserControlTables/WUC_ContrattiElScad.ascx" tagname="WUC_ContrattiElScad" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ContrattiElScad ID="WUC_ContrattiElScad1" runat="server" />

</asp:Content>
