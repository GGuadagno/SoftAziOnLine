<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WFP_DocCollegati.ascx.vb" Inherits="SoftAziOnLine.WFP_DocCollegati" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/WebUserControlTables/WUC_DocCollegati.ascx" TagName="DocCollegati" TagPrefix="wuc" %>
<%@ Register src="../WebUserControl/WUC_Attesa.ascx" tagname="Attesa" tagprefix="uc3" %>

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
        <asp:Label ID="Label1" runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="DOCUMENTI COLLEGATI" Width="99%"></asp:Label>
        <wuc:DocCollegati ID="WUCDocCollegati" runat="server" />
        <uc3:Attesa ID="Attesa" runat="server" />
    </asp:Panel>
    <asp:Panel ID="Panel1" runat="server" Height="30px" >
        <div style="overflow:auto; text-align:center;">    
            <asp:Button ID="btnOk" runat="server" Text="OK" OnClick="btnOk_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Annulla" OnClick="btnCancel_Click" />
            <asp:Button ID="btnStampa" runat="server" Text="Stampa documento" OnClick="btnStampa_Click" Visible="true" />
        </div>
   </asp:Panel>
    <asp:Panel ID="Panel3" runat="server" Height="30px" >
        <div style="overflow:auto;">
            <asp:Label ID="lblMessUtente" runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="" Width="99%"></asp:Label>
        </div>
   </asp:Panel> 
</asp:Panel>