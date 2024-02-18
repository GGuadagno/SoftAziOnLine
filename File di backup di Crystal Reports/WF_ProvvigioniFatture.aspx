<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_ProvvigioniFatture.aspx.vb" Inherits="SoftAziOnLine.WF_ProvvigioniFatture" %>



<%@ Register src="../WebUserControlTables/WUC_ProvvigioniFatture.ascx" tagname="WUC_ProvvigioniFatture" tagprefix="uc1" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

 

    <uc1:WUC_ProvvigioniFatture ID="WUC_ProvvigioniFatture1" runat="server" />

 

</asp:Content>