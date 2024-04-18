<%@ Page Language="vb"  MasterPageFile="~/ContentPage.Master" AutoEventWireup="false" CodeBehind="WF_EvadiDocumenti.aspx.vb" Inherits="SoftAziOnLine.WF_EvadiDocumenti" %>

<%@ Register src="../WebUserControlTables/WUC_EvadiDocumenti.ascx" tagname="WUC_EvadiDocumenti" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_EvadiDocumenti ID="WUC_EvadiDocumenti1" runat="server" />

</asp:Content>
