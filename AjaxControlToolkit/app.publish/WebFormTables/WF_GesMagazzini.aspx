<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_GesMagazzini.aspx.vb" Inherits="SoftAziOnLine.WF_GesMagazzini" %>
<%@ Register Src="../WebUserControlTables/WUC_GesMagazzini.ascx" TagName="WUC_GesMagazzini" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <uc1:WUC_GesMagazzini ID="WUC_GesMagazzini1" runat="server" />
</asp:Content>
