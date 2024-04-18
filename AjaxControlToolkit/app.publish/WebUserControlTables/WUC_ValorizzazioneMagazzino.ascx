<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ValorizzazioneMagazzino.ascx.vb" Inherits="SoftAziOnLine.WUC_ValorizzazioneMagazzino" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>
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
        height: 324px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaFornitori" Titolo="Elenco fornitori" />
            <br /> 
            <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 340px; width: 940px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelMagazzino" style="margin-top: 0px;border-style:groove;" runat="server" GroupingText="">
                        &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblMagazzino" runat="server" Width="146px" Height="16px">Magazzino</asp:Label>
                        <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"
                               AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                               DataTextField="Descrizione" 
                               DataValueField="Codice" Width="545px" TabIndex="2">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
                               SelectCommand="SELECT Codice, Descrizione FROM Magazzini ORDER BY Descrizione">
                        </asp:SqlDataSource>
                    </asp:Panel>
                    <asp:Panel ID="PanelOrdinamento" style="margin-top: 0px;border-style:groove;" runat="server" groupingtext="Ordinamento - Raggruppamento - Tipo Stampa Analitica/Sintetica per Fornitore">
                        <table style="width: 100%;">
                        <tr>
                            <td width="40%" align="left">
                            <asp:RadioButton ID="rbtnCodice" runat="server" Text="Codice articolo" 
                            Checked="true" AutoPostBack="true" GroupName="Ordinamento" TabIndex="3" />
                            </td>
                            <td width="60%" align="left">
                            <div>
                            <asp:RadioButton ID="rbtnRaggrFornCodice" runat="server" Text="Analitica per Fornitore / Codice articolo (Totali parziali per Fornitore)" 
                            Checked="false" AutoPostBack="true" GroupName="Ordinamento" TabIndex="3" />
                            </div>
                            <div>
                            <asp:RadioButton ID="rbtnRaggrFornSint" runat="server" Text="Sintetica per Fornitore (Solo Totali parziali per Fornitore)" 
                            Checked="false" AutoPostBack="true" GroupName="Ordinamento" TabIndex="3" />
                            </div>
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="Panel" runat="server" groupingtext="" style="margin-top: 0px;border-style:groove;" Height="78px" Width="847px">
                        <br>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblFornitore" runat="server" Width="145px" Height="17px">Fornitore</asp:Label>
                            <asp:Button ID="btnFornitore" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca fornitore" Enabled="false" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodFornitore" runat="server" Width="138px" MaxLength="16" AutoPostBack="True" TabIndex="6" Enabled="false" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDescFornitore" runat="server" Width="440px" MaxLength="50" TabIndex="7" Enabled="False"  ></asp:TextBox>
                        <br>
                        &nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkTuttiFornitori" runat="server" AutoPostBack="True" TabIndex="8" Text="Seleziona tutti i fornitori" TextAlign="Left" Checked="true" />
                        </asp:Panel>
                    <asp:Panel ID="PanelSelezione" runat="server" groupingtext="Selezione" style="margin-top: 0px;border-style:groove;" Width="847px"> 
                    <table>
                    <tr>
                        <td><asp:Label ID="lblCategoria" runat="server" Width="145px" Height="16px">Categoria</asp:Label>
                            <asp:Button ID="btnTrovaCategoria" runat="server" Height="22px" Text="?"
                                Visible="False" Width="30px" />
                            <asp:TextBox ID="txtCodCategoria" runat="server" AutoPostBack="true" Width="80px" TabIndex="6" MaxLength="3"></asp:TextBox>
                            <asp:DropDownList ID="ddlCatgoria" runat="server" AppendDataBoundItems="true"
                                    AutoPostBack="true" DataSourceID="SqlDataSourceCategoria" 
                                    DataTextField="Descrizione" 
                                    DataValueField="Codice" Width="545px" TabIndex="7">
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceCategoria" runat="server"
                                    SelectCommand="SELECT Codice, Descrizione FROM CategArt ORDER BY Descrizione">
                                </asp:SqlDataSource>
                           </td>
                    </tr>
                    <tr>
                        <td>
                           <asp:Label ID="lblLinea" runat="server" Width="145px" Height="16px">Linea</asp:Label>
                                <asp:Button ID="btnTrovaLinea" runat="server" Height="22px" Text="?"
                                   Visible="False" Width="30px" />
                                <asp:TextBox ID="txtCodLinea" runat="server" AutoPostBack="true" 
                                Width="80px" TabIndex="8" MaxLength="3"></asp:TextBox>
                                 <asp:DropDownList ID="ddlLinea" runat="server" AppendDataBoundItems="true"
                                 AutoPostBack="true" DataSourceID="SqlDataSourceLinea" 
                                  DataTextField="Descrizione" DataValueField="Codice" Width="545px" 
                                TabIndex="9">
                                 <asp:ListItem Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceLinea" runat="server"
                                SelectCommand="SELECT Codice, Descrizione FROM LineeArt ORDER BY Descrizione" />
                        </td>
                    </tr>
                    <tr>
                    <td>
                        <asp:Label ID="lblDal" runat="server" Width="145px" Height="17px">Dal codice articolo</asp:Label>
                        <asp:Button ID="btnCod1" runat="server" Width="30px" Height="22px" Text="?" ToolTip="Ricerca articolo" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server"  
                                Width="145px" MaxLength="20" AutoPostBack="false" TabIndex="10" ></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblAl" runat="server" Width="135px" Height="17px">Al codice articolo</asp:Label>&nbsp;
                            <asp:Button ID="btnCod2" runat="server" Height="22px" Width="30px" Text="?" ToolTip="Ricerca articolo" />
                        &nbsp;&nbsp;
                            <asp:TextBox ID="txtCod2" runat="server"  
                                Width="145px" MaxLength="20" TabIndex="11" AutoPostBack="false" ></asp:TextBox>
                        <tr>
                            <td>
                                <table style="width: 100%;">
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%;">
                                    <tr>
                                        <td align="left" width="100%">
                                            <asp:CheckBox ID="chkArtGiacNoZero" runat="server" AutoPostBack="false" 
                                                TabIndex="12" Text="Stampa solo articoli con giacenza diversa da ZERO" />
                                        </td>
                                    </tr>
                                </table>
                                <table style="width: 100%;">
                                    <tr>
                                        <td align="left" width="100%">
                                            <asp:CheckBox ID="chkArtGiacNegativa" runat="server" AutoPostBack="false" 
                                                TabIndex="12" Text="Stampa solo articoli con giacenza NEGATIVA" />
                                            <asp:Label ID="lblMessNegativi" runat="server" Height="17px" Visible="false" ForeColor="Red" Font-Bold="true"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <br />
                                <table style="width: 100%;">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Height="17px" Width="240px">Valorizzazione giacenza alla data</asp:Label>
                                            &nbsp;<asp:TextBox ID="txtData" runat="server" MaxLength="10" TabIndex="1" 
                                                Width="70px"></asp:TextBox><asp:ImageButton ID="imgBtnShowCalendar" runat="server" 
                                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                ToolTip="apri il calendario" />
                                            <asp:CalendarExtender ID="txtData_CalendarExtender" runat="server" 
                                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendar" 
                                                TargetControlID="txtData">
                                            </asp:CalendarExtender>
                                        </td>
                                    </tr>
                                </table>
                                <%--</br>--%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%;">
                                    <asp:Label ID="lblMess" runat="server" Height="17px" Visible="false" ForeColor="Red" Font-Bold="true"></asp:Label>
                                </table>
                            </td>
                        </tr>
                    </tr>
                    </table>
                      <br>
                    </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <div style="height: 65px"></div>
                                 <div id="noradio">
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Elabora Stampa" 
                                         TabIndex="14" />
                                </div>
                                <div>
                                    <a ID="LnkStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Stampa">Apri Stampa</a>
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" 
                                        TabIndex="15" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div style="height: 15px">
                                </div>                                
                                <div>
                                    <asp:Button ID="BtnStampaControllo" runat="server" class="btnstyle" Text="Stampa di controllo" 
                                        TabIndex="15" />
                                </div>        
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>   
</asp:Panel>