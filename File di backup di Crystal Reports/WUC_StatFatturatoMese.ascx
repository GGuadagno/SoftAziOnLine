<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_StatFatturatoMese.ascx.vb" Inherits="SoftAziOnLine.WUC_StatFatturatoMese" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
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
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaFornitori" Titolo="Elenco fornitori" />
        <br />
        <br />
        <br />
        <br />
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height:260px; width:980px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelFor" runat="server" groupingtext="Fornitori" style="margin-top: 0px;" Height="78px" Width="100%" Visible="false">
                            <asp:Label ID="lblFornitore" runat="server" Width="160px" Height="17px">Fornitore</asp:Label>
                            <asp:Button ID="btnFornitore" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca fornitore" Enabled="false" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodFornitore" runat="server" Width="138px" MaxLength="16" AutoPostBack="True" TabIndex="6" Enabled="false" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDescFornitore" runat="server" Width="440px" MaxLength="50" TabIndex="7" Enabled="False"  ></asp:TextBox>
                        <br>
                        <asp:CheckBox ID="chkTuttiFornitori" runat="server" AutoPostBack="True" TabIndex="8" Text="Seleziona tutti i fornitori" TextAlign="Left" Checked="true"/>
                            <br>
                        </asp:Panel>
                    <br />
                    <asp:Panel ID="PanelSelezionaDate" style="margin-top: 0px;" runat="server" GroupingText="Statistica Fatturato Annuo (In corso/Precedente)">
                        <br />
                        <div>
                            <asp:Label ID="Label1" runat="server" Height="16px">Seleziona l'esecizio in corso</asp:Label>
                            <asp:DropDownList ID="DDLEser" runat="server" Height="22px" Width="60px"  
                                AppendDataBoundItems="True" AutoPostBack="true"
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
                            <asp:Label ID="Label2" runat="server" Text="" Width="100px"></asp:Label>
                            <asp:Label ID="Label8" runat="server" Text="Fino al Mese "></asp:Label>
                            <asp:DropDownList ID="DDLFinoAlMM" runat="server" 
                                AppendDataBoundItems="True" AutoPostBack="false" Height="22px" TabIndex="2" Width="155px">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                <asp:ListItem Text="GENNAIO" Value="1"></asp:ListItem>
                                <asp:ListItem Text="FEBBRAIO" Value="2"></asp:ListItem>
                                <asp:ListItem Text="MARZO" Value="3"></asp:ListItem>
                                <asp:ListItem Text="APRILE" Value="4"></asp:ListItem>
                                <asp:ListItem Text="MAGGIO" Value="5"></asp:ListItem>
                                <asp:ListItem Text="GIUGNO" Value="6"></asp:ListItem>
                                <asp:ListItem Text="LUGLIO" Value="7"></asp:ListItem>
                                <asp:ListItem Text="AGOSTO" Value="8"></asp:ListItem>
                                <asp:ListItem Text="SETTEMBRE" Value="9"></asp:ListItem>
                                <asp:ListItem Text="OTTOBRE" Value="10"></asp:ListItem>
                                <asp:ListItem Text="NOVEMBRE" Value="11"></asp:ListItem>
                                <asp:ListItem Text="DICEMBRE" Value="12"></asp:ListItem>
                            </asp:DropDownList>  
                        </div> 
                        <br>                      
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