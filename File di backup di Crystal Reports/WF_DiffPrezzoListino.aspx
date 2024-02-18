<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_DiffPrezzoListino.aspx.vb" Inherits="SoftAziOnLine.WF_DiffPrezzoListino" %>

<%@ Register src="../WebUserControlTables/WUC_DiffPrezzoListino.ascx" tagname="WUC_DiffPrezzoListino" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_DiffPrezzoListino ID="WUC_DiffPrezzoListino1" runat="server" />

    </asp:Content>