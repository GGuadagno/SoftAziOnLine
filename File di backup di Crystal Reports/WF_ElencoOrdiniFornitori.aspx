<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ElencoOrdiniFornitori.aspx.vb" Inherits="SoftAziOnLine.WF_ElencoOrdiniFornitori" %>

<%@ Register src="../WebUserControlTables/WUC_ElencoOrdiniFornitori.ascx" tagname="WUC_ElencoOrdiniFornitori" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">


    <uc1:WUC_ElencoOrdiniFornitori ID="WUC_ElencoOrdiniFornitori1" runat="server" />


</asp:Content>