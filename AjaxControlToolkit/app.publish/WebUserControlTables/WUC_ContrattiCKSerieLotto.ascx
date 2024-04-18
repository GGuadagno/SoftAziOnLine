<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ContrattiCKSerieLotto.ascx.vb" Inherits="SoftAziOnLine.WUC_ContrattiCKSerieLotto" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<style type="text/css">
    .styleTDBTN
    {
        height: 478px;
    }
    .btnstyle
    {
        Width: 108px;
        height: 35px;
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
</style>
<br />
<br />
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <br />
        <br />
        <br />
        <br />
        <br />
        <div align="center" >        
        <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 206px; width: 708px;">
            <tr>
                <td class="style8">
                    <asp:Panel ID="PanelSelezionaDate" runat="server" GroupingText="VERIFICA,MODIFICA E STAMPA N° SERIE / LOTTO con caratteri speciali" 
                        style="margin-top: 0px;" Width="574px" heigth="66px">                        
                        
                        <br />
                    </asp:Panel>
                </td>
                <td align="left" class="style7">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnStampa" runat="server" class="btnstyle" TabIndex="20" 
                                    Text="Solo Stampa" Visible="false" />
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div>
                                <asp:Button ID="btnAggiorna" runat="server" class="btnstyle" TabIndex="20" 
                                    Text="Modifica e Stampa" />
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div>
                                <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" TabIndex="21" 
                                    Text="Annulla" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            </caption>
            </tr>
        </table>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>