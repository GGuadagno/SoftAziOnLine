<%@ Page Language="vb" MasterPageFile="~/ContentPage.Master" AutoEventWireup="true" CodeBehind="WF_GiacInizioAnno.aspx.vb" Inherits="SoftAziOnLine.WF_GiacInizioAnno" %>

<%@ Register src="../WebUserControlTables/WUC_GiacInizioAnno.ascx" tagname="WUC_GiacInizioAnno" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

     <uc1:WUC_GiacInizioAnno ID="WUC_GiacInizioAnno1" runat="server" />

</asp:Content>