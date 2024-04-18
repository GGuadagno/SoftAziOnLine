<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_AnagraficaArticoli.aspx.vb" Inherits="SoftAziOnLine.WF_AnagraficaArticoli" %>
<%@ Register src="~/WebUserControlTables/WUC_Articoli.ascx" tagname="WUC_Articoli" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
        
    <uc1:WUC_Articoli ID="WUC_Articoli" runat="server" />
    
</asp:Content>