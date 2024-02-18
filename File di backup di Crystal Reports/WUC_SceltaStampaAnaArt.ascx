<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_SceltaStampaAnaArt.ascx.vb" Inherits="SoftAziOnLine.WUC_SceltaStampaAnaArt" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

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
    CancelControlID="btnAnnulla" 
    DropShadow="True"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>

<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:auto;padding:10px">
    <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;text-align:center;">
        <asp:Label ID="Label1" runat="server" Text="Label"><b>Stampa anagrafica articoli</b></asp:Label>
    </asp:Panel>
   
        
  <br />  
    <table width= "350px" border="2px;" style="margin:10px;">
        <tr>
            <td style="padding:10px; width:50%">
                <asp:Label ID="Label3" runat="server" ><b>Ordinamento</b></asp:Label><br /><br />
                <asp:RadioButton ID="rbCodice" runat="server" Text="per codice" GroupName="Tipo"/><br />
                <asp:RadioButton ID="rbDescrizione" runat="server" Text="per descrizione" GroupName="Tipo"/>
            </td>
            <td style="padding:10px; width:50%">
                <asp:Label ID="Label2" runat="server" visible="true"><b>Tipo di stampa</b></asp:Label><br /><br />
                <asp:RadioButton ID="rbSintetica" runat="server" visible="true" Text="sintetica" GroupName="TipoStampa"/><br />
                <asp:RadioButton ID="rbAnalitica" runat="server" visible="true" Text="analitica" GroupName="TipoStampa"/>
            </td>
            
            
        </tr>
        </table>  
        
        <!--<table  width= "350px" border="2px;" style="margin:10px;">
        <tr>
            <td style="padding:10px;">
                <asp:Label ID="Label4" runat="server" ><b>Articoli</b></asp:Label><br /><br />
                <table style="margin:0px;">
                    <tr>
                        <td>
                            <asp:RadioButton ID="rbArticoli" runat="server" Text="Tutti gli articoli" GroupName="TipoStampa" AutoPostBack="False" /><br />
                        </td>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton ID="rbDaCodiceAcodice" runat="server" Text="Da codice" GroupName="TipoStampa" AutoPostBack="False" />
                        </td>
                        <td >
                            <asp:TextBox ID="txtDaCodice" runat="server" style="margin:5px" MaxLength =20></asp:TextBox>
                           <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="Server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;a codice:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtACodice" runat="server" style="margin:5px" MaxLength =20 ></asp:TextBox>
                        </td>
                    </tr>
                </table>                               
            </td>
        </tr>
    </table>-->
    
    <br />       
    <br />  
    <div style="text-align:center;">    
        <asp:Button ID="btnOK" runat="server" Text="OK"  OnClick="OkButton_Click"/>
        <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" OnClick="CancelButton_Click"/>
    </div>
   <br />
 
   
</asp:Panel>
 