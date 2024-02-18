<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatClientiMovimNuovi.aspx.vb" Inherits="SoftAziOnLine.WF_StatClientiMovimNuovi" %>

<%@ Register src="../WebUserControlTables/WUC_StatClientiMovimNuovi.ascx" tagname="WUC_StatClientiMovimNuovi" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatClientiMovimNuovi ID="WUC_StatClientiMovimNuovi1" runat="server" />

</asp:Content>
