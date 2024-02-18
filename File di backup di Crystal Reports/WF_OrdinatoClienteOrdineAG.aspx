<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdinatoClienteOrdineAG.aspx.vb" Inherits="SoftAziOnLine.WF_OrdinatoClienteOrdineAG" %>

<%@ Register src="../WebUserControlTables/WUC_OrdinatoClienteOrdineAG.ascx" tagname="WUC_OrdinatoClienteOrdineAG" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdinatoClienteOrdineAG ID="WUC_OrdinatoClienteOrdineAG1" runat="server" />

</asp:Content>