<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ContrattiDett.ascx.vb" Inherits="SoftAziOnLine.WUC_ContrattiDett" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WFP_Articolo_Seleziona.ascx" tagname="WFP_Articolo_Seleziona" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WFP_ElencoDestCF.ascx" tagname="WFP_ElencoDestCF" tagprefix="uc15" %>
<style type="text/css">
    .btnstyle2Righe
        {
            Width: 108px;
            height: 45px;
        margin-left: 0px;
        white-space: pre-wrap;
        }   
</style> 
<div align="left" style="border:1 solid White; Width:1090px; height:610px;">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc2:WFP_Articolo_Seleziona ID="WFP_Articolo_Seleziona1" runat="server" />
        <uc15:WFP_ElencoDestCF ID="WFPElencoDestCF" runat="server" Elenco="ListaDestCliFor" Titolo="Elenco Destinazioni" />
         <asp:SqlDataSource ID="SqlDSRespAreaApp" runat="server" 
            SelectCommand="SELECT *, RTRIM(CAST(Codice AS NVARCHAR(10))) + ' - ' + Descrizione AS CodDes FROM [RespArea] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSRespVisiteApp" runat="server" 
            SelectCommand="SELECT *, RTRIM(CAST(Codice AS NVARCHAR(10))) + ' - ' + Descrizione AS CodDes FROM [RespVisite] WHERE ([CodRespArea] = @IDRespAreaApp) ORDER BY [Descrizione]">
            <SelectParameters>
                <asp:SessionParameter Name="IDRespAreaApp" SessionField="IDRespAreaApp" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <table style="width:auto; height:auto;" class="sfondopagine" >
            <tr>
                <td colspan="4">
                        <asp:Panel ID="PanelDett" runat="server" Height="305px" Width ="1090px">
                        <asp:UpdatePanel ID="UpdatePanelDett" runat="server"><ContentTemplate>
                        <div id="divGridViewDett" style="overflow: auto; height:305px; border-style:groove; background-color: Silver;">
                          <asp:GridView ID="GridViewDett" runat="server" AutoGenerateColumns="False" 
                                EmptyDataText="Nessun dato disponibile."  
                                DataKeyNames="Riga" 
                                GridLines="None" CssClass="GridViewStyle" EnableTheming="True"
                                AllowPaging="true" 
                                PageSize="10" 
                                PagerSettings-Mode="NextPreviousFirstLast"
                                PagerSettings-Visible="True"
                                PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" >
                                <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                                    LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                                    Mode="NextPreviousFirstLast" 
                                    NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                    PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                                <RowStyle CssClass="RowStyle" />
                                <PagerStyle CssClass="PagerStyle" />
                                <HeaderStyle CssClass="HeaderStyle" />
                                <AlternatingRowStyle CssClass="AltRowStyle" />
                                <SelectedRowStyle CssClass="SelectedRowStyle"/>
                                <Columns>
                                    <asp:TemplateField InsertVisible="True"><ItemTemplate>
                                    <asp:Button ID="btnInsRigaDopo" runat="server" CommandName="Select" Text="+" /> 
                                    </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10px" />
                                    </asp:TemplateField>
                                    <asp:CommandField ButtonType="Button" ShowEditButton="True" 
                                        HeaderStyle-Width="50px" >
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="50px" />
                                    </asp:CommandField>
                                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" 
                                        HeaderStyle-Width="20px" DeleteText="X" > 
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="20px" />
                                    </asp:CommandField>
                                </Columns>
                          </asp:GridView>
                          </div>
                          </ContentTemplate>
                          </asp:UpdatePanel>
                          </asp:Panel>   
                       </td>
                    </tr> 
            <tr>
            <td colspan="4">
            <table >
            <tr>
            <td>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Riga "></asp:Label>
                    <asp:Label ID="lblRigaSel" runat="server" BorderStyle="Outset" Width="30px" 
                            Font-Bold="True"></asp:Label> 
                    <asp:Label ID="lblBase" runat="server" BorderStyle="Outset" Width="20px" Font-Bold="False" Visible="false"></asp:Label>
                    <asp:Label ID="lblOpz" runat="server" BorderStyle="Outset" Width="20px" Font-Bold="False" Visible="false"></asp:Label> 
                    <asp:Button ID="BtnSelArticolo" runat="server" CommandName="BtnSelArticolo" Text="?" ToolTip="Seleziona articoli" />
                    <asp:Button ID="btnPrimaRiga" runat="server" CommandName="btnPrimaRiga" Text="+" Visible="false" ForeColor="Black" BackColor="Green" ToolTip="Inserisci 1° riga" />
                </td>
                <td>Totale Documento</td><td align="right"><asp:Label ID="LblTotale" runat="server" BorderStyle="Outset" Width="105px" Font-Bold="True"></asp:Label></td>
                <td><asp:Label ID="Label18" runat="server" Width="10px"></asp:Label></td>
                <td>
                    <div style="width:260px">
                        <asp:Label ID="Label8" runat="server" BorderStyle="None" Font-Bold="false" Text="Scelta dettagli"></asp:Label>
                        <asp:DropDownList ID="DDLTipoDettagli" runat="server" Width="150px" AutoPostBack="true">
                                        <asp:ListItem Text="Apparecchiature" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Attività per periodo" Value="1"></asp:ListItem>
                                  </asp:DropDownList>
                     </div>
                    <asp:Label ID="Label4" runat="server" BorderStyle="None" Font-Bold="true" Text="Totale dettagli N°"></asp:Label>
                    <asp:Label ID="lblTotDett" runat="server" BorderStyle="Outset" Width="105px" Font-Bold="True"></asp:Label>
                </td>
                <td>
                    <div style="width:190px">
                        <asp:Label ID="lblDurNumRiga" runat="server" BorderStyle="None" Font-Bold="True" Text="N°"></asp:Label>
                        <asp:DropDownList ID="DDLDurNumRIga" runat="server" Width="150px" AutoPostBack="true"></asp:DropDownList>
                    </div>
                    <asp:CheckBox ID="chkSelModifica" runat="server" Font-Bold="false" Text="Seleziona per Modifica" Checked="false" AutoPostBack="false"/>
                 </td>
                <td>
                     <div style="width:80px">
                        <asp:Label ID="Label23" runat="server" BorderStyle="None" Font-Bold="true" Text="Modello"></asp:Label>
                        <asp:DropDownList ID="DDLModello" runat="server" Width="60px" AutoPostBack="true">
                                        <asp:ListItem Text="" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="HS1" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="FR2" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="FR3" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="FRX" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="C1" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="C2" Value="6"></asp:ListItem>
                                  </asp:DropDownList>
                     </div>
                </td>
                 <td>
                    <asp:Button ID="btnDelDettagli" runat="server" CommandName="btnDelDettagli" Text="Cancella Dettagli" ToolTip="Cancella Dettagli dell'Apparechiatura o Periodo selezionato" />
                     <asp:Button ID="btnDelPeriodiAtt" runat="server" CommandName="btnDelPeriodiAtt" Text="Cancella Periodi Attività" Visible="false" ForeColor="White" BackColor="DarkRed" ToolTip="Cancella Periodi Attività" />
                </td>
            </td>
            </tr></table></td>
            </tr>
        <tr>
        <td colspan="4">
                <asp:TabContainer ID="PanelSubDettArt" runat="server" ActiveTabIndex="0" Width="1090px" Height="250px" BackColor="Silver">
                     <asp:TabPanel runat="server" ID="TabPanelDettArtIns" HeaderText="Aggiorna/Inserisci dettagli Apparecchiature/Attività"  BackColor="Silver">
                     <HeaderTemplate>Aggiorna/Inserisci dettagli Apparecchiature/Attività</HeaderTemplate>
                     <ContentTemplate>
                     <table class="sfondopagine" style="Width:960px;Height:255px;">
                     <tr>
                     <td align="left">
                        <asp:Panel ID="DettArtInsPanel" runat="server" Height="250px" Width ="960px">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server"><ContentTemplate>
                        <div id="div2" style="overflow: auto; height:245px; border-style:groove; background-color: Silver;">
                          <div>
                              <asp:Label ID="Label6" runat="server" Width="35px">&nbsp;</asp:Label> 
                              <asp:Label ID="lblCodice" runat="server" Width="155px">Codice</asp:Label> 
                              <asp:Label ID="lblDescrizione" runat="server" Width="425px">Descrizione</asp:Label> 
                              <asp:Label ID="lblLabelUM" runat="server" Width="30px">UM</asp:Label>
                              <asp:Label ID="lblLabelQtaOr" runat="server" Width="50px">Ordin.</asp:Label> 
                              <asp:Label ID="lblLabelQtaEv" runat="server" Width="54px">Evasa</asp:Label>
                              <asp:Label ID="lblLabelQtaRe" runat="server" Width="55px" Visible="false">Residua</asp:Label>
                              <asp:CheckBox ID="chkFatturata" runat="server" Font-Bold="false" Text="Fatturata" Checked="false" AutoPostBack="true" TextAlign="Left"/> 
                          </div>
                          <div>
                              <asp:Label ID="Label20" runat="server" Width="5px">&nbsp;</asp:Label>
                              <asp:Button ID="BtnSelArticoloIns" runat="server" CommandName="BtnSelArticoloIns" Text="?" ToolTip="Ricerca articoli" Enabled="false"/>
                              <asp:TextBox ID="txtCodArtIns" runat="server" Width="150px" MaxLength="20" AutoPostBack="true"  BorderStyle="None"></asp:TextBox>
                              <asp:TextBox ID="txtDesArtIns" runat="server" Width="425px" MaxLength="150" BorderStyle="None"></asp:TextBox>
                              <asp:TextBox ID="txtUMIns" runat="server" MaxLength="2" Width="25px" BorderStyle="None"></asp:TextBox>
                              <asp:TextBox ID="txtQtaIns" runat="server" MaxLength="5" Width="45px" BorderStyle="None"></asp:TextBox>
                              <asp:Label ID="lblQtaEv" runat="server" BorderStyle="Outset" Width="50px" Font-Bold="True"></asp:Label>
                              <asp:Label ID="LblQtaRe" runat="server" BorderStyle="Outset" Width="50px" Font-Bold="True" Visible="false"></asp:Label>
                              <asp:Label ID="lblQtaFa" runat="server" BorderStyle="Outset" Width="50px" Font-Bold="True"></asp:Label>
                          </div>
                          <div>&nbsp;
                          </div>
                          <div>
                              <asp:Label ID="Label21" runat="server" Width="5px">&nbsp;</asp:Label>
                              <asp:Label ID="Label12" runat="server" Width="25px">IVA</asp:Label>
                              <asp:Label ID="lblPrezzoAL" runat="server" Width="104px" Text="Prezzo listino"></asp:Label>
                              <asp:Label ID="Label10" runat="server" Width="55px">Sc.(1)</asp:Label>
                              <asp:Label ID="Label5" runat="server" Width="105px">Prezzo Netto</asp:Label>
                              <asp:Label ID="Label2" runat="server" Width="100px">Importo riga</asp:Label>
                              <asp:Label ID="Label16" runat="server" Width="100px"></asp:Label>
                              <asp:CheckBox ID="chkSWSostituito" runat="server" Font-Bold="false" Text="Sostituito" Width="150px" Checked="false" AutoPostBack="false"/>
                              <asp:Label ID="lblDesDataSc" runat="server" Width="115px" Font-Bold="false">Data scadenza</asp:Label>
                              <asp:CheckBox ID="chkEvasa" runat="server" Font-Bold="false" Text="Data evasione" Checked="false" AutoPostBack="true" TextAlign="Left"/>
                          <div>
                              <asp:Label ID="Label19" runat="server" Width="5px">&nbsp;</asp:Label>
                              <asp:TextBox ID="txtIVAIns" runat="server" Width="20px" MaxLength="2" AutoPostBack="true" BorderStyle="None"></asp:TextBox>
                              <asp:TextBox ID="txtPrezzoIns" runat="server" Width="100px" MaxLength="15" BorderStyle="None"></asp:TextBox>
                              <asp:TextBox ID="txtSconto1Ins" runat="server" MaxLength="5" Width="40px" BorderStyle="None"></asp:TextBox>
                              <asp:Label ID="LblPrezzoNetto" runat="server" BorderStyle="Outset" Width="100px" Font-Bold="True"></asp:Label>
                              <asp:Label ID="LblImportoRiga" runat="server" BorderStyle="Outset" Width="100px" Font-Bold="True"></asp:Label>
                              <asp:CheckBox ID="chkNoPrezzo" runat="server" Font-Bold="true" Text="No prezzo" Width="105px" Checked="true" AutoPostBack="true"/>
                              <asp:CheckBox ID="chkSWCalcoloTot" runat="server" Font-Bold="false" Text="Escluso dal Totale" Width="150px" Checked="false" AutoPostBack="false"/>
                              <asp:TextBox ID="txtDataSc" runat="server" AutoPostBack="false" MaxLength="10" Width="70px" BorderStyle="None"></asp:TextBox>
                                    <asp:ImageButton ID="ImgDataSc" runat="server" CausesValidation="False" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                        ToolTip="apri il calendario"/>
                                    <asp:CalendarExtender ID="txtDataSc_CalendarExtender1" runat="server" Enabled="True"
                                        Format="dd/MM/yyyy" PopupButtonID="ImgDataSc"
                                        TargetControlID="txtDataSc">
                                    </asp:CalendarExtender>
                              <asp:Label ID="Label9" runat="server" Width="20px"></asp:Label>
                              <asp:TextBox ID="txtDataEv" runat="server" AutoPostBack="false" MaxLength="10" Width="70px" BorderStyle="None"></asp:TextBox>
                                    <asp:ImageButton ID="ImgDataEv" runat="server" CausesValidation="False" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                        ToolTip="apri il calendario"/>
                                    <asp:CalendarExtender ID="txtDataEv_CalendarExtender" runat="server" Enabled="True"
                                        Format="dd/MM/yyyy" PopupButtonID="ImgDataEv"
                                        TargetControlID="txtDataEv">
                                    </asp:CalendarExtender>
                          </div>
                          <div style="width:950px">
                                &nbsp;<asp:Label ID="lblMessAgg" runat="server" BorderStyle="Outset" Width="930px" Font-Bold="true"></asp:Label>
                          </div>
                          <div style="width: 950px">&nbsp;
                                <asp:Label ID="Label3" runat="server">Serie</asp:Label>
                                <asp:TextBox ID="txtSerie" runat="server" Width="140px" MaxLength="16" BorderStyle="None" AutoPostBack="true"></asp:TextBox>
                                <asp:Label ID="Label14" runat="server" Visible="false">Lotto</asp:Label>
                                <asp:TextBox ID="txtLotto" runat="server" Width="140px" MaxLength="16" Visible="false" BorderStyle="None"></asp:TextBox>
                                
                                <asp:TextBox ID="txtNote" runat="server" Width="290px" MaxLength="30" BorderStyle="None"></asp:TextBox>
                          </div>
                          <div>&nbsp;
                          </div>
                          <div style="width: 950px">
                                &nbsp;<asp:Label ID="lblLabelSelDestD" runat="server" BorderStyle="Outset" Font-Bold="true" Width="95px">Luogo App.</asp:Label>
                                <asp:Button 
                                    ID="btnCercaDestD" runat="server" CommandName="btnCercaDestD" 
                                    Text="?" ToolTip="Ricerca Destinazione" />
                                <asp:Button
                                    ID="btnModificaDestD" runat="server" CommandName="btnModificaDestD"
                                    Text="+ M" ToolTip="Inserimento/Modifica dati Destinazione"  Visible="false" />
                                <asp:Button 
                                    ID="btnInsDestD" runat="server" CommandName="btnInsDestD" 
                                    Text="+" ToolTip="Nuova Destinazione" Visible="false" />
                                 <asp:Label ID="lblDestSelDett" runat="server" BorderStyle="Outset" Font-Bold="false" Width="765px"></asp:Label>
                                 <asp:Button ID="btnDelDestD" runat="server" CommandName="btnDelDestD" 
                                        Text="X" ToolTip="Nessuna Destinazione" />
                          </div>
                          <div>&nbsp;
                          </div>
                          <div style="width: 950px">
                                &nbsp;<asp:Label ID="Label24" runat="server" BorderStyle="Outset" Font-Bold="true" Width="95px">Resp.Area</asp:Label>
                                 <asp:DropDownList ID="DDLRespAreaApp" runat="server" AppendDataBoundItems="True" 
                                    AutoPostBack="True" DataSourceID="SqlDSRespAreaApp" DataTextField="Descrizione" 
                                    DataValueField="Codice" Height="22px" Width="365px">
                                    <asp:ListItem ></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="Label25" runat="server" BorderStyle="Outset" Font-Bold="true">Resp.Visite</asp:Label>
                                <asp:DropDownList ID="DDLRespVisiteApp" runat="server" AppendDataBoundItems="True" 
                                    AutoPostBack="True" DataSourceID="SqlDSRespVisiteApp" DataTextField="Descrizione" 
                                    DataValueField="Codice" Height="22px" Width="365px">
                                    <asp:ListItem ></asp:ListItem>
                                </asp:DropDownList>
                          </div>
                          <div style="width: 950px">
                                &nbsp;<asp:Label ID="lblMessRespAV" runat="server" BorderStyle="Outset" Font-Bold="true" ForeColor="DarkRed" Width="930px"></asp:Label>
                          </div>
                        </div>
                        </ContentTemplate>
                        </asp:UpdatePanel>
                        </asp:Panel>   
                     </td>
                     <td>
                     <div>
                        <asp:Button ID="btnAggArtGridSel" runat="server" CommandName="btnAggArtGridSel" 
                        Text="Aggiorna riga" Height="35px" Width="105px" />
                     </div>
                     <div style="height: 5px"></div>
                     <div>
                        <asp:Label ID="lblSuperatoScMax" runat="server" BorderStyle="None"
                                 Width="100px" Font-Bold="True"></asp:Label>
                     </div>
                     <div style="height: 5px"></div>
                     <div style="height: 25px">
                        <asp:CheckBox ID="checkNoScontoValore" runat="server" Font-Bold="True" Visible="false" Text="NO Sc.Val."/>
                     </div>    
                     <div>
                        <asp:Label ID="Label11" runat="server" BorderStyle="None" Visible="false" 
                                 Width="105px" Font-Bold="False" Text="Costo unitario"></asp:Label>
                     </div>
                      <div>
                        <asp:TextBox ID="txtPrezzoCosto" runat="server" Width="100px" MaxLength="15" Visible="false" 
                              Enabled="False" BorderStyle="None"></asp:TextBox>
                      </div>
                     </td>
                     </tr>
                     </table>
                     </ContentTemplate>
                     </asp:TabPanel>
                     <asp:TabPanel runat="server" ID="PanelDettArtNoteInterv" HeaderText="Aggiorna/Inserisci Note Intervento Apparecchiature/Attività"  BackColor="Silver" Visible="false">
                     <HeaderTemplate>Aggiorna/Inserisci Note Intervento Apparecchiature/Attività</HeaderTemplate>
                     <ContentTemplate>
                     <table class="sfondopagine" style="Width:960px;Height:250px;">
                     <tr>
                     <td align="left">
                        <asp:Panel ID="Panel1" runat="server" Height="250px" Width ="960px">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server"><ContentTemplate>
                        <div id="div1" style="overflow: auto; height:240px; border-style:groove; background-color: Silver;">
                          <asp:TextBox  ID="txtNoteIntervento" runat="server" Width="950px" Height="235px" TextMode="MultiLine" BorderStyle="None" AutoPostBack="false" Enabled="false"></asp:TextBox>
                          <asp:TextBox  ID="txtNoteInterventoALL" runat="server" Width="950px" Height="235px" TextMode="MultiLine" BorderStyle="None" AutoPostBack="false" Enabled="false" Visible="false"></asp:TextBox>
                          </div>
                          </ContentTemplate>
                          </asp:UpdatePanel>
                          </asp:Panel>   
                     </td>
                     <td>
                        <div>
                            <asp:Button ID="btnModificaNoteInterv" runat="server" Text="Modifica Note Intervento" Class="btnstyle2Righe" Enabled="false" />
                        </div>
                        <div style=" height:5px; "></div>
                        <div>
                            <asp:Button ID="btnAggiornaNoteInterv" runat="server" Text="Aggiorna Note Intervento" Class="btnstyle2Righe" Enabled="true" Visible="false"/>
                        </div>
                        <div style=" height:5px; "></div>
                        <div>
                            <asp:Button ID="btnAnnullaModNoteInterv" runat="server" Text="Annulla Modifica Note" Class="btnstyle2Righe" Enabled="false" />
                        </div>
                        </td>
                     </tr>
                     </table>
                     </ContentTemplate>
                     </asp:TabPanel>
                </asp:TabContainer>         
        </td>
        </tr>                     
    </table> 
    </ContentTemplate>
</asp:UpdatePanel> 
</div>