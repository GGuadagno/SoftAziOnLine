<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatMovMag.aspx.vb" Inherits="SoftAziOnLine.WF_StatMovMag" %>

<%@ Register src="../WebUserControlTables/WUC_StatMovMag.ascx" tagname="WUC_StatMovMag" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatMovMag ID="WUC_StatMovMag1" runat="server" />

    </asp:Content>