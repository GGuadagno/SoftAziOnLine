<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ValMagCostoVenduto.aspx.vb" Inherits="SoftAziOnLine.WF_ValMagCostoVenduto" %>

<%@ Register src="../WebUserControlTables/WUC_ValMagCostoVenduto.ascx" tagname="WUC_ValMagCostoVenduto" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ValMagCostoVenduto ID="WUC_ValMagCostoVenduto1" runat="server" />

</asp:Content>
