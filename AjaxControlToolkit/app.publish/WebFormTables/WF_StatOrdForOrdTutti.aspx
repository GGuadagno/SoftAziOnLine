<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatOrdForOrdTutti.aspx.vb" Inherits="SoftAziOnLine.WF_StatOrdForOrdTutti" %>

<%@ Register src="../WebUserControlTables/WUC_OrdForArt.ascx" tagname="WUC_StatOrdForOrdTutti" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatOrdForOrdTutti ID="WUC_StatOrdForOrdTutti1" runat="server" />

</asp:Content>

