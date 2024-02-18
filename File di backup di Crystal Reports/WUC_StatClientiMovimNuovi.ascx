<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_StatClientiMovimNuovi.ascx.vb" Inherits="SoftAziOnLine.WUC_StatClientiMovimNuovi" %>
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
        <br />
        <br />
        <br />
        <br />
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height:260px; width:980px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelSelezionaDate" style="margin-top: 0px;" runat="server" GroupingText="Statistica Clienti Movimentati">
                        <div><br></div>
                        <div>
                            <asp:Label ID="lblTotale" runat="server" Height="16px">Totale Clienti Movimentati/Nuovi al </asp:Label>
                            <asp:DropDownList ID="DDLEserFinoAl" runat="server" Height="22px" Width="60px"  
                                AppendDataBoundItems="True" 
                                DataSourceID="SqlDSEserFinoAl" DataTextField="Esercizio" 
                                DataValueField="Esercizio" Enabled="true">
                                <asp:ListItem ></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDSEserFinoAl" runat="server" 
                                SelectCommand="SELECT [Ditta], [Esercizio] FROM [Esercizi] WHERE ([Ditta] = @Ditta) ORDER BY Esercizio DESC">
                                <SelectParameters>
                                    <asp:SessionParameter DefaultValue="00" Name="Ditta" SessionField="CodiceDitta" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </div> 
                        <br>
                        <asp:Label ID="lblPerRegione" runat="server" Height="16px">Suddiviso per Regione</asp:Label>
                        <br><br />                       
                     </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                 <div>
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" 
                                         TabIndex="20" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" 
                                        TabIndex="21" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    </tr>
                </caption>
                </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>