<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_GeneraPropostaRiordino.aspx.vb" Inherits="SoftAziOnLine.WF_GeneraPropostaRiordino" %>

<%@ Register src="../WebUserControlTables/WUC_GeneraPropostaRiordino.ascx" tagname="WUC_GeneraPropostaRiordino" tagprefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_GeneraPropostaRiordino ID="WUC_GeneraPropostaRiordino1" runat="server" />
    
</asp:Content>
