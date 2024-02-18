<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_StatRiepVendNumeroAG.aspx.vb" Inherits="SoftAziOnLine.WF_StatRiepVendNumeroAG" %>

<%@ Register src="../WebUserControlTables/WUC_StatRiepVendNumeroAG.ascx" tagname="WUC_StatRiepVendNumeroAG" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_StatRiepVendNumeroAG ID="WUC_StatRiepVendNumeroAG1" runat="server" />

    </asp:Content>