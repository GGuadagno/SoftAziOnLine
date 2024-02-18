<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_Menu.aspx.vb" Inherits="SoftAziOnLine.WF_Menu" %>

<%@ Register src="../WebUserControlTables/WUC_Menu.ascx" tagname="WUC_Menu" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">


    <uc1:WUC_Menu ID="WUC_Menu2" runat="server" />


</asp:Content>