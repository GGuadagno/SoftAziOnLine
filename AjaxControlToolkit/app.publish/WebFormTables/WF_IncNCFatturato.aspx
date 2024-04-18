<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_IncNCFatturato.aspx.vb" Inherits="SoftAziOnLine.WF_IncNCFatturato" %>

<%@ Register src="../WebUserControlTables/WUC_IncidenzaNCFatturato.ascx" tagname="WUC_IncidenzaNCFatturato" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_IncidenzaNCFatturato ID="WUC_IncidenzaNCFatturato1" runat="server" />

</asp:Content>
