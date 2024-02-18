<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ElencoSchedeInventario.aspx.vb" Inherits="SoftAziOnLine.WF_ElencoSchedeInventario" %>

<%@ Register src="../WebUserControlTables/WUC_ElencoSchedeInventario.ascx" tagname="WUC_ElencoSchedeInventario" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">


    <uc1:WUC_ElencoSchedeInventario ID="WUC_ElencoSchedeInventario1" runat="server" />


</asp:Content>
