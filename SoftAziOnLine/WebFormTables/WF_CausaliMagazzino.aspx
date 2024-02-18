<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_CausaliMagazzino.aspx.vb" Inherits="SoftAziOnLine.WF_CausaliMagazzino" %>

<%@ Register src="../WebUserControlTables/WUC_CausaliMagazzino.ascx" tagname="WUC_CausaliMagazzino" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_CausaliMagazzino ID="WUC_CausaliMagazzino1" runat="server" />

</asp:Content>
