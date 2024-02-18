<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ContrattiCKSerieLotto.aspx.vb" Inherits="SoftAziOnLine.WF_ContrattiCKSerieLotto" %>

<%@ Register src="../WebUserControlTables/WUC_ContrattiCKSerieLotto.ascx" tagname="WUC_ContrattiCKSerieLotto" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_ContrattiCKSerieLotto ID="WUC_ContrattiCKSerieLotto1" runat="server" />

    </asp:Content>
