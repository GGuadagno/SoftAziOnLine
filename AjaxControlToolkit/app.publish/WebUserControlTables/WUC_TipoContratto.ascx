<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_TipoContratto.ascx.vb" Inherits="SoftAziOnLine.WUC_TipoContratto" %>
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
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="980px" Height="560px" CssClass ="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <table border="0" cellpadding="0" frame="box" style="width:auto; height:500px; margin-right:0;">
            <tr>
                <td align="left" class="style1" colspan="2">
                    <table style="width: 960px">
                    <tr>
                        <td align="left" class="style1" colspan="2">
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" 
                                AutoPostBack="True" Width="240px">
                            </asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            <asp:TextBox ID="txtRicerca" runat="server" Width="275px"></asp:TextBox>
                            <asp:Button ID="btnRicerca" runat="server"  
                                Text="Avvia ricerca" Width="120px" />
                        </td>
                    </tr>
                    </table> 
                </td>
            </tr>
            <tr>
                <td align="left" style="width:866px; height:230;">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                    <div id="divGridViewPrevT" style="overflow:auto; width:860px; height:210px; border-style:groove;">
                        <asp:SqlDataSource ID="SqlDSTElenco" runat="server" 
                            SelectCommand="SELECT TipoContratto.Codice, TipoContratto.Descrizione, ISNULL(TipoContratto.TipoPagamento,0) AS TipoPagamento, 
                                ISNULL(TipoContratto.TipoScadenza,0) AS TipoScadenza, ISNULL(FineMese,0) AS FineMese, ISNULL(Anticipato,0) AS Anticipato, 
                                ISNULL(TipoContratto.DurataNum,0) AS DurataNum, ISNULL(TipoContratto.NVisite,0) AS NVisite, ISNULL(TipoContratto.CodVisita,'') AS CodVisita,
                                ISNULL(TipoContratto.MeseCS,0) AS MeseCS, ISNULL(TipoContratto.GiornoFisso,0) AS GiornoFisso, TipoContratto.Cod_Causale, 
                                CausMag.Descrizione + ' - ' + TipoContratto.Descrizione AS DesTipoCaus  
                                FROM TipoContratto INNER JOIN CausMag ON TipoContratto.Cod_Causale = CausMag.Codice ORDER BY CausMag.Descrizione, TipoContratto.Descrizione" 
                            SelectCommandType="Text">
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="SqlDSCausMag" runat="server" 
                            SelectCommand="SELECT * FROM [CausMag] WHERE Descrizione LIKE '%CONTR%' ORDER BY [Descrizione]" 
                            SelectCommandType="Text">
                        </asp:SqlDataSource>
                        <asp:GridView ID="GridViewPrevT" runat="server" AllowPaging="True" 
                            AllowSorting="True" AutoGenerateColumns="False" BackColor="Silver" 
                            CssClass="GridViewStyle" DataKeyNames="Codice" DataSourceID="SqlDSTElenco" 
                            EmptyDataText="Nessun dato disponibile." EnableTheming="True" 
                            GridLines="Horizontal" 
                            PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                            PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                            PagerSettings-Mode="NextPreviousFirstLast" 
                            PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                            PagerSettings-Position="Bottom" 
                            PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                            PagerSettings-Visible="true" PagerStyle-HorizontalAlign="Center" PageSize="10">
                            <RowStyle CssClass="RowStyle" />
                            <PagerStyle CssClass="PagerStyle" />
                            <HeaderStyle CssClass="HeaderStyle" />
                            <AlternatingRowStyle CssClass="AltRowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                            <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                                LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                                Mode="NextPreviousFirstLast" 
                                NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="5" InsertVisible="False">
                                    <ItemTemplate>
                                        <asp:Button ID="btnSel" runat="server" CausesValidation="False" 
                                            CommandName="Select" Text="&gt;" />
                                    </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                </asp:TemplateField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Codice" 
                                    HeaderText="Codice" ReadOnly="True" SortExpression="Codice">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="DesTipoCaus" HeaderStyle-Wrap="False" 
                                    HeaderText="Descrizione" ItemStyle-Wrap="true" SortExpression="DesTipoCaus">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle Width="350px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="TipoPagamento" HeaderText="Tipo Pagamento" 
                                    ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="TipoPagamento">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="TipoScadenza" HeaderText="Tipo Scadenza" 
                                    ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="TipoScadenza">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="FineMese" HeaderText="Fine Mese" 
                                    ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="FineMese">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Anticipato" HeaderText="Pagamento Anticipato" 
                                    ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="Anticipato">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="DurataNum" HeaderText="Durata N°" 
                                    ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="DurataNum">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="NVisite" HeaderText="N° Visite" 
                                    ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="NVisite">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="MeseCS" HeaderText="Mese Corrente/Successivo" 
                                    ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="MeseCS">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="GiornoFisso" HeaderText="Giorno fisso" 
                                    ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="GiornoFisso">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                </td>
                <td align="left">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnNuovo" runat="server" class="btnstyle" Text="Nuovo" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnModifica" runat="server" class="btnstyle" Text="Modifica" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnAggiorna" runat="server" class="btnstyle" Text="Aggiorna"  TabIndex="6"/>
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnElimina" runat="server" class="btnstyle" Text="Elimina" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnStampaElenco" runat="server" class="btnstyle" Text="Stampa" Visible="false"/>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td width ="100%" align="left" colspan="2">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                            <asp:Panel ID="PanelDettaglio" style="margin-top: 0px;" runat="server" 
                                groupingtext="Dettaglio" Width="970px" Height="300px">
                                <table style="width:100%">
                                <tr>
                                    <td width ="100%" align="left" style="border-style: outset; border-width: thin">
                                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Width="150px">Codice</asp:Label>
                                        <asp:TextBox ID="txtCodice" runat="server" AutoPostBack="true"
                                            Width="50px" TabIndex="1" MaxLength="5"></asp:TextBox>
                                        <asp:TextBox ID="txtDescrizione" runat="server" AutoPostBack="true"
                                            Width="650px" TabIndex="1" MaxLength="150"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td width ="100%" align="left" style="border-style: outset; border-width: thin">
                                        <div>
                                            <asp:Label ID="Label4" runat="server" Visible="true" Width="150px">Tipo Pagamento</asp:Label>
                                            <asp:DropDownList ID="DDLTipoPagamento" runat="server" Width="400px" TabIndex="2">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Anticipato" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Posticipato" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Già fatturato all'ordine" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Alla Scadenza evasione attività" Value="4"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:CheckBox ID="CheckFineMese" runat="server" Text="Fine mese" Checked="false"/>
                                            <asp:CheckBox ID="CheckAnticipato" runat="server" Text="Pagamento Anticipato" Checked="false"/>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td width ="100%" align="left" style="border-style:none; border-width:thin">
                                        <div>
                                            <asp:Label ID="Label2" runat="server" Visible="true" Width="150px">Tipo Scadenza</asp:Label>
                                            <asp:DropDownList ID="DDLTipoScadenza" runat="server" Width="400px" TabIndex="3">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Data Contratto" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Data Accettazione" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Data Inizio" Value="3"></asp:ListItem>
                                            </asp:DropDownList><br />
                                            
                                        </div>
                                        <div>
                                            <asp:Label ID="Label3" runat="server" Visible="true" Width="150px">Giorno del mese</asp:Label>
                                            <asp:TextBox ID="txtGiornoFisso" runat="server" AutoPostBack="false" MaxLength="2" Width="50px" TabIndex="4"></asp:TextBox>
                                            <asp:DropDownList ID="DDLMeseCS" runat="server" Width="341px" TabIndex="5">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Mese in corso (Data tipo Scadenza)" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Mese successivo (Data tipo Scadenza)" Value="2"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="Label6" runat="server" Font-Bold="true">Nota: valido solo per le scadenze Mensili</asp:Label>
                                        </div>
                                         <div>
                                            <asp:Label ID="Label5" runat="server" Visible="true" Width="150px">Tipo Contratto</asp:Label>
                                            <asp:TextBox ID="txtCodCausale" runat="server" AutoPostBack="True" 
                                                    MaxLength="5" TabIndex="6" Width="50px"></asp:TextBox>
                                            <asp:DropDownList ID="DDLCausali" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" DataSourceID="SqlDSCausMag" DataTextField="Descrizione" 
                                                DataValueField="Codice" Height="22px" TabIndex="7" Width="655px">
                                                <asp:ListItem ></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label9" runat="server" Visible="true" Width="150px">Durata N°</asp:Label>
                                            <asp:TextBox ID="txtDurataNum" runat="server" AutoPostBack="false" 
                                                    MaxLength="5" TabIndex="6" Width="50px"></asp:TextBox>
                                             <asp:Label ID="Label11" runat="server" Font-Bold="true">Nota: dato propositivo, modificabile nella compilazione del contratto</asp:Label>
                                        </div>
                                        <div><br /></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td width ="100%" align="left" style="border-style: outset; border-width: thin">
                                        <div>
                                            <asp:Label ID="Label7" runat="server" Visible="true" Width="150px">N° Visite</asp:Label>
                                            <asp:TextBox ID="txtNVisite" runat="server" AutoPostBack="false" 
                                                    MaxLength="5" TabIndex="6" Width="50px"></asp:TextBox>
                                            <asp:Label ID="Label8" runat="server" Font-Bold="true">Nota: Le visite sono riferite al singolo periodo del contratto</asp:Label>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label10" runat="server" Visible="true" Width="150px">Codice Visita</asp:Label>
                                            <asp:TextBox ID="txtCodVisita" runat="server" AutoPostBack="true"
                                                Width="150px" TabIndex="1" MaxLength="20"></asp:TextBox>
                                            <asp:Label ID="lblDesVisita" runat="server" Font-Bold="false" BorderStyle="Outset" Width="550px" ></asp:Label>
                                        </div>
                                    </td>
                                </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>     
</asp:Panel>