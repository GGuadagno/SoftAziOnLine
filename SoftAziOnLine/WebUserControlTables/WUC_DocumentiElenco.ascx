<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_DocumentiElenco.ascx.vb" Inherits="SoftAziOnLine.WUC_DocumentiElenco" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register Src="~/WebUserControl/WFP_NotaCreditoCliente.ascx" TagName="WFPNotaCreditoCliente" TagPrefix="wuc1" %>
<%@ Register Src="~/WebUserControl/WFP_DocCollegati.ascx" TagName="WFPDocCollegati" TagPrefix="wuc2" %>
<%@ Register src="../WebUserControl/WUC_Attesa.ascx" tagname="Attesa" tagprefix="uc3" %>
<%@ Register src="../WebUserControl/WFP_EtichettePrepara.ascx" TagName="WFPETP" TagPrefix="wfp1" %>
<style type="text/css">
    .btnstyle1RL
        {
            Width: 140px;
            height: 30px;
        margin-left: 0px;
        }  
    .styleTDBTN
        {
            height: 478px;
        }
    .btnstyle2R
    {
        Width: 108px;
        height: 40px;
    margin-left: 0px;
    white-space: pre-wrap;        
    }
    .btnstyle1R
    {
        Width: 108px;
        height: 30px;
    margin-left: 0px;
    white-space: pre-wrap;        
    }
    .btnstyle1R25
    {
        Width: 108px;
        height: 20px;
    margin-left: 0px;
    white-space: pre-wrap;        
    }
    .styleMenu
    {
        width: auto;
        border-style:groove;
    }
    .style1
    {
        height: 35px;
        }
    .styleBordo
    {
        height: 35px;
        width: 860px;
        border-style:groove;
    }
    .style2
    {
        height: 35px;
    }
    .style3
    {
        width: auto;
        border-style: none;
        height: 185px;
    }
    .style5
    {
        width: auto;
        border-style: none;
        height: 80px;
    }
    .style6
    {
        height: 239px;
    }
    .style7
    {
        height: 185px;
    }
</style>    
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="550px" CssClass="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <wuc1:WFPNotaCreditoCliente ID="WFPNotaCreditoCliente" runat="server" />
    <wuc2:WFPDocCollegati ID="WFPDocCollegati" runat="server" />
    <uc3:Attesa ID="Attesa" runat="server" />
    <wfp1:WFPETP ID="WFPETP" runat="server" />
    <asp:SqlDataSource ID="SqlDSTipoFatt" runat="server" 
            SelectCommand="SELECT * FROM [TipoFatt] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
    <table border="0" cellpadding="0" frame="box" style="width:auto; height:550px; margin-right:0;">
            <tr>
                <td  width ="100%" align="left" colspan="2" >
                    <table style="width:1230px">
                    <tr>
                        <td>
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" 
                                AutoPostBack="True" Width="160px">
                            </asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            &nbsp;<asp:TextBox ID="txtRicerca" runat="server" Width="250px"></asp:TextBox>&nbsp;<asp:Button ID="btnRicerca" runat="server"  
                                Text="Cerca documento" class="btnstyle1RL" />
                            &nbsp;<asp:Button ID="btnDocCollegati" runat="server"  
                                Text="Documenti Collegati" class="btnstyle1RL" />
                            <asp:Label ID="Label1" runat="server" BorderColor="White"
                                BorderStyle="None" Font-Bold="True" ForeColor="Black" Width="40px"></asp:Label>
                            <asp:Label ID="lblEsercizio" runat="server" BorderColor="White"
                                BorderStyle="Double" Font-Bold="True" ForeColor="Black">Esercizio</asp:Label>
                            <asp:DropDownList ID="ddL_Esercizi" runat="server" Height="22px" Width="60px"  
                                DataSourceID="SqlDataSource2" DataTextField="Esercizio" 
                                DataValueField="Esercizio">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                                SelectCommand="SELECT [Ditta], [Esercizio] FROM [Esercizi] WHERE ([Ditta] = @Ditta) ORDER BY Esercizio DESC">
                                <SelectParameters>
                                    <asp:SessionParameter DefaultValue="00" Name="Ditta" SessionField="CodiceDitta" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    </table> 
                </td>
            </tr>
            <tr>
                <td  width ="100%" align="left" colspan="2">
                    <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="50px" BorderStyle="Groove">
                    <table style="width:1230px">
                        <tr>
                            <td style="width:280px" >
                                <div>
                                    <asp:RadioButton ID="rbtnFC" runat="server" ToolTip="Fattura commerciale"
                                    Text="FC" AutoPostBack="True" GroupName="Tipo"/>
                                    <asp:RadioButton ID="rbtnFA" runat="server" ToolTip="Fattura accompagnatoria"
                                    Text="FA" AutoPostBack="True" GroupName="Tipo" Visible="False" />
                                    <asp:RadioButton ID="rbtnFCPA" runat="server" ToolTip="Fattura PA"
                                    Text="FCPA" AutoPostBack="True" GroupName="Tipo" Visible="True" />
                                    <asp:RadioButton ID="rbtnNC" runat="server" ToolTip="Note di Credito" 
                                    Text="NC" AutoPostBack="True" GroupName="Tipo" />
                                    <asp:RadioButton ID="rbtnNCPA" runat="server" ToolTip="Note di Credito PA" 
                                    Text="NCPA" AutoPostBack="True" GroupName="Tipo" />
                                    <asp:Label ID="Label2" runat="server" Text="" Width="50px" ></asp:Label>
                                    <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" AutoPostBack="True" GroupName="Tipo"/>
                                </div>
                            </td> 
                            <td style="width:220px">
                                <div>
                                <asp:RadioButton ID="rbtnTipoFT" runat="server" Text="Fatturazione" AutoPostBack="True" Visible="true" GroupName="Tipo"/>
                                <asp:DropDownList ID="ddlTipoFattur" runat="server" AutoPostBack="True"  DataSourceID="SqlDSTipoFatt" DataTextField="Descrizione" DataValueField="Codice" Width="150px">
                                    <asp:ListItem Text="" Value="" ></asp:ListItem>
                                </asp:DropDownList>
                                </div>
                            </td>
                            <td style="width:100px">
                                <asp:RadioButton ID="rbtnDTInLista" runat="server" Text="DDT in Lista" AutoPostBack="True" ForeColor="Blue" GroupName="Tipo" />
                            </td>
                            <td style="width:350px">
                                <div>
                                    <asp:RadioButton ID="rbtnDTDaInviare" runat="server" Text="Spedizioni DDT" AutoPostBack="True" ForeColor="Blue" GroupName="Tipo" />
                                    <asp:Label ID="lblDal" runat="server" Font-Bold="false" ForeColor="Blue" Text="dal"></asp:Label>
                                    <asp:TextBox ID="txtDalN" runat="server" MaxLength="10" TabIndex="3" Width="70px" Enabled="false" BorderStyle="None" ></asp:TextBox>
                                    <asp:Label ID="lblAl" runat="server" Font-Bold="false" ForeColor="blue" Text="al" ></asp:Label>
                                    <asp:TextBox ID="txtAlN" runat="server" MaxLength="10" TabIndex="4" Width="70px" Enabled="false" BorderStyle="None"></asp:TextBox>
                                    <asp:Button ID="btnCercaDDT" runat="server" Text="Cerca DDT" ForeColor="Blue" Visible="true" Enabled="false"/>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:280px">
                                <div>
                                    <asp:RadioButton ID="rbtnDT" runat="server" Text="DDT Clienti (Tutti)" AutoPostBack="True" GroupName="Tipo" />&nbsp;
                                    <asp:RadioButton ID="rbtnDTNoFatt" runat="server" Text="DDT Clienti da fatturare" AutoPostBack="True" GroupName="Tipo" />
                                </div>
                            </td>
                            <td style="width:220px">
                                <asp:RadioButton ID="rbtnNONFatt" runat="server" Text="DDT NON Fatturabili" AutoPostBack="True" GroupName="Tipo" />
                            </td>
                            <td style="width:100px">
                                <asp:RadioButton ID="rbtnDTFOR" runat="server" Text="DDT Fornitori" AutoPostBack="True" GroupName="Tipo" />
                            </td>
                            <td style="width:350px">
                                <div>
                                    <asp:RadioButton ID="rbtnCVisione" runat="server" Text="DDT C/Visione" AutoPostBack="True" GroupName="Tipo" />&nbsp;
                                    <asp:RadioButton ID="rbtnCDeposito" runat="server" Text="DDT C/Deposito" AutoPostBack="True" GroupName="Tipo" />
                                    <asp:Label ID="Label3" runat="server" Text="" Width="10px" ></asp:Label>
                                    <asp:Button ID="btnGeneraSpedDDT" runat="server" Text="Genera Sped. DDT" ForeColor="Blue" Visible="true" Enabled="false"/>
                                    <a ID="lnkSpedDDT" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri File CSV/XLS">
                                        <img id="Img" runat="server" alt="" src="../Immagini/Icone/expand.jpg" /></a>
                                </div>
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td align="left" class="style3">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                    <div id="divGridViewPrevT" style="overflow:auto; width:1110px; height:195px; border-style:groove;">
                        <asp:GridView ID="GridViewPrevT" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="IDDocumenti" 
                        AllowPaging="true"
                        PageSize="10" 
                        PagerStyle-HorizontalAlign="Center" 
                        PagerSettings-Mode="NextPreviousFirstLast"
                        PagerSettings-Visible="true"
                        PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                        PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                        PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                        PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                        EnableTheming="True" GridLines="None" DataSourceID="SqlDSPrevTElenco" BackColor="Silver" AllowSorting="True" PagerSettings-Position="Bottom">
                        <RowStyle CssClass="RowStyle" />
                        <PagerStyle CssClass="PagerStyle" />
                        <HeaderStyle CssClass="HeaderStyle" />
                        <AlternatingRowStyle CssClass="AltRowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle"/>
                        <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                            LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                            Mode="NextPreviousFirstLast" 
                            NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                            PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                        <Columns><asp:TemplateField InsertVisible="False">
                        <ItemTemplate><asp:Button ID="Button1" runat="server" 
                        CausesValidation="False" CommandName="Select" Text="&gt;" />
                        </ItemTemplate>
                        <controlstyle font-size="XX-Small" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Tipo_Doc" HeaderText="Tipo" 
                        SortExpression="Tipo_Doc"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="DesStatoDoc" HeaderText="Stato" 
                                SortExpression="DesStatoDoc"><HeaderStyle Wrap="True" /><ItemStyle 
                                Width="15px" /></asp:BoundField> 
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                                    DataFormatString="{0:d}" HeaderText="Numero" ReadOnly="True" 
                                    SortExpression="Numero">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data_Doc" HeaderText="Data" 
                                    SortExpression="Data_Doc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="DataOraConsegna" HeaderText="Data consegna" 
                                    SortExpression="DataOraConsegna">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Cod_Cliente" HeaderText="Codice Cliente" 
                                    SortExpression="Cod_Cliente">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" SortExpression="Rag_Soc"><HeaderStyle Wrap="True" /><ItemStyle Width="250px" /></asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Denominazione" 
                                    HeaderText="Denominazione" ReadOnly="True" 
                                    SortExpression="Denominazione">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                                    HeaderText="Località" ReadOnly="True" 
                                    SortExpression="Localita">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" 
                                    HeaderText="CAP" ReadOnly="True" 
                                    SortExpression="CAP">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Partita_IVA" 
                                    HeaderText="Partita IVA" ReadOnly="True" 
                                    SortExpression="Partita_IVA">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Codice_Fiscale" 
                                    HeaderText="Codice Fiscale" ReadOnly="True" 
                                    SortExpression="Codice_Fiscale">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                                    SortExpression="Riferimento">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="50px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data_Riferimento" HeaderText="Data Rif." 
                                    SortExpression="Data_Riferimento">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Cod_Agente" 
                                    DataFormatString="{0:d}" HeaderText="C/Ag." ReadOnly="True" 
                                    SortExpression="Cod_Agente">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione1" 
                                    HeaderText="Destinazione(1)" ReadOnly="True" 
                                    SortExpression="Destinazione1">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="25px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione2" 
                                    HeaderText="Destinazione(2)" ReadOnly="True" 
                                    SortExpression="Destinazione2">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="25px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione3" 
                                    HeaderText="Destinazione(3)" ReadOnly="True" 
                                    SortExpression="Destinazione3">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="25px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Acconto" 
                                    HeaderText="PL" ReadOnly="True"  
                                    SortExpression="Acconto">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="RefDataDDT" 
                                    HeaderText="Spedito" ReadOnly="True"  
                                    SortExpression="RefDataDDT">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Vettore_1" 
                                    HeaderText="V1" ReadOnly="True"  
                                    SortExpression="Vettore_1">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Tipo_Spedizione" 
                                    HeaderText="TSp" ReadOnly="True"  
                                    SortExpression="Tipo_Spedizione">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                        </Columns>
                        <%--<HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />--%>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                            SelectCommand="get_DocTElenco" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="ZZ" Name="TipoDoc" SessionField="TipoDoc" Type="String" />
                                <asp:SessionParameter DefaultValue="0" Name="StatoDoc" SessionField="StatoDoc" Type="Int32" />
                                <asp:SessionParameter DefaultValue="N" Name="SortPrevTEl" SessionField="SortPrevTEl" Type="String" />
                                <asp:SessionParameter DefaultValue="0" Name="DAL" SessionField="DAL" Type="Int32" />
                                <asp:SessionParameter DefaultValue="0" Name="AL" SessionField="AL" Type="Int32" />
                                <asp:SessionParameter DefaultValue="0" Name="Vettore1" SessionField="Vettore1" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                </td>
                <td align="left">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnVisualizza" runat="server" class="btnstyle1R" Text="Visualizza" Visible="false"/>
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnNuovo" runat="server" class="btnstyle1R" Text="Nuovo" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnModifica" runat="server" class="btnstyle1R" Text="Modifica" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnElimina" runat="server" class="btnstyle1R" Text="Elimina" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnCreaFattura" runat="server" class="btnstyle1R" Text="Fattura" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnCambiaStato" runat="server" class="btnstyle2R" Text="Cambia stato" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnSblocca" runat="server" class="btnstyle2R" Text="Sblocca" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        <tr>
            <td align="left">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                <div id="divGridViewPrevD" style="overflow:auto; width:1110px; height:230px; border-style:groove;">
                    <asp:GridView ID="GridViewPrevD" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato selezionato."  
                        DataKeyNames="IDDocumenti" 
                        EnableTheming="True" GridLines="None" 
                        DataSourceID="SqlDSPrevDByIDDocumenti"><AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo">
                        <HeaderStyle Wrap="false" Width="20px" />
                        <ItemStyle Wrap="false" Width="20px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" /><ItemStyle Width="200px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità ordinata" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Ordinata"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità evasa" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Evasa"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Residua" HeaderText="Quantità residua" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Residua"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Allestita" HeaderText="Quantità inviata" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Allestita"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Cod_IVA" HeaderText="IVA" 
                        SortExpression="Cod_IVA"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo" 
                        SortExpression="Prezzo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="TipoScontoMerce" HeaderText="SM OM" 
                        SortExpression="TipoScontoMerce"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoValore" HeaderText="Sconto valore" 
                        SortExpression="ScontoValore"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Sconto_1" HeaderText="Sc.(1)" 
                        SortExpression="Sconto_1"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Importo" HeaderText="Importo" 
                        SortExpression="Importo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="DedPerAcconto" HeaderText="Ded." 
                        SortExpression="DedPerAcconto"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoReale" HeaderText="Sc.Riga" 
                        SortExpression="ScontoReale"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Pro_Agente" HeaderText="Provv." 
                        SortExpression="Pro_Agente"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="ImportoProvvigione" HeaderText="Imp.Provv." 
                        SortExpression="ImportoProvvigione"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        </Columns>
                        <HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
                    <asp:SqlDataSource ID="SqlDSPrevDByIDDocumenti" runat="server" 
                        SelectCommand="get_PrevDByIDDocumenti" 
                        SelectCommandType="StoredProcedure">
                        <SelectParameters>
                            <asp:SessionParameter DefaultValue="0" Name="IDDocumenti" SessionField="IDDocumenti" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    </div>
            </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td align="left" class="style6">
                    <div>
                        <asp:Button ID="btnSiLista" runat="server" class="btnstyle1R" Text="Si Lista" ForeColor="Blue" Visible="false"/>
                    </div>
                    <div style="height: 5px"></div>
                    <div>
                        <asp:Button ID="btnNoLista" runat="server" class="btnstyle1R" Text="No Lista" ForeColor="Blue" Visible="false"/>
                    </div>
                    <div style="height: 5px"></div>
                    <div>
                        <asp:Button ID="btnSpedOK" runat="server" class="btnstyle1R" Text="Già spedito" ForeColor="Blue" Visible="false"/>
                    </div>
                    <div style="height: 5px"></div>
                    <div>
                        <asp:Button ID="btnSpedNO" runat="server" class="btnstyle1R" Text="Non spedito" ForeColor="Blue" Visible="false"/>
                    </div>
                    <div style="height: 5px"></div>
                    <div>
                        <asp:Button ID="btnResoClienteFornitore" runat="server" class="btnstyle2R" Text="Reso da Cliente" Enabled="false" />
                    </div>
                    <div style="height: 5px">&nbsp;</div> 
                    <div style="height: 15px">
                         <asp:Button ID="btnCopia" runat="server" class="btnstyle1R" Text="Copia" />
                    </div>
                    <div style="height: 20px">&nbsp;</div>                  
                    <div style="height: 15px; text-align:center">
                                <asp:Label ID="lblStampe" runat="server" BorderColor="White"
                                BorderStyle="None" Font-Bold="True" ForeColor="Black">Stampe</asp:Label>
                            </div> 
                    <div style="height: 5px">&nbsp;</div> 
                    <div style="height: 15px">
                        <asp:Button ID="btnStampa" runat="server" class="btnstyle1R" Text="Documento" />
                    </div>  
                    <div style="height:15px">&nbsp;</div>
                    <div>
                        <a ID="LnkStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Documento">Apri Documento</a>
                    </div>
                    <div style="height: 10px">&nbsp;</div>
                    <div style="height: 15px;">                        
                        <asp:Button ID="btnStampaEti" runat="server" class="btnstyle2R" Text="Etichette" />
                    </div>
                    <div style="height:10px"></div>
                </td>
        </tr>
</table>
    </ContentTemplate>
 </asp:UpdatePanel> 
</asp:Panel>