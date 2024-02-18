<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_RicollegaVisteCoGe.aspx.vb" Inherits="SoftAziOnLine.WF_RicollegaVisteCoGe" %>

<%@ Register src="../WebUserControlTables/WUC_RicollegaVisteCoGe.ascx" tagname="WUC_RicollegaVisteCoGe" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_RicollegaVisteCoGe ID="WUC_RicollegaVisteCoGe1" runat="server" />

</asp:Content>
