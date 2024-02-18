﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ModalPopup.ascx.vb" Inherits="SoftAziOnLine.WUC_ModalPopup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<%--<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/StyleSheet.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css" />--%>

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
    CancelControlID="CancelButton" 
    DropShadow="True"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>
<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:350px;padding:10px">
    <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;text-align:center;">
        <div style="text-align:left;">
            &nbsp;
            <asp:Image ID="ImageIcon" runat="server" ImageUrl="~/Immagini/Icone/warning.png" />
            &nbsp;
            <asp:Label ID="LabelTitle" runat="server" Text="Titolo" Font-Bold="true" Font-Size="Medium"></asp:Label>
            <hr />
            <asp:Label ID="LabelMessage" runat="server" Text="Messaggio"></asp:Label>
            <br />
            <asp:TextBox ID="txtValore" runat="server"/>
            <br />
            <asp:Label ID="LabelMessage2" runat="server" Text="Messaggio2" Visible="false"></asp:Label>
            <br />
            <asp:TextBox ID="txtValore2" runat="server"/>
        </div>
    </asp:Panel>
    <br />        
    <div style="text-align:center;">    
        <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="OkButton_Click"/>
        <asp:Button ID="NoButton" runat="server" Text="No" OnClick="NoButton_Click" />
        <asp:Button ID="CancelButton" runat="server" Text="No" OnClick="CancelButton_Click" />
    </div>
   <br />
</asp:Panel>

