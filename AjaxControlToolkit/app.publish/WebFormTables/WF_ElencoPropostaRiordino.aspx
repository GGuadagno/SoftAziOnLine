<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ElencoPropostaRiordino.aspx.vb" Inherits="SoftAziOnLine.WF_ElencoPropostaRiordino" %>

<%@ Register src="../WebUserControlTables/WUC_ElencoPropostaRiordino.ascx" tagname="WUC_ElencoPropostaRiordino" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ElencoPropostaRiordino ID="WUC_ElencoPropostaRiordino1" runat="server" />

</asp:Content>
