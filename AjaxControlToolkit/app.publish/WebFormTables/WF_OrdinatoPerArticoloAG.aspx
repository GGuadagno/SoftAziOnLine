<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_OrdinatoPerArticoloAG.aspx.vb" Inherits="SoftAziOnLine.WF_OrdinatoPerArticoloAG" %>

<%@ Register src="../WebUserControlTables/WUC_OrdinatoPerArticoloAG.ascx" tagname="WUC_OrdinatoPerArticoloAG" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_OrdinatoPerArticoloAG ID="WUC_OrdinatoPerArticoloAG1" runat="server" />

</asp:Content>
