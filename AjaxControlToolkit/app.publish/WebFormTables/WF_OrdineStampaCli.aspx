<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdineStampaCli.aspx.vb" Inherits="SoftAziOnLine.WF_OrdineStampaCli" %>

<%@ Register src="../WebUserControlTables/WUC_OrdineStampaCli.ascx" tagname="WUC_OrdineStampaCli" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdineStampaCli ID="WUC_OrdineStampaCli1" runat="server" />

</asp:Content>