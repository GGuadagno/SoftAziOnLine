<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Articoli.ascx.vb" Inherits="SoftAziOnLine.WUC_Articoli" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<%@ Register Src="~/WebUserControlTables/WUC_FornitoriSec.ascx" TagName="FornitoriSec" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControlTables/WUC_PrezziAcquisto.ascx" TagName="PrezziAcquisto" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControlTables/WUC_DescEstesa.ascx" TagName="DescEstesa" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControlTables/WUC_ArticoliCTV.ascx" TagName="ArticoliCTV" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControl/WFP_FornitoreSec.ascx" TagName="WFPFornitoreSec" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControl/WFP_ListiniDaAggiornare.ascx" TagName="WFPListiniDaAggiornare" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControl/WFP_Elenco.ascx" TagName="WFPElenco" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControl/WFP_ElencoCliForn.ascx" TagName="WFPElencoCliForn" TagPrefix="wuc" %>
<%@ Register src="../WebUserControl/WFP_CategorieArt.ascx" tagname="WFP_CategorieArt" tagprefix="wuc" %>
<%@ Register src="../WebUserControl/WFP_LineeArt.ascx" tagname="WFP_LineeArt" tagprefix="wuc" %>
<%@ Register src="../WebUserControl/WFP_TipoCodArt.ascx" tagname="WFP_TipoCodArt" tagprefix="wuc" %>
<%@ Register src="../WebUserControl/WFP_Misure.ascx" tagname="WFP_Misure" tagprefix="wuc" %>
<%@ Register src="../WebUserControl/WFP_Pagamenti.ascx" tagname="WFP_Pagamenti" tagprefix="wuc" %>
<%@ Register src="../WebUserControl/WUC_SceltaStampaAnaArt.ascx" tagname="WUC_SceltaStampaAnaArt" tagprefix="uc2" %>
<%@ Register Src="~/WebUserControlTables/WUC_DistintaBase.ascx" TagName="DistintaBase" TagPrefix="wuc" %>
<style type="text/css">
    .btnstyle
    {
        Width: 101px;
        height: 35px;
        white-space: pre-wrap;      
    }
    .btnstyleDoppio
    {
        Width: 101px;
        height: 55px;
        margin-left: 0px;
        white-space: pre-wrap;   
    }
    .stylePagAltezza
    {
        height: 550px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="980px" Height="600px" BackColor="Silver">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <wuc:WFPElenco ID="WFPElencoAliquotaIVA" runat="server" Tabella="AliquoteIva" Titolo="Elenco Aliquote IVA"/>
    <wuc:WFPElenco ID="WFPElencoPagamenti" runat="server" Tabella="Pagamenti" Titolo="Elenco Pagamenti"/>
    <wuc:WFPElencoCliForn ID="WFPElencoForn" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori" />
    <uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
    <wuc:WFPFornitoreSec ID="WFPFornSec" runat="server" />
    <wuc:WFPListiniDaAggiornare ID="WFPListiniDaAgg" runat="server" />
    <uc2:WUC_SceltaStampaAnaArt ID="WUC_SceltaStampaAnaArt1" runat="server" />
    <wuc:WFP_CategorieArt ID="WFP_CategorieArt1" runat="server" />
    <wuc:WFP_LineeArt ID="WFP_LineeArt1" runat="server" />
    <wuc:WFP_TipoCodArt ID="WFP_TipoCodArt1" runat="server" />
    <wuc:WFP_Misure ID="WFP_Misure1" runat="server" />
    <wuc:WFP_Pagamenti ID="WFP_Pagamenti1" runat="server" />
    <asp:SqlDataSource ID="SqlDataSourceArticoli" runat="server" SelectCommand="SELECT Cod_Articolo, Descrizione FROM AnaMag ORDER BY Cod_Articolo">
       <SelectParameters>
        <asp:SessionParameter DefaultValue="C" Name="SortArticoli" SessionField="SortArticoli" Type="String" />
       </SelectParameters>
    </asp:SqlDataSource>
    <table style="width:auto; height:auto;">
        <tr>
            <td class="stylePagAltezza">
                <div id="divPrincipale" runat="server" >
                <table class="sfondopagine">
                    <tr>
                        <td>
                            Ricerca articolo per:
                            <asp:DropDownList ID="ddlRicerca" AutoPostBack="true" runat="server" Width="210px">
                                <asp:ListItem>Codice</asp:ListItem>
                                <asp:ListItem>Descrizione</asp:ListItem>
                                <asp:ListItem>Descrizione (Parole contenute)</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="txtRicerca" runat="server" Width="390px" BorderStyle="None"></asp:TextBox>&nbsp;
                            <asp:Button ID="btnRicercaArticolo" runat="server" Text="Cerca articolo"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <div id="divGridArticoli" style="overflow: auto; height: 110px; border-style:groove"> 
                                <asp:GridView ID="GridViewArticoli" runat="server" 
                                    GridLines="None" CssClass="GridViewStyle" EnableTheming="True" 
                                    AllowSorting="false" AutoGenerateColumns="false"
                                    EmptyDataText="Nessun dato disponibile."   
                                    DataKeyNames="Cod_Articolo"
                                    AllowPaging="true"
                                    PageSize="10" 
                                    PagerStyle-HorizontalAlign="Center" 
                                    PagerSettings-Mode="NextPreviousFirstLast"
                                    PagerSettings-Visible="true"
                                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                                    DataSourceID="SqlDataSourceArticoli" BackColor="Silver" >
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
                                    <Columns>
                                    <asp:TemplateField InsertVisible="False">
                                    <ItemTemplate><asp:Button ID="Button1" runat="server" 
                                    CausesValidation="False" CommandName="Select" Text="&gt;" />
                                    </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                    </asp:TemplateField>
                                        <asp:BoundField DataField="Cod_Articolo" HeaderText="Codice" 
                                            SortExpression="Cod_Articolo" />
                                        <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" 
                                            SortExpression="Descrizione" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <%--<br />--%>
                        </ContentTemplate>
                        </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <ajaxToolkit:TabContainer ID="Tabs" runat="server" ActiveTabIndex="2" 
                                Width="860px" Height="370px" BackColor="Silver">
                                <ajaxToolkit:TabPanel runat="server" ID="Panel1" HeaderText="Articolo"  BackColor="Silver">
                                    <HeaderTemplate>
                                        Articolo
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <table class="sfondopagine" style="Width:840px;Height:370px;">
                                            <tr>
                                                <td align="Left">
                                                    <asp:Panel ID="panel2" runat="server" BorderWidth="0px" width="800px">
                                                        <table class="sfondopagine" style="Width:830px;Height:370px">
                                                            <tr>
                                                                <td>Codice articolo</td>
                                                                <td colspan="2">
                                                                    <asp:TextBox ID="txtCodBase" runat="server" AutoPostBack="True" MaxLength="20"
                                                                        Width="160px" BorderStyle="None" />&nbsp;
                                                                    <asp:TextBox ID="txtCodOpzione" runat="server" AutoPostBack="True"
                                                                        MaxLength="5" Width="50px" BorderStyle="None"/>&nbsp;
                                                                    <asp:Label ID="lblCodArticolo" runat="server" BorderStyle="Outset"
                                                                        Font-Bold="True" Width="190px" />&nbsp;&nbsp;
                                                                    <asp:Label ID="LblBasOpz" runat="server" Text="Base/Opzione" />&nbsp;
                                                                    <asp:TextBox ID="TxtDefCodBase" runat="server" AutoPostBack="True" MaxLength="2" Width="30px" BorderStyle="None"/>&nbsp;                                                                    
                                                                    <asp:Label ID="LblDefBasOpz" runat="server" BorderStyle="Outset" Font-Bold="True" Width="20px" Text="0" />&nbsp;
                                                                    <asp:Button ID="btnDefBaseOpz" runat="server" Height="22px" Text="M" Width="25px" ToolTip ="Modifica" /> 
                                                                    <asp:Button ID="btnAnnDefBaseOpz" runat="server" Height="22px" Text="X" Width="25px" ToolTip ="Annulla" /> 
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Descrizione articolo</td>
                                                                <td colspan="2">
                                                                    <asp:TextBox ID="txtDescBreve" runat="server" AutoPostBack="True" MaxLength="150"
                                                                        Width="650px" Rows="2" TextMode="MultiLine" BorderStyle="None"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="3">
                                                                    <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="2" 
                                                                        Width="830px" Height="180px" BackColor="Silver">
                                                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanel5" HeaderText="DescrizioneEstesa"  BackColor="Silver">
                                                                            <HeaderTemplate>Descrizione estesa</HeaderTemplate>
                                                                            <ContentTemplate>
                                                                                <wuc:DescEstesa ID="GWDescEstesa" runat="server" />
                                                                            </ContentTemplate>
                                                                        </ajaxToolkit:TabPanel>
                                                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanel6" HeaderText="CodiciTipoValore"  BackColor="Silver">
                                                                            <HeaderTemplate>Codice Tipo Valore per XML</HeaderTemplate>
                                                                            <ContentTemplate>
                                                                                <wuc:ArticoliCTV ID="GWArticoliCTV" runat="server" />
                                                                            </ContentTemplate>
                                                                        </ajaxToolkit:TabPanel>
                                                                    </ajaxToolkit:TabContainer>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="Label3" runat="server" Text="Linea Prodotto"
                                                                        Font-Bold="True"/>
                                                                    <asp:Button ID="btnTipoCodBarre" runat="server" CausesValidation="False" CommandName="btnTipoCodBarre" 
                                                                        Text="+" ToolTip="Gestione Linea Prodotto"/>
                                                                </td>
                                                                <td colspan="2">
                                                                    <asp:DropDownList ID="ddlTipoCodBarre" runat="server"
                                                                        AppendDataBoundItems="True" AutoPostBack="True" 
                                                                        DataSourceID="SqlDataSourceCodBarre" DataTextField="Tipo_Codice" 
                                                                        DataValueField="Tipo_Codice" Width="100px">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:SqlDataSource ID="SqlDataSourceCodBarre" runat="server"
                                                                        SelectCommand="SELECT Tipo_Codice, Descrizione FROM TipiCodBar ORDER BY Descrizione">
                                                                    </asp:SqlDataSource>
                                                                    <asp:Label ID="lblTipoCodBarre" runat="server" BorderStyle="Outset"
                                                                        Font-Bold="True" Width="250px" />&nbsp;
                                                                    Codice barre
                                                                    &nbsp;<asp:TextBox ID="txtCodBarre" runat="server" MaxLength="20" Width="170px" BorderStyle="None"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td><asp:CheckBox ID="checkArticoloDiVendita" runat="server" Text="Articolo vendita" /></td>
                                                                <td><asp:CheckBox ID="checkGestioneInLotti" runat="server" Text="Gestione lotti"/></td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel runat="server" ID="TabPanel1" HeaderText="Caratteristiche commerciali">
                                    <HeaderTemplate>
                                        Caratteristiche commerciali
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <table class="sfondopagine" style="Width:840px;Height:370px;">
                                            <tr>
                                                <td align="center">
                                                    <asp:Panel ID="panel4" runat="server" BorderWidth="0px" width="700px">
                                                        <table class="sfondopagine" style="Width:750px;Height:370px;">
                                                            <tr>
                                                                <td style="width:120px">
                                                                    Categoria</td>
                                                                <td>
                                                                    <asp:Button ID="btnTrovaCategoria" runat="server" Height="22px" Text="?"
                                                                        Visible="False" Width="30px" />                                                                                                       
                                                                    <asp:TextBox ID="txtCodCategoria" runat="server" AutoPostBack="true"
                                                                        Width="80px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:Button ID="btnCategoriaArt" runat="server" CausesValidation="False" CommandName="btnCategoriaArt" 
                                                                        Text="+" ToolTip="Gestione anagrafiche Categorie"/> 
                                                                </td> 
                                                                <td colspan="2">
                                                                    <asp:DropDownList ID="ddlCatgoria" runat="server" AppendDataBoundItems="true"
                                                                        AutoPostBack="true" DataSourceID="SqlDataSourceCategoria" 
                                                                        DataTextField="Descrizione" DataValueField="Codice" Width="400px">
                                                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <asp:SqlDataSource ID="SqlDataSourceCategoria" runat="server"
                                                                    SelectCommand="SELECT Codice, Descrizione FROM CategArt ORDER BY Descrizione">
                                                                </asp:SqlDataSource>
                                                            </tr>
                                                            <tr>
                                                                <td>Linea</td>
                                                                <td>
                                                                    <asp:Button ID="btnTrovaLinea" runat="server" Height="22px" Text="?"
                                                                        Visible="False" Width="30px" />
                                                                    <asp:TextBox ID="txtCodLinea" runat="server" AutoPostBack="true" Width="80px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:Button ID="btnLinea" runat="server" CausesValidation="False" CommandName="btnLinea" 
                                                                        Text="+" ToolTip="Gestione anagrafiche Linee"/>                                                                     
                                                                </td>
                                                                <td colspan="2">
                                                                    <asp:DropDownList ID="ddlLinea" runat="server" AppendDataBoundItems="true"
                                                                        AutoPostBack="true" DataSourceID="SqlDataSourceLinea" 
                                                                        DataTextField="Descrizione" DataValueField="Codice" Width="400px">
                                                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <asp:SqlDataSource ID="SqlDataSourceLinea" runat="server"
                                                                    SelectCommand="SELECT Codice, Descrizione FROM LineeArt ORDER BY Descrizione" />
                                                            </tr>
                                                            <tr>
                                                                <td>Unità di misura</td>
                                                                <td>
                                                                    <asp:Button ID="btnTrovaUnitaMisura" runat="server" Height="22px" Text="?"
                                                                        Visible="False" Width="30px" />
                                                                    <asp:TextBox ID="txtCodUnitaMisura" runat="server" AutoPostBack="true"
                                                                        Width="80px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:Button ID="btnMisure" runat="server" CausesValidation="False" CommandName="btnMisure" 
                                                                        Text="+" ToolTip="Gestione anagrafiche Misure"/>                                                                         
                                                                </td>
                                                                <td colspan="2">
                                                                    <asp:DropDownList ID="ddlUnitaMisura" runat="server"
                                                                        AppendDataBoundItems="true" AutoPostBack="true" DataSourceID="SqlDataSourceUM" 
                                                                        DataTextField="Descrizione" DataValueField="Codice" Width="400px">
                                                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <asp:SqlDataSource ID="SqlDataSourceUM" runat="server"
                                                                    SelectCommand="SELECT Codice, Descrizione FROM Misure ORDER BY Descrizione" />
                                                            </tr>
                                                            <tr>
                                                                <td>Prezzo acquisto</td>
                                                                <td><asp:TextBox ID="txtUltPrezzoAcq" runat="server" AutoPostBack="false"
                                                                        Width="120px" BorderStyle="None"></asp:TextBox></td>
                                                                <td style="text-align:right">Ricarico %</td>
                                                                <td>
                                                                    <asp:TextBox ID="txtRicarico" runat="server" AutoPostBack="false" Width="100px" BorderStyle="None"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Prezzo di listino</td>
                                                                <td><asp:TextBox ID="txtPrezzoVendita" runat="server" AutoPostBack="false"
                                                                        Width="120px" BorderStyle="None"></asp:TextBox></td>
                                                                <td style="text-align:right">Prezzo minimo</td>
                                                                <td>
                                                                    <asp:TextBox ID="txtPrezzoMinVen" runat="server" AutoPostBack="false" 
                                                                        Width="100px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:Label ID="lblMessListVendD" runat="server" Text="" Font-Bold="True"/>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Sc.Cliente</td>
                                                                <td><asp:TextBox ID="txtSconto1Perc" runat="server" AutoPostBack="false"
                                                                        Width="120px" BorderStyle="None"></asp:TextBox></td>
                                                                <td style="text-align:right">Sc.Fornitore</td>
                                                                <td>
                                                                    <asp:TextBox ID="txtSconto2Perc" runat="server" AutoPostBack="false" 
                                                                        Width="100px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:CheckBox ID="chkOkListino" runat="server" Text="Inserisci Articolo/Prezzi nel Listino" Checked="False" AutoPostBack="true" Visible="false" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" rowspan="2" title=" (Nota: solo il Tipo Articolo uguale ad Articolo movimeta la giacenza mentre, gli altri tipi No)">
                                                                    <table>
                                                                        <tr><td><asp:Label ID="Label1" runat="server" Text="Tipo di articolo" Font-Bold="true"/></td></tr>
                                                                        <tr><td><asp:RadioButton ID="radioArticolo" runat="server" AutoPostBack="False"
                                                                                GroupName="TipoArticolo" Text="Articolo"/></td></tr>
                                                                        <tr><td><asp:RadioButton ID="radioKit" runat="server" AutoPostBack="False"
                                                                                GroupName="TipoArticolo" Text="Kit"/></td></tr>
                                                                        <tr><td><asp:RadioButton ID="radioImballo" runat="server" AutoPostBack="False"
                                                                                GroupName="TipoArticolo" Text="Imballo"/></td></tr>
                                                                        <tr><td><asp:RadioButton ID="radioBancale" runat="server" AutoPostBack="False"
                                                                                GroupName="TipoArticolo" Text="Bancale"/></td></tr>
                                                                        <tr><td><asp:RadioButton ID="RadioVoceGenerica" runat="server" AutoPostBack="False"
                                                                                GroupName="TipoArticolo" Text="Voce generica per interventi vari" /></td></tr>
                                                                    </table>
                                                                </td>
                                                                <td style="text-align:right">Confezione da</td>
                                                                <td>
                                                                    <asp:TextBox ID="txtConfezioneDa" runat="server" AutoPostBack="false" 
                                                                        Width="100px" BorderStyle="None"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="text-align:right">Aliquota IVA</td>
                                                                <td>
                                                                    <asp:Button ID="btnTrovaAliquotaIva" runat="server" Height="22px" Text="?"
                                                                        Width="30px" />
                                                                    <asp:TextBox ID="txtCodAliquotaIva" runat="server" AutoPostBack="true"
                                                                        Width="50px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:DropDownList ID="ddlAliquotaIva" runat="server"
                                                                        AppendDataBoundItems="true" AutoPostBack="true" 
                                                                        DataSourceID="SqlDataSourceAliquotaIVA" DataTextField="Descrizione" 
                                                                        DataValueField="Aliquota" Width="250px">
                                                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:SqlDataSource ID="SqlDataSourceAliquotaIva" runat="server"
                                                                        SelectCommand="SELECT Aliquota, Descrizione FROM Aliquote_Iva ORDER BY Descrizione">
                                                                    </asp:SqlDataSource>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel runat="server" ID="TabPanel2" HeaderText="Caratteristiche fisiche / N° Anni scadenze">
                                    <HeaderTemplate>
                                        Caratteristiche fisiche / N° Anni scadenze e Allegati
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <table class="sfondopagine" style="Width:840px;Height:370px">
                                            <tr style="Height:10%">
                                                <td style="text-align:right">
                                                    Peso espresso in Kg.</td>
                                                <td>
                                                    <asp:TextBox ID="txtPeso" runat="server" AutoPostBack="false" Width="70px" BorderStyle="None"></asp:TextBox>
                                                </td>
                                                <td style="text-align:right">
                                                    Altezza</td>
                                                <td style="text-align:left">
                                                    <asp:TextBox ID="txtAltezza" runat="server" AutoPostBack="false" Width="70px" BorderStyle="None"></asp:TextBox>
                                                </td>
                                                <td style="text-align:right">
                                                    Lunghezza</td>
                                                <td>
                                                    <asp:TextBox ID="txtLunghezza" runat="server" AutoPostBack="false" Width="70px" BorderStyle="None"></asp:TextBox>
                                                </td>
                                                <td>
                                                    Larghezza</td>
                                                <td style="text-align:left">
                                                    <asp:TextBox ID="txtLarghezza" runat="server" AutoPostBack="false" Width="70px" BorderStyle="None"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr style="Height:10%">
                                                <td style="text-align:right">
                                                    N° anni garanzia del prodotto</td>
                                                <td>
                                                    <asp:TextBox ID="txtNAnniGaranzia" runat="server" AutoPostBack="false" Width="70px" BorderStyle="None"></asp:TextBox>
                                                </td>
                                                <td style="text-align:right">
                                                    N° anni scadenza elettrodi</td>
                                                <td style="text-align:left">
                                                    <asp:TextBox ID="txtNAnniScadElettrodi" runat="server" AutoPostBack="false" Width="70px" BorderStyle="None"></asp:TextBox>
                                                </td>
                                                <td style="text-align:right">
                                                    N° anni scadenza batterie</td>
                                                <td>
                                                    <asp:TextBox ID="txtNAnniScadBatterie" runat="server" AutoPostBack="false" Width="70px" BorderStyle="None"></asp:TextBox>
                                                </td>
                                                <td>&nbsp;</td>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr style="Height:10%">
                                                <td style="text-align:right">
                                                    <asp:CheckBox ID="chkID1HS1" runat="server" Text="Allegato HS1" Checked="False" AutoPostBack="false" />
                                                </td>
                                                <td>&nbsp;</td>
                                                <td style="text-align:right">
                                                    <asp:CheckBox ID="chkID2FR2" runat="server" Text="Allegato FR2" Checked="False" AutoPostBack="false" />
                                                </td>
                                                <td>&nbsp;</td>
                                                <td style="text-align:right">
                                                    <asp:CheckBox ID="chkID3FR3" runat="server" Text="Allegato FR3" Checked="False" AutoPostBack="false" />
                                                </td>
                                                <td>&nbsp;</td>
                                                <td style="text-align:right">
                                                    <asp:CheckBox ID="chkID4FRX" runat="server" Text="Allegato FRX" Checked="False" AutoPostBack="false" />
                                                </td>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr style="Height:60%">
                                                <td>&nbsp;</td>
                                                <td>&nbsp;</td>
                                                <td>&nbsp;</td>
                                                <td>&nbsp;</td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel runat="server" ID="TabPanel3" HeaderText="Fornitori">
                                    <HeaderTemplate>
                                        Fornitori
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <table class="sfondopagine" style="Width:840px;Height:370px">
                                            <asp:SqlDataSource ID="SqlDataSourceFornitore" runat="server"
                                                SelectCommand="SELECT Codice_CoGe, Rag_Soc, Titolare, Riferimento FROM Fornitori ORDER BY Rag_Soc" />
                                            <tr>
                                                <td>Fornitore preferenziale</td>
                                                <td colspan="2">
                                                    <asp:Button ID="btnTrovaFornitore" runat="server" Height="22px" Text="?"
                                                        Width="30px" />&nbsp;
                                                    <asp:TextBox ID="txtCodFornitore" runat="server" AutoPostBack="true"
                                                        Width="100px" BorderStyle="None"></asp:TextBox>&nbsp;
                                                    <asp:DropDownList ID="ddlFornitore" runat="server" AppendDataBoundItems="true"
                                                        AutoPostBack="true" DataSourceID="SqlDataSourceFornitore" 
                                                        DataTextField="Rag_Soc" DataValueField="Codice_CoGe" Width="490px">
                                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Titolare</td>
                                                <td colspan="2">
                                                    <asp:Label ID="lbTitolare" runat="server" BorderStyle="Outset" Enabled="false" 
                                                        Width="640px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Riferimento</td>
                                                <td colspan="2">
                                                    <asp:Label ID="lbRiferimento" runat="server" BorderStyle="Outset" 
                                                        Enabled="false" Width="640px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Codice articolo fornitore</td>
                                                <td>
                                                    <asp:TextBox ID="txtCodArtFornitore" runat="server" AutoPostBack="False" 
                                                        Width="370px" BorderStyle="None"></asp:TextBox>
                                                </td>
                                                <td rowspan="4">
                                                    <table>
                                                        <tr>
                                                            <td>Ultimi prezzi di acquisto</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <wuc:PrezziAcquisto ID="GWPrezziAcquisto" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Condizioni pagamento</td>
                                                <td colspan="2">
                                                    <asp:Button ID="btnTrovaCondizioniPag" runat="server" Height="22px" Text="?"
                                                        Width="30px" />&nbsp;
                                                    <asp:TextBox ID="txtCodCondizioniPag" runat="server" AutoPostBack="true"
                                                        Width="60px" BorderStyle="None"></asp:TextBox>&nbsp;
                                                    <asp:Button ID="btnPagamenti" runat="server" CausesValidation="False" CommandName="btnPagamenti" 
                                                                        Text="+" ToolTip="Condizioni di pagamento" Visible="false" />                                                                        
                                                    <asp:DropDownList ID="ddlCondizioniPag" runat="server"
                                                        AppendDataBoundItems="true" AutoPostBack="true" 
                                                        DataSourceID="SqlDataSourceCondizioniPag" DataTextField="Descrizione" 
                                                        DataValueField="Codice" Width="220px">
                                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <asp:SqlDataSource ID="SqlDataSourceCondizioniPag" runat="server"
                                                    SelectCommand="SELECT Codice, Descrizione FROM Pagamenti ORDER BY Descrizione">
                                                </asp:SqlDataSource>
                                            </tr>
                                            <tr>
                                                <td>Quantità sottoscorta</td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="txtQtaSottoscorta" runat="server" AutoPostBack="false" 
                                                        Width="140px" BorderStyle="None"></asp:TextBox>
                                                    &nbsp;<asp:CheckBox ID="checkSottoscorta" runat="server" Text="Avvisa se sottoscorta" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Quantità riordino</td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="txtQtaRiordino" runat="server" AutoPostBack="false" 
                                                        Width="140px" BorderStyle="None"></asp:TextBox>
                                                    Giorni di consegna
                                                    &nbsp;<asp:TextBox ID="txtGiorniConsegna" runat="server" AutoPostBack="false"
                                                        Width="40px" BorderStyle="None"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td colspan="3">
                                                    Fornitori secondari</td>
                                            </tr>
                                            <tr>
                                                <td colspan="3">
                                                    <wuc:FornitoriSec ID="GWFornitoriSec" runat="server" />
                                                    <div style="position:relative;top:-80px;left:577px">
                                                        <asp:Button ID="btnAggiungiFornSec" runat="server" Height="22px"
                                                            Text="Aggiungi" Width="70px" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel runat="server" ID="TabPanel4" HeaderText="Distinta base"  BackColor="Silver">
                                    <HeaderTemplate>
                                        Distinta base
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <table class="sfondopagine" style="Width:840px;Height:370px;">
                                            <tr>
                                                <td align="Left">
                                                    <asp:Panel ID="panel3" runat="server" BorderWidth="0px" width="800px">
                                                        <table class="sfondopagine" style="Width:830px;Height:370px">
                                                            <tr>
                                                                <td colspan="3">Elenco componenti</td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="3">
                                                                    <wuc:DistintaBase ID="DistintaBase1" runat="server" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                            </ajaxToolkit:TabContainer>
                        </td>
                    </tr>
                </table>
                </div>
            </td>
            <div id="divBottoni" runat="server" >
            <td class="stylePagAltezza" align="left">
                <div id="noradio" >
                <asp:Button ID="btnNuovo" runat="server" Text="Nuovo" class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnModifica" runat="server" Text="Modifica"  class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnAggiorna" runat="server" Text="Aggiorna"  class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnAnnulla" runat="server" Text="Annulla"  class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnElimina" runat="server" Text="Elimina"  class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnStampa" runat="server" Text="Stampa"  class="btnstyle"/>
                </div>
                <div style="height: 15px">
                </div>
                <div>
                    <asp:Button ID="btnStampaArtFor" runat="server" class="btnstyleDoppio" Text="Stampa articoli fornitori"/>
                </div>
                <div style="height: 15px"></div>
            </td>
            </div>
        </tr>
    </table>
    <div>
        <asp:Label ID="Label2" runat="server" Width="10px" />
        <asp:Label ID="lblCArtSel" runat="server" BorderStyle="Outset" Font-Bold="True" Width="190px" />
        <asp:Label ID="lblDArtSel" runat="server" BorderStyle="Outset" Font-Bold="True" Width="600px" />
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>