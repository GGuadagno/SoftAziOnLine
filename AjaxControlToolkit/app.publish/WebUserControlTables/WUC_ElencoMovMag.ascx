<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ElencoMovMag.ascx.vb" Inherits="SoftAziOnLine.WUC_ElencoMovMag" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register Src="~/WebUserControl/WFP_ResoDaCliente.ascx" TagName="WFPResoDaCliente" TagPrefix="wuc1" %>
<%@ Register Src="~/WebUserControl/WFP_ResoAFornitore.ascx" TagName="WFPResoAFornitore" TagPrefix="wuc2" %>
<%@ Register Src="~/WebUserControl/WFP_DocCollegati.ascx" TagName="WFPDocCollegati" TagPrefix="wuc6" %>

<style type="text/css">
    .styleTDBTN
        {
            height: 478px;
        }
    .btnstyle1R
        {
            Width: 120px;
            height: 30px;
        margin-left: 0px;
        }  
    .btnstyle3R
        {
            Width: 108px;
            height: 50px;
        margin-left: 0px;
        white-space: pre-wrap;
        }  
    .btnstyle
        {
            Width: 108px;
            height: 40px;
        margin-left: 0px;
        white-space: pre-wrap;      
    }
    .btnstyleSingle
    {
        Width: 108px;
        height: 30px;
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
        width: 1115px;
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
    .style6
    {
        height: 239px;
    }
    </style>    
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="550px" CssClass ="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <wuc1:WFPResoDaCliente ID="WFPResoDaCliente" runat="server" />
    <wuc2:WFPResoAFornitore ID="WFPResoAFornitore" runat="server" />
    <wuc6:WFPDocCollegati ID="WFPDocCollegati" runat="server" />
    <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
               SelectCommand="SELECT Codice, Descrizione FROM Magazzini WHERE Codice>0 ORDER BY Descrizione">
        </asp:SqlDataSource>
    <table border="0" cellpadding="0" frame="box" 
            style="width:auto; height:550px; margin-right:0;">
        </td>
    </tr>
            <tr>
                <td align="left" class="style1" colspan="2">
                    <table>
                    <tr>
                        <td align="left" class="style1">
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" 
                                AutoPostBack="True" Width="145px">
                            </asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            &nbsp;<asp:TextBox ID="txtRicerca" runat="server" Width="200px"></asp:TextBox>&nbsp;<asp:Button ID="btnRicerca" runat="server" Text="Cerca documento" class="btnstyle1R" />
                            &nbsp;<asp:Button ID="btnDocCollegati" runat="server" Text="Doc. Collegati" class="btnstyle1R" />
                            &nbsp;<asp:Label ID="lblMagazzino" runat="server" Height="16px" Font-Bold="true" ForeColor="Blue">Magazzino</asp:Label>
                            <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"  
                                   AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                                   DataTextField="Descrizione" 
                                   DataValueField="Codice" Width="150px">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    </table> 
                </td>
            </tr>
            <tr>
                <td align="left" class="styleBordo">
                    <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="25px">
                        <table>
                        <tr>
                            <td style="width:120px" >
                                <asp:RadioButton ID="rbtnCarichi" runat="server" 
                                    Text="Carichi" AutoPostBack="True" GroupName="Tipo" /></td> 
                            <td style="width:120px">
                                <asp:RadioButton ID="rbtnScarichi" runat="server" 
                                    Text="Scarichi" AutoPostBack="True" GroupName="Tipo" /></td>
                            <td style="width:120px">
                                <asp:RadioButton ID="rbtnMM" runat="server" 
                                    Text="MM Interni" AutoPostBack="True" GroupName="Tipo" /></td>
                            <td style="width:120px"><asp:RadioButton ID="rbtnDocStato5" runat="server" 
                                    Text="In corso" AutoPostBack="True" GroupName="Tipo"/></td>
                            <td style="width:80px"><asp:RadioButton ID="rbtnTutti" runat="server" 
                                    Text="Tutti" AutoPostBack="True" GroupName="Tipo"/></td>
                            <td style="width:350px">
                                <div>
                                <asp:RadioButton ID="rbtnCausale" runat="server" 
                                    Text="Causale" AutoPostBack="True" Visible="true" GroupName="Tipo"/>
                                <asp:DropDownList ID="ddlCausale" runat="server" 
                                AutoPostBack="True"  DataSourceID="SqlDSCausale" DataTextField="Descrizione" 
                                DataValueField="Codice" Width="200px" AppendDataBoundItems="true" >
                                <asp:ListItem Text="" Value="" ></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDSCausale" runat="server" 
                                    SelectCommand="SELECT * FROM [CausMag] ORDER BY [Descrizione]">
                                </asp:SqlDataSource>
                                </div>
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                </td>
                <td align="left" class="style2">
                    <div>
                        <asp:Button ID="btnCambiaStato" runat="server" class="btnstyle" Text="Cambia stato" />
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left" class="style3">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                    <div id="divGridViewPrevT" style="overflow:auto; width:1110px; height:220px; border-style:groove;">
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
                        EnableTheming="True" GridLines="None"
                        DataSourceID="SqlDSPrevTElenco" BackColor="Silver" AllowSorting="True">
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
                        <asp:BoundField DataField="DesCausale" HeaderText="Causale" 
                                SortExpression="DesCausale"><HeaderStyle Wrap="True" /><ItemStyle 
                                Width="15px" /></asp:BoundField> 
                        <asp:BoundField DataField="Magazzino" HeaderText="Magazzino" 
                                SortExpression="Magazzino"><HeaderStyle Wrap="True" /><ItemStyle 
                                Width="05px" /></asp:BoundField>
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
                                <asp:BoundField DataField="Cod_Cliente" HeaderText="Codice Cliente" 
                                    SortExpression="Cod_Cliente">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" 
                                    SortExpression="Rag_Soc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="50px" />
                                </asp:BoundField>
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
                                <asp:BoundField DataField="DataOraConsegna" HeaderText="Data consegna" 
                                    SortExpression="DataOraConsegna">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data_Validita" HeaderText="Data validità" 
                                    SortExpression="Data_Validita">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                                    SortExpression="Riferimento">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="50px" />
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
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="DesStatoDoc" 
                                    HeaderText="Stato" ReadOnly="True" 
                                    SortExpression="DesStatoDoc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="25px" />
                                </asp:BoundField>
                        </Columns>
                       <%-- <HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />--%>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                            SelectCommand="get_MMTElenco" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="ZZ" Name="TipoDoc" SessionField="TipoDoc" Type="String" />
                                <asp:SessionParameter DefaultValue="999" Name="StatoDoc" SessionField="StatoDoc" Type="Int32" />
                                <asp:SessionParameter DefaultValue="D" Name="SortPrevTEl" SessionField="SortPrevTEl" Type="String" />
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
                                <asp:Button ID="btnVisualizza" runat="server" class="btnstyleSingle" Text="Visualizza" Visible="false"/>
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnNuovo" runat="server" class="btnstyleSingle" Text="Nuovo" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnModifica" runat="server" class="btnstyleSingle" Text="Modifica" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnElimina" runat="server" class="btnstyleSingle" Text="Elimina" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnResoClienteFornitore" class="btnstyle" runat="server" Text="Reso da Cliente" Enabled="false" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnSblocca" runat="server" class="btnstyle" Text="Sblocca" />
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
                        DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="30px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="80px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità ordinata" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Ordinata"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità evasa" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Ordinata"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Residua" HeaderText="Quantità residua" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Ordinata"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Cod_IVA" HeaderText="IVA" 
                        SortExpression="Cod_IVA"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo" 
                        SortExpression="Prezzo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoValore" HeaderText="Sconto valore" 
                        SortExpression="ScontoValore"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Sconto_1" HeaderText="Sc.(1)" 
                        SortExpression="Sconto_1"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Importo" HeaderText="Importo" 
                        SortExpression="Importo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoReale" HeaderText="Sc.Riga" 
                        SortExpression="ScontoReale"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
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
                    <div style="height: 15px">
                         <asp:Button ID="btnCopia" runat="server" class="btnstyle" Text="Copia" />
                            </div>
                    <div style="height: 15px">&nbsp;</div>
                    <div style="height: 15px">&nbsp;</div> 
                    <div style="height: 15px; text-align:center"><b>Stampe</b></div> 
                    <div style="height: 5px">&nbsp;</div> 
                    <div>
                        <asp:Button ID="btnStampaNoLotti" runat="server" class="btnstyle3R" Text="Stampa (NO Lotti)" /> </div> 
                    <div style="height: 5px">&nbsp;</div>   
                    <div>
                        <asp:Button ID="btnStampaSiLotti" runat="server" class="btnstyle3R" Text="Stampa (SI Lotti)" /> </div>                   
                    <div style="height: 5px">&nbsp;</div>                     
                    <div style="height: 15px">
                        <asp:Button ID="btnStampaTutti" runat="server" class="btnstyle" Text="Stampa tutti" />
                            </div>
                    <div style="height: 10px">
                            </div>
                </td>
        </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>