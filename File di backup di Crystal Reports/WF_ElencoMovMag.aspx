<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ElencoMovMag.aspx.vb" Inherits="SoftAziOnLine.WF_ElencoMovMag" %>

<%@ Register src="../WebUserControlTables/WUC_ElencoMovMag.ascx" tagname="WUC_ElencoMovMag" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ElencoMovMag ID="WUC_ElencoMovMag1" runat="server" />

</asp:Content>
