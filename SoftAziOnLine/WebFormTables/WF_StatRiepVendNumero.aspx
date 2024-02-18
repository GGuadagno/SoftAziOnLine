<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatRiepVendNumero.aspx.vb" Inherits="SoftAziOnLine.WF_StatRiepVendNumero" %>

<%@ Register src="../WebUserControlTables/WUC_StatRiepVendNumero.ascx" tagname="WUC_StatRiepVendNumero" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatRiepVendNumero ID="WUC_StatRiepVendNumero1" runat="server" />

    </asp:Content>