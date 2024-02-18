<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_Magazzino.aspx.vb" Inherits="SoftAziOnLine.WF_Magazzino" %>

<%@ Register src="../WebUserControlTables/WUC_Magazzino.ascx" tagname="WUC_Magazzino" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">                
        
    <uc1:WUC_Magazzino ID="WUC_Magazzino1" runat="server" />              
        
</asp:Content>
    