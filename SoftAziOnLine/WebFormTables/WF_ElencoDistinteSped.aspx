<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ElencoDistinteSped.aspx.vb" Inherits="SoftAziOnLine.WF_ElencoDistinteSped" %>

<%@ Register src="../WebUserControlTables/WUC_ElencoDistinteSped.ascx" tagname="WUC_ElencoDistinteSped" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ElencoDistinteSped ID="WUC_ElencoDistinteSped1" runat="server" />

</asp:Content>
