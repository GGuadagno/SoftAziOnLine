<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_SceltaStampaCFRubrica.ascx.vb" Inherits="SoftAziOnLine.WUC_SceltaStampaCFRubrica" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/StyleSheet.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css" />



<style type="text/css">
    .modalBackground {
        background-color:Gray;
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
    CancelControlID="btnAnnulla" 
    DropShadow="True"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>

<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:auto;padding:10px">
    <br />  
    <table width= "350px" border="2px;" style="margin:10px;">
        <tr>
            <td style="padding:10px; width:50%">
                <asp:RadioButton ID="rbtnNessuno" runat="server" Text="Stampa anagrafica clienti" GroupName="TipoDettagli" Checked="true"/><br />
                <asp:RadioButton ID="rbtnContatti" runat="server" Text="Stampa contatti" GroupName="TipoDettagli"/><br />
                <asp:RadioButton ID="rbtnDestinazioni" runat="server" Text="Stampa destinazioni merce" GroupName="TipoDettagli"/><br />
            </td>
        </tr>
        </table>      
    <br />       
    <br />  
    <div style="text-align:center;">    
        <asp:Button ID="btnOK" runat="server" Text="OK"  OnClick="OkButton_Click"/>
        <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" OnClick="CancelButton_Click"/>
    </div>
   <br />
 
   
</asp:Panel>
 