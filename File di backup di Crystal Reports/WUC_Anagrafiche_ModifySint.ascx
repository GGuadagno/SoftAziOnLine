<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Anagrafiche_ModifySint.ascx.vb" Inherits="SoftAziOnLine.WUC_Anagrafiche_ModifySint" %>
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
        <asp:Label ID="Label1" runat="server" Text="Label"><b>Modifica dati anagrafica selezionata</b></asp:Label>
    </asp:Panel>
    <table width= "500px" border="2px;" style="margin:10px;">
        <tr>
            <td style="padding:10px;" align="left">
                <asp:Label ID="lblCodice" runat="server" BorderColor="White" 
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="100px">Codice</asp:Label><br />
                <asp:Label ID="lblRagSoc" runat="server" BorderColor="White" 
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="99%">Ragione Sociale</asp:Label><br />
                <asp:Label ID="lblDenominazione" runat="server" BorderColor="White" 
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="99%">Denominazione</asp:Label><br />
                <%--<asp:Label ID="lblLabelPICF" runat="server" Width="30px">P.IVA</asp:Label>
                <asp:Label ID="lblPICF" runat="server" Width="165px" BorderColor="White"
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label><br />--%>
                <asp:Label ID="lblIndirizzo" runat="server" BorderColor="White"
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="99%">INDIRIZZO</asp:Label><br />
                <asp:Label ID="lblLocalita" runat="server" BorderColor="White"
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="99%">LOCALITA</asp:Label>
                <asp:Label ID="Label3" runat="server" ><b>E-mail cliente</b></asp:Label><br />
                <asp:TextBox ID="txtEmail" AutoPostBack="true" runat="server" Width="99%" 
                    MaxLength="100"></asp:TextBox>
                <asp:Label ID="Label2" runat="server" ><b>E-mail invio scadenza</b></asp:Label><br />
                <asp:TextBox ID="txtEmailInvioScad" AutoPostBack="true" runat="server" Width="99%" 
                    MaxLength="100"></asp:TextBox>
                <asp:CheckBox ID="chkInvioMailScad" AutoPostBack="true" runat="server" Text="Si/No Invio E-mail scadenza" TabIndex="1" />
                <asp:CheckBox ID="chkChiudiEmail" AutoPostBack="false" Enabled="false" Visible="false" runat="server" Text="Si/No Chiudi E-mail" TabIndex="1" />
                <asp:CheckBox ID="chkApriEmail" AutoPostBack="false" Enabled="false" Visible="false" runat="server" Text="Si/No Apri E-mail" TabIndex="1" />
            </td>
        </tr>
    </table>     
    <table width= "500px" border="2px;" style="margin:10px;">
        <tr>
            <td style="padding:10px;" align="left">
                <asp:Label ID="lblCodFiliale" runat="server" BorderColor="White" 
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="100px">Codice Filiale</asp:Label><br />
                <asp:Label ID="lblRagSocDest" runat="server" BorderColor="White" 
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="99%">Ragione Sociale</asp:Label><br />
                <asp:Label ID="lblIndirizzoDest" runat="server" BorderColor="White"
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="99%">INDIRIZZO</asp:Label><br />
                <asp:Label ID="lblLocalitaDest" runat="server" BorderColor="White"
                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="99%">LOCALITA</asp:Label>
                <asp:Label ID="lblEmailDest" runat="server" ><b>E-mail destinazione</b></asp:Label><br />
                <asp:TextBox ID="txtEmailDest" AutoPostBack="true" runat="server" Width="99%" 
                    MaxLength="100"></asp:TextBox>
            </td>
        </tr>
    </table> 
    <div style="text-align:center;">  
        <asp:CheckBox ID="chkAggEmailT" AutoPostBack="false" Enabled="true" Visible="true" runat="server" Text="Si/No Aggiorna tutte le E-mail inviate in archivio" TabIndex="1" />  
        <asp:Label ID="Label5" runat="server" Width="50px"></asp:Label>
        <asp:Button ID="btnOK" runat="server" Text="OK"  OnClick="OkButton_Click"/>
        <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" OnClick="CancelButton_Click"/>
    </div>
    <div style="text-align:left;">
        <asp:Label ID="Label6" runat="server" Width="99%" Text="Invio E-mail scadenze articoli consumabili"></asp:Label>
    </div>
    <div style="text-align:left;">
        <asp:Label ID="lblEmailInvio" runat="server" ForeColor="Blue" BorderStyle="Outset" Font-Bold="true" Width="99%" Text=""></asp:Label><br />
    </div>
    <div style="text-align:center;">
        <asp:Label ID="lblMess" runat="server" ForeColor="Red" ><b>Nota ricaricare i dati di ricerca al termine della modifica</b></asp:Label>
    </div>
</asp:Panel>
 