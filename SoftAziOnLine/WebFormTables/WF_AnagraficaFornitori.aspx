<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_AnagraficaFornitori.aspx.vb" Inherits="SoftAziOnLine.WF_AnagraficaFornitori" %>
<%@ Register src="~/WebUserControlTables/WUC_Fornitori.ascx" tagname="WUC_Fornitori" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
        
    <uc1:WUC_Fornitori ID="WUC_Fornitori" runat="server" />
    
</asp:Content>