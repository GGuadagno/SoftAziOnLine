<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatMovMagArt.aspx.vb" Inherits="SoftAziOnLine.WF_StatMovMagArt" %>

<%@ Register src="../WebUserControlTables/WUC_StatMovMagArt.ascx" tagname="WUC_StatMovMagArt" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatMovMagArt ID="WUC_StatMovMagArt1" runat="server" />

    </asp:Content>