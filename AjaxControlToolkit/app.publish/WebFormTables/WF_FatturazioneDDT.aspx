<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_FatturazioneDDT.aspx.vb" Inherits="SoftAziOnLine.WF_FatturazioneDDT" %>

<%@ Register src="../WebUserControlTables/WUC_FatturazioneDDT.ascx" tagname="WUC_FatturazioneDDT" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_FatturazioneDDT ID="WUC_FatturazioneDDT1" runat="server" />

</asp:Content>
