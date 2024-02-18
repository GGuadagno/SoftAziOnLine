<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Magazzino.ascx.vb" Inherits="SoftAziOnLine.WUC_Magazzino" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<style type="text/css">
    .styleTDBTN
        {
            height: 478px;
        }
    .btnstyle
        {
            Width: 108px;
            height: 40px;
        margin-left: 0px;
        white-space: pre-wrap;      
    }
    .styleMenu
    {
        width: auto;
        border-style:groove;
    }
    .styleBordo
    {
        height: 35px;
        width: 860px;
        border-style:groove;
    }
    .style7
    {
        height: 185px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <br/>
<table align="center" width="980px" border="0"  style="background-color:white; height:495px;"  >
    <tr>
        <td align="center" style="background-color:white;height:100%;"  >                        
            <asp:Panel ID="PanelRicalcolo" runat="server" width="530px" 
                BackColor="Silver"  >
                <table style="width:530px;Height:100%; Border-Style:Double;">
                    <tr>
                        <td>
                            <asp:Label ID="lblTotArticoli" runat="server" Width="370px" Height="17px">Totale articoli da ricalcolare:</asp:Label>
                             <br/><br/>
                            <asp:Label ID="lblTotMovimenti" runat="server" Width="370px" Height="17px">Totale movimenti magazzino da ricalcolare</asp:Label>
                             <br/>
                            <asp:Label ID="lblDataPrimoMovimento" runat="server" Width="370px" Height="17px">Data primo movimento</asp:Label>
                             <br/>
                            <asp:Label ID="lblDataUltimoMovimento" runat="server" Width="370px" Height="17px">Data ultimo movimento</asp:Label>
                             <br/><br/>
                            <asp:Label ID="lblTotOrdCli" runat="server" Width="370px" Height="17px">Totale ordini clienti</asp:Label>
                             <br/>
                            <asp:Label ID="lblDataPrimoOrdCli" runat="server" Width="370px" Height="17px">Data primo ordine cliente</asp:Label>
                             <br/>
                            <asp:Label ID="lblDataUltimoOrdCli" runat="server" Width="370px" Height="17px">Data ultimo ordine cliente</asp:Label>
                             <br/<br/>
                            <asp:Label ID="lblTotOrdFor" runat="server" Width="370px" Height="17px">Totale ordini fornitori</asp:Label>
                             <br/>
                            <asp:Label ID="lblDataPrimoOrdFor" runat="server" Width="370px" Height="17px">Data primo ordine fornitore</asp:Label>
                             <br/>
                            <asp:Label ID="lblDataUltimoOrdFor" runat="server" Width="370px" Height="17px">Data ultimo ordine fornitore</asp:Label>                                                       
                        </td>
                        <td valign ="middle" align ="center" class="style7">
                            <asp:Button ID="btnRicalcola" runat="server" Text="Ricalcola giacenza" 
                                class="btnstyle" />
                             <br/>
                             <br/>
                            <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" 
                                class="btnstyle" />
                        </td>
                    </tr>
                </table>        
            </asp:Panel>                        
        </td>
    </tr> 
 </table>
    </ContentTemplate>
</asp:UpdatePanel>      
</asp:Panel>
