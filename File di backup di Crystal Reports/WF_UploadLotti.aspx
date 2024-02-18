<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_UploadLotti.aspx.vb" Inherits="SoftAziOnLine.WF_UploadLotti" %>

<%@ Register src="../WebUserControlTables/WUC_UploadLotti.ascx" tagname="WUC_UploadLotti" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <uc1:WUC_UploadLotti ID="WUC_UploadLotti1" runat="server" />

    </asp:Content>
