<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_FattAgenteCliente.aspx.vb" Inherits="SoftAziOnLine.WF_FattAgenteCliente" %>

<%@ Register src="../WebUserControlTables/WUC_FattAgenteCliente.ascx" tagname="WUC_FattAgenteCliente" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_FattAgenteCliente ID="WUC_FattAgenteCliente1" runat="server" />

    </asp:Content>