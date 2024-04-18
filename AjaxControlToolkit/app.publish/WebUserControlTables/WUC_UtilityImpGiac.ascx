<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_UtilityImpGiac.ascx.vb" Inherits="SoftAziOnLine.WUC_UtilityImpGiac" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
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
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<div style="text-align:center;Height:100%"> 
<table align="center" width="980px" border="0"  style="background-color:white; height:530px;"  >
    <tr>
        <td align =center style="background-color:white;height:100%;"  >                        
            <asp:Panel ID="PanelRicalcolo" runat="server" width="530px" 
                BackColor="Silver"  >
                <table style="width:530px;Height:100%; Border-Style:Double;">
                    <tr>
                        <td>
                            <!--<asp:Label ID="lblTotArticoli" runat="server" Width="370px" Height="17px">Totale articoli da ricalcolare:</asp:Label>-->
                             <!--<BR>-->
                            <asp:Label ID="lblTotOrdini" runat="server" Width="370px" Height="17px">Totale ordini:</asp:Label>
                             <br/>
                            <asp:Label ID="lblDataPrimoOrdine" runat="server" Width="370px" Height="17px">Data primo ordine:</asp:Label>
                             <br/>
                            <asp:Label ID="lblDataUltimoOrdine" runat="server" Width="370px" Height="17px">Data ultimo ordine:</asp:Label>
                        </td>
                        <td valign ="middle" align ="center" class="style7">
                            <asp:Button ID="btnImpegna" runat="server" Text="Impegna giacenza" 
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
</div> 