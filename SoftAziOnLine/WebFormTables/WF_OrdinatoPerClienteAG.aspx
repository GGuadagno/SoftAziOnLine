<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdinatoPerClienteAG.aspx.vb" Inherits="SoftAziOnLine.WF_OrdinatoPerClienteAG" %>

<%@ Register src="../WebUserControlTables/WUC_OrdinatoPerClienteAG.ascx" tagname="WUC_OrdinatoPerClienteAG" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdinatoPerClienteAG ID="WUC_OrdinatoPerClienteAG1" runat="server" />

</asp:Content>
