<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_TipoContratto.aspx.vb" Inherits="SoftAziOnLine.WF_TipoContratto" %>

<%@ Register src="../WebUserControlTables/WUC_TipoContratto.ascx" tagname="WUC_TipoContratto" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_TipoContratto ID="WUC_TipoContratto1" runat="server" />

</asp:Content>
