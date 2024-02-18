<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Menu.ascx.vb" Inherits="SoftAziOnLine.WUC_Menu" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<style type="text/css">
    .style1240
    {
    	width: 1240px;
    }
    .style540x26
    {
    	width: 540px;
        height: 26px;
    }
    .style540x20
    {
    	width: 540px;
        height: 20px;
    }
    .style350x26
    {
        width: 345px;
        height: 26px;
    }
    .style350x20
    {
        width: 345px;
        height: 20px;
    }
    .style125x20
    {
        width: 125px;
        height: 20px;
    }
    .style620x20
    {
        width: 620px;
        height: 20px;
    }
    .style890x20
    {
        width: 890px;
        height: 20px;
    }
</style>
<br />    
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="600px" BackColor="Silver">
<table class="style1240">
<tr>
    <td style="border:None;width:1240px;height:20px;">
        <asp:LinkButton ID="LnkSalvaDataBase" runat="server" Text="Salva DB" Visible="false"
                        BorderColor="Snow" Font-Bold="false" Font-Size="Medium" ForeColor="White" 
                        BorderStyle="Outset" BackColor="DarkRed" Style="text-align:center;text-decoration:none"></asp:LinkButton>
        <asp:Label ID="lblMessBenvenuti" runat="server" BorderStyle="Ridge" Font-Bold="True" Text="Benvenuti su Soft Azienda OnLine"
            Style="text-align:left"></asp:Label>
        <asp:Label ID="Label6" runat="server" BorderStyle="None" Width="5px" ></asp:Label>
        <asp:Label ID="lblDataOdierna" runat="server" BorderStyle="Ridge" 
        Text="Oggi, Venerdì 2 settembre 2011" Font-Bold="True" 
         Style="text-align:center"></asp:Label>
        <asp:Label ID="Label7" runat="server" BorderStyle="None" Width="5px" ></asp:Label>
        <asp:Label ID="lblUltimoAccesso" runat="server" BorderStyle="Ridge" 
        Text="Accesso precedente: Giovedì 1 settembre 2011, 09:00"  
        Font-Bold="true" Style="text-align:right"></asp:Label>
        <asp:LinkButton ID="LnkAlert" runat="server" Text="Aggiorna Alert" Visible="true"
                        BorderColor="Snow" Font-Bold="false" Font-Size="Medium" ForeColor="White" 
                        BorderStyle="Outset" BackColor="DarkRed" Style="text-align:center;text-decoration:none"></asp:LinkButton>
    </td>
</tr>        
</table>
<table border="1" cellpadding="0" frame="box" class="style1240">
<tr>
    <td class="style540x20" style="border:None">
        &nbsp;Utente:
        <asp:Label ID="lblUtente" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" ForeColor="Black"></asp:Label>
        &nbsp;Tipo:
        <asp:Label ID="lblTipoUtente" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left" ForeColor="Black"></asp:Label>
    </td>
    <td align="center" class="style350x20"  
        style="color:White;background-color:#6B696B;font-weight:bold;border:none;">
        <asp:Label ID="lblDDT" runat="server" BorderStyle="None" Font-Bold="True" Text="ELENCO DOCUMENTI da fatturare"
            ForeColor="White" Style="text-align:center"></asp:Label>
        </td>
    <td align="center" class="style350x20"  
        style="color:White;background-color:#6B696B;font-weight:bold;border:none;">
        <asp:Label ID="lblSCMan" runat="server" BorderStyle="None" Font-Bold="True" Text="ELENCO ATTIVITA' in scadenza"
            ForeColor="White" Style="text-align:center"></asp:Label>
        </td>
</tr>
    <tr>
        <td class="style540x26" style="border:None">
            &nbsp;<asp:Label ID="lblUtConn0" runat="server" BorderStyle="None" 
                    Font-Bold="True" Style="text-align:left"  
                ForeColor="Black" Text="" ></asp:Label>
        </td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDDT1" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left"
            ForeColor="Black"></asp:Label>
        </td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblSCMan1" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
    </tr>
    <tr>
        <td class="style540x26" align="center"
            style="color:White;background-color:#6B696B;font-weight:bold;border:none;">
            &nbsp;<asp:Label ID="lblScProdCons" runat="server" BorderStyle="None" 
                Font-Bold="true" Style="text-align:center" Font-Size="Medium" ForeColor="White" Text=""></asp:Label>
        </td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDDT2" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblSCMan2" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            &nbsp;<asp:Label ID="lblUtConn1" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label>
        </td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDDT3" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblSCMan3" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            &nbsp;<asp:Label ID="lblUtConn2" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label>
        </td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDDT4" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblSCMan4" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            &nbsp;<asp:Label ID="lblUtConn3" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label>
        </td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDDT5" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblSCMan5" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            &nbsp;<asp:Label ID="lblUtConn4" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label>
        </td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDDT6" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblSCMan6" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            &nbsp;<asp:Label ID="lblUtConn5" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label>
        </td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDDT7" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblScMan7" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
    </tr>
     <tr>
        <td class="style540x26" style="border:None">
            &nbsp;<asp:LinkButton ID="LinkDocBloccati" runat="server" Text="ERRORE: Documenti con Qtà Evasa maggiore Qtà Ordinata - STATO Bloccato" Visible="false" Font-Bold="false" ForeColor="White" BackColor="DarkRed" BorderStyle="Outset"></asp:LinkButton>
        </td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDDT8" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblScMan8" runat="server" BorderStyle="None" 
            Font-Bold="true" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
    </tr>
</table>
<table border="1" cellpadding="0" frame="box" class="style1240">
<tr>
<td style="color:White;background-color:#6B696B;font-weight:bold;border:None;" >
        <asp:Label ID="lblOrdini_DocINCoGe" runat="server" BorderStyle="None" Font-Bold="True" Text="ELENCO ORDINI (N° Ordini e consegna prevista)"
            ForeColor="White" Style="text-align:center" class="style540x20"></asp:Label>
        </td>
<td align="center"  
    style="color:White;background-color:#6B696B;font-weight:bold;border:None;">
    <asp:Label ID="lblElencoDocCoGe" runat="server" BorderStyle="None" Font-Bold="True" Text="ELENCO DOCUMENTI da trasferire in CoGe"
        ForeColor="White" Style="text-align:center" class="style350x20"></asp:Label>
    </td>
<td align="center" 
    style="color:White;background-color:#6B696B;font-weight:bold;border:None;">
    <asp:Label ID="lblSCContr" runat="server" BorderStyle="None" Font-Bold="True" Text="ELENCO ATTIVITA' da fatturare"
        ForeColor="White" Style="text-align:center" class="style350x20" ></asp:Label>
    </td>
</tr>
<tr>
<td class="style540x26" style="border:None">
    <asp:Label ID="lblDoc1" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label>
    </td>
    <td class="style350x26" style="border:None">
        <asp:Label ID="lblDoc10" runat="server" BorderStyle="None" Font-Bold="True" 
            ForeColor="Black" Style="text-align:left"></asp:Label>
    </td>
    <td class="style540x26" style="border:None">
    <asp:Label ID="lblSCContr1" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label>
    </td>
</tr>
    <tr>
        <td class="style540x26" style="border:None">
            <asp:Label ID="lblDoc2" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDoc11" runat="server" BorderStyle="None" Font-Bold="True" 
                ForeColor="Black" Style="text-align:left"></asp:Label>
        </td>
        <td class="style540x26" style="border:None">
        <asp:Label ID="lblSCContr2" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left" 
            ForeColor="Black"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            <asp:Label ID="lblDoc3" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDoc12" runat="server" BorderStyle="None" Font-Bold="True" 
                ForeColor="Black" Style="text-align:left"></asp:Label>
        </td>
        <td class="style540x26" style="border:None">
        <asp:Label ID="lblSCContr3" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left" 
            ForeColor="Black"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            <asp:Label ID="lblDoc4" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDoc13" runat="server" BorderStyle="None" Font-Bold="True" 
                ForeColor="Black" Style="text-align:left"></asp:Label>
        </td>
        <td class="style540x26" style="border:None">
        <asp:Label ID="lblSCContr4" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left" 
            ForeColor="Black"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            <asp:Label ID="lblDoc5" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDoc14" runat="server" BorderStyle="None" Font-Bold="True" 
                ForeColor="Black" Style="text-align:left"></asp:Label>
        </td>
        <td class="style540x26" style="border:None">
        <asp:Label ID="lblSCContr5" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left" 
            ForeColor="Black"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            <asp:Label ID="lblDoc6" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDoc15" runat="server" BorderStyle="None" Font-Bold="True" 
                ForeColor="Black" Style="text-align:left"></asp:Label>
        </td>
        <td class="style540x26" style="border:None">
        <asp:Label ID="lblSCContr6" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left" 
            ForeColor="Black"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            <asp:Label ID="lblDoc7" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDoc16" runat="server" BorderStyle="None" Font-Bold="True" 
                ForeColor="Black" Style="text-align:left"></asp:Label>
        </td>
        <td class="style540x26" style="border:None">
        <asp:Label ID="lblSCContr7" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left" 
            ForeColor="Black"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            <asp:Label ID="lblDoc8" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDoc17" runat="server" BorderStyle="None" Font-Bold="True" 
                ForeColor="Black" Style="text-align:left"></asp:Label>
        </td>
        <td class="style540x26" style="border:None">
        <asp:Label ID="lblSCContr8" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left" 
            ForeColor="Black"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style540x26" style="border:None">
            <asp:Label ID="lblDoc9" runat="server" BorderStyle="None" 
            Font-Bold="True" Style="text-align:left" 
        ForeColor="Black"></asp:Label></td>
        <td class="style350x26" style="border:None">
            <asp:Label ID="lblDoc18" runat="server" BorderStyle="None" Font-Bold="True" 
                ForeColor="Black" Style="text-align:left"></asp:Label>
        </td>
        <td class="style540x26" style="border:None">
        <asp:Label ID="lblSCContr9" runat="server" BorderStyle="None" 
                Font-Bold="True" Style="text-align:left" 
            ForeColor="Black"></asp:Label>
        </td>
    </tr>
<%--</table>--%>
<%--<table border="1" cellpadding="0" frame="box" class="style1240">--%>
<tr>
<td align="center" style="color:White;background-color:#6B696B;font-weight:bold;border:None;" 
        class="style540x20">
        <asp:Label ID="lblMessaggiMenu1" runat="server" BorderStyle="None" Font-Bold="true" Text="Servizio salvataggio dati automatico"
            ForeColor="White" Style="text-align:center"></asp:Label>
        </td>
    <td align="center" class="style890x20" 
        style="color:White;background-color:#6B696B;font-weight:bold;border:None;" colspan="2">
        <asp:Label ID="lblMessaggiMenu2" runat="server" BorderStyle="None" Font-Bold="true" Text="Servizio Invio E-Mail automatico"
            ForeColor="White" Style="text-align:center"></asp:Label>
        </td>
    </tr>
</table>
</asp:Panel>