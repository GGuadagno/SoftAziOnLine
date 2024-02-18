<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_FatturazioneRiepDDT.aspx.vb" Inherits="SoftAziOnLine.WF_FatturazioneRiepDDT" %>

<%@ Register src="../WebUserControlTables/WUC_FatturazioneRiepDDT.ascx" tagname="WUC_FatturazioneRiepDDT" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_FatturazioneRiepDDT ID="WUC_FatturazioneRiepDDT1" runat="server" />

</asp:Content>
