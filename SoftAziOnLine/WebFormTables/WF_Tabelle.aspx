<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_Tabelle.aspx.vb" Inherits="SoftAziOnLine.WF_Tabelle" %>

<%@ Register src="../WebUserControlTables/WUC_Tabelle.ascx" tagname="WUC_Tabelle" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">


    <uc1:WUC_Tabelle ID="WUC_Tabelle1" runat="server" />


</asp:Content>
