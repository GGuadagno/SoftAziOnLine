<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_FatturatoClienteDocumento.aspx.vb" Inherits="SoftAziOnLine.WF_FatturatoClienteDocumento" %>
<%@ Register src="../WebUserControlTables/WUC_FattCliDoc.ascx" tagname="WUC_FattCliDoc" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <uc1:WUC_FattCliDoc ID="WUC_FattCliDoc1" runat="server" />
</asp:Content>
