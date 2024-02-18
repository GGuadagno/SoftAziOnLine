<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ElencoPreventivi.aspx.vb" Inherits="SoftAziOnLine.WF_ElencoPreventivi" %>

<%@ Register src="../WebUserControlTables/WUC_ElencoPreventivi.ascx" tagname="WUC_ElencoPreventivi" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ElencoPreventivi ID="WUC_ElencoPreventivi1" runat="server" />

</asp:Content>