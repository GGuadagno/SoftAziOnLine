<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdinatoPerArticolo.aspx.vb" Inherits="SoftAziOnLine.WF_OrdinatoPerArticolo" %>

<%@ Register src="../WebUserControlTables/WUC_OrdinatoPerArticolo.ascx" tagname="WUC_OrdinatoPerArticolo" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdinatoPerArticolo ID="WUC_OrdinatoPerArticolo1" runat="server" />

</asp:Content>
