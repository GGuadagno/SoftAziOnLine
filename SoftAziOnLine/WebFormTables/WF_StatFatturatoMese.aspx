<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatFatturatoMese.aspx.vb" Inherits="SoftAziOnLine.WF_StatFatturatoMese" %>

<%@ Register src="../WebUserControlTables/WUC_StatFatturatoMese.ascx" tagname="WUC_StatFatturatoMese" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatFatturatoMese ID="WUC_StatFatturatoMese1" runat="server" />

</asp:Content>
