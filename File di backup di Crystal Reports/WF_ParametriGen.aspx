<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ParametriGen.aspx.vb" Inherits="SoftAziOnLine.WF_ParametriGen" %>

<%@ Register src="../WebUserControlTables/WUC_ParametriGen.ascx" tagname="WUC_ParametriGen" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ParametriGen ID="WUC_ParametriGen1" runat="server" />

</asp:Content>