<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ValorizzazioneMagazzino.aspx.vb" Inherits="SoftAziOnLine.WF_ValorizzazioneMagazzino" %>

<%@ Register src="../WebUserControlTables/WUC_ValorizzazioneMagazzino.ascx" tagname="WUC_ValorizzazioneMagazzino" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ValorizzazioneMagazzino ID="WUC_ValorizzazioneMagazzino1" runat="server" />

</asp:Content>
