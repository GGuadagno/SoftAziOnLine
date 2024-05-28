﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WFP_AnagrProvv_Insert.ascx.vb" Inherits="SoftAziOnLine.WFP_AnagrProvv_Insert" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/WebUserControlTables/WUC_AnagrProvv_Insert.ascx" TagName="AnagrProvv_Insert" TagPrefix="wuc" %>


<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/StyleSheet.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css" />

<style type="text/css">
    .modalBackground {
        background-color:Gray;
        filter:alpha(opacity=70);
        opacity:0.7;
    }

    .modalPopup {
        background-color:#ffffdd;
        border-width:3px;
        border-style:solid;
        border-color:Gray;
        padding:3px;
        width:250px;
    }
</style>

<ajaxToolkit:ModalPopupExtender runat="server" ID="ProgrammaticModalPopup"
    TargetControlID="LinkButton1"
    PopupControlID="programmaticPopup" 
    BackgroundCssClass="modalBackground"
    DropShadow="true"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>
<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:auto;padding:10px">
    <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;text-align:center;">
        <wuc:AnagrProvv_Insert ID="WUC_AnagrProvv_Insert" runat="server" />
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel3" runat="server" Height="25px">
        <asp:Label ID="lblMessUtente" runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
            Style="text-align:center" Text="" Width="99%"></asp:Label>
   </asp:Panel> 
    <br />           
    <div style="text-align:center;">
        <asp:Button ID="btnUsaAnagPresentiPIVA" runat="server" Text="Associa dati per P. IVA" />   
        <asp:Button ID="btnUsaAnagPresentiCF" runat="server" Text="Associa dati per C.F." />  
        <asp:Button ID="btnUsaAnagPresentiRag" runat="server" Text="Associa dati per Ragione Sociale" />  
        <asp:Button ID="btnAggiorna" runat="server" Text="Seleziona e aggiorna" OnClick="btnAggiorna_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Annulla" OnClick="btnCancel_Click" />
    </div>
   <br />
</asp:Panel>