<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_DispMag.aspx.vb" Inherits="SoftAziOnLine.WF_DispMag" ValidateRequest="false" %>

<%@ Register src="../WebUserControlTables/WUC_DispMag.ascx" tagname="WUC_DispMag" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_DispMag ID="WUC_DispMag1" runat="server" />

</asp:Content>
