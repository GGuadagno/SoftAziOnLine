<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_FatturatoClienteDocumentoAG.aspx.vb" Inherits="SoftAziOnLine.WF_FatturatoClienteDocumentoAG" %>
<%@ Register src="../WebUserControlTables/WUC_FattCliDocAG.ascx" tagname="WUC_FattCliDocAG" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <uc1:WUC_FattCliDocAG ID="WUC_FattCliDocAG1" runat="server" />
</asp:Content>
