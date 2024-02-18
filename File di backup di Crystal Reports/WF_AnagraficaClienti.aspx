<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_AnagraficaClienti.aspx.vb" Inherits="SoftAziOnLine.WF_AnagraficaClienti" %>
<%@ Register src="~/WebUserControlTables/WUC_Clienti.ascx" tagname="WUC_Clienti" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <uc1:WUC_Clienti ID="WUC_Clienti" runat="server" />
</asp:Content>
