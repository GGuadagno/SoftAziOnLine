<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_CausaliMagazzino.ascx.vb" Inherits="SoftAziOnLine.WUC_CausaliMagazzino" %>
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
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="560px" CssClass ="sfondopagine">
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
                <td align="left" style="width:1120px; height:230;">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                    <div id="divGridViewPrevT" style="overflow:auto; width:1110px; height:210px; border-style:groove;">
                        <asp:SqlDataSource ID="SqlDSTElenco" runat="server" 
                            SelectCommand="SELECT * FROM CausMag ORDER BY Codice" 
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
                                <asp:BoundField DataField="Descrizione" HeaderStyle-Wrap="False" 
                                    HeaderText="Descrizione" ItemStyle-Wrap="False" SortExpression="Descrizione">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Tipo" HeaderText="Tipo" 
                                    ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="Tipo">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Segno_Giacenza" HeaderText="Segno Giacenza" 
                                    ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="Segno_Giacenza">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Segno_Lotti" HeaderText="Segno Lotti" 
                                    ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="Segno_Lotti">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Fatturabile" ItemStyle-HorizontalAlign="Center" 
                                    SortExpression="Fatturabile">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkFatturabile" runat="server" 
                                            Checked='<%# Bind("Fatturabile") %>' Enabled="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Movimento il magazzino" ItemStyle-HorizontalAlign="Center" 
                                    SortExpression="Movimento">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkMovimento" runat="server" 
                                            Checked='<%# Bind("Movimento") %>' Enabled="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Movimento fra magazzino" ItemStyle-HorizontalAlign="Center" 
                                    SortExpression="MovMag">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkMovMag" runat="server" 
                                            Checked='<%# Bind("Movimento_Magazzini") %>' Enabled="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Segno_Prodotto" HeaderText="Segno Prodotto" 
                                    ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="Segno_Prodotto">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Segno_Confezionato" HeaderText="Segno Confezionato" 
                                    ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="Segno_Confezionato">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Segno_Ordinato" HeaderText="Segno Ordinato" 
                                    ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="Segno_Ordinato">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Segno_Venduto" HeaderText="Segno Venduto" 
                                    ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="Segno_Venduto">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Segno_Deposito" HeaderText="Segno Deposito" 
                                    ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="Segno_Deposito">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Center" Width="10px" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Causale Vendita" 
                                    ItemStyle-HorizontalAlign="Center" SortExpression="CausVend">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkCausVend" runat="server" Checked='<%# Bind("CausVend") %>' 
                                            Enabled="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Costo Venduto" 
                                    ItemStyle-HorizontalAlign="Center" SortExpression="CausCostoVenduto">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CausCostoVenduto" runat="server" 
                                            Checked='<%# Bind("CausCostoVenduto") %>' Enabled="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="C/Visione" ItemStyle-HorizontalAlign="Center" 
                                    SortExpression="CVisione">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CVisione" runat="server" Checked='<%# Bind("CVisione") %>' 
                                            Enabled="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="C/Deposito" ItemStyle-HorizontalAlign="Center" 
                                    SortExpression="CDeposito">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CDeposito" runat="server" Checked='<%# Bind("CDeposito") %>' 
                                            Enabled="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Causale Reso" ItemStyle-HorizontalAlign="Center" 
                                    SortExpression="Reso">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="Reso" runat="server" Checked='<%# Bind("Reso") %>' 
                                            Enabled="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="PrezzoAL" HeaderText="Prezzo Acq./Lis." 
                                    ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" 
                                    SortExpression="PrezzoAL">
                                    <HeaderStyle Wrap="true" />
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Center" Width="10px" />
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
                            <div id="noradio">
                                <asp:Button ID="btnNuovo" runat="server" class="btnstyle" Text="Nuovo" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnModifica" runat="server" class="btnstyle" Text="Modifica" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnAggiorna" runat="server" class="btnstyle" Text="Aggiorna" />
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
                            <asp:Panel ID="PanelDettaglio" style="margin-top: 0px;" runat="server" groupingtext="Dettaglio" Width="1120px" Height="300px" >
                                <table style="width:100%;height:250px">
                                <tr>
                                    <td colspan="2" width ="100%" align="left" style="border-style: outset; border-width: thin">
                                        <asp:Label ID="Label1" runat="server" Font-Bold="true">Causale</asp:Label>
                                        <asp:TextBox ID="txtCodice" runat="server" AutoPostBack="true"
                                            Width="50px" TabIndex="1" MaxLength="5"></asp:TextBox>
                                        <asp:TextBox ID="txtDescrizione" runat="server" AutoPostBack="true"
                                            Width="450px" TabIndex="1" MaxLength="50"></asp:TextBox>
                                        <asp:Label ID="Label7" runat="server" Width="10px"></asp:Label>
                                        <asp:Label ID="Label8" runat="server" Font-Bold="true">Tipo:</asp:Label>
                                        <asp:RadioButton ID="rbtnCliente" runat="server" Text="Cliente" AutoPostBack="False" GroupName="Tipo"/>
                                        <asp:RadioButton ID="rbtnFornitore" runat="server" Text="Fornitore" AutoPostBack="False" GroupName="Tipo"/>
                                        <asp:RadioButton ID="rbtnNeutro" runat="server" Text="Neutro" AutoPostBack="False" GroupName="Tipo"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" width ="100%" align="left">
                                        <asp:CheckBox id="chkFatturabile" AutoPostBack="false" Checked="false" runat="server" Text="Fatturabile" />
                                        <asp:Label ID="Label9" runat="server" Width="10px"></asp:Label>
                                        <asp:CheckBox id="ChkMovimento" AutoPostBack="false" Checked="false" runat="server" Text="Movimento il magazzino" />
                                        <asp:Label ID="Label11" runat="server" Width="10px"></asp:Label>
                                        <asp:CheckBox id="chkCausCostoVenduto" AutoPostBack="false" Checked="false" runat="server" Text="Costo del venduto" />
                                        <asp:Label ID="Label12" runat="server" Width="10px"></asp:Label>
                                        <asp:Label ID="Label26" runat="server" Font-Bold="true">Causale:</asp:Label>
                                        <asp:RadioButton ID="rbtnCausVend" runat="server" Text="Vendita" AutoPostBack="False" GroupName="CausVendReso"/>
                                        <asp:RadioButton ID="rbtnReso" runat="server" Text="Reso" AutoPostBack="False" GroupName="CausVendReso"/>
                                        <asp:RadioButton ID="rbtnVendResoNo" runat="server" Text="No" AutoPostBack="False" GroupName="CausVendReso"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" width="100%" align="left" >
                                        <asp:Label ID="Label13" runat="server" Font-Bold="true">Vendita per C/?:</asp:Label>
                                        <asp:RadioButton ID="rbtnCVisione" runat="server" Text="C/Visione" AutoPostBack="False" GroupName="CVCDN"/>
                                        <asp:RadioButton ID="rbtnCDeposito" runat="server" Text="C/Deposito" AutoPostBack="False" GroupName="CVCDN"/>
                                        <asp:RadioButton ID="rbtnCNessuna" runat="server" Text="Nessuno" AutoPostBack="False" GroupName="CVCDN"/>
                                        <asp:Label ID="Label15" runat="server" Width="10px"></asp:Label>
                                        <asp:Label ID="Label14" runat="server" Font-Bold="true">Prezzi da proporre nell'emissione documenti:</asp:Label>
                                        <asp:RadioButton ID="rbtnPrezzoListino" runat="server" Text="Listino" AutoPostBack="False" GroupName="PLPA"/>
                                        <asp:RadioButton ID="rbtnPrezzoAcquisto" runat="server" Text="Acquisto" AutoPostBack="False" GroupName="PLPA"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" width="100%" align="left">
                                        <asp:Label ID="Label10" runat="server" Font-Bold="true">Azione su Giacenza+Lotti:</asp:Label>
                                        <asp:RadioButton ID="rbtnSegnoGiacenzaP" runat="server" Text="+" AutoPostBack="False" GroupName="SG"/>
                                        <asp:RadioButton ID="rbtnSegnoGiacenzaM" runat="server" Text="-" AutoPostBack="False" GroupName="SG"/>
                                        <asp:RadioButton ID="rbtnSegnoGiacenzaN" runat="server" Text="N" AutoPostBack="False" GroupName="SG"/>
                                        <asp:Label ID="Label16" runat="server" Width="10px"></asp:Label>
                                        <asp:Label ID="Label17" runat="server" Font-Bold="true">Azione su Produzione:</asp:Label>
                                        <asp:RadioButton ID="rbtnSegnoProdP" runat="server" Text="+" AutoPostBack="False" GroupName="SP"/>
                                        <asp:RadioButton ID="rbtnSegnoProdM" runat="server" Text="-" AutoPostBack="False" GroupName="SP"/>
                                        <asp:RadioButton ID="rbtnSegnoProdN" runat="server" Text="N" AutoPostBack="False" GroupName="SP"/>
                                        <asp:Label ID="Label18" runat="server" Width="10px"></asp:Label>
                                        <asp:Label ID="Label19" runat="server" Font-Bold="true">Azione su Confezionato:</asp:Label>
                                        <asp:RadioButton ID="rbtnSegnoConfP" runat="server" Text="+" AutoPostBack="False" GroupName="SC"/>
                                        <asp:RadioButton ID="rbtnSegnoConfM" runat="server" Text="-" AutoPostBack="False" GroupName="SC"/>
                                        <asp:RadioButton ID="rbtnSegnoConfN" runat="server" Text="N" AutoPostBack="False" GroupName="SC"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" width="100%" align="left" >
                                        <asp:Label ID="Label20" runat="server" Font-Bold="true">Azione su Ordinato:</asp:Label>
                                        <asp:Label ID="Label2" runat="server" Width="40px"></asp:Label>
                                        <asp:RadioButton ID="rbtnSegnoOrdP" runat="server" Text="+" AutoPostBack="False" GroupName="SO"/>
                                        <asp:RadioButton ID="rbtnSegnoOrdM" runat="server" Text="-" AutoPostBack="False" GroupName="SO"/>
                                        <asp:RadioButton ID="rbtnSegnoOrdN" runat="server" Text="N" AutoPostBack="False" GroupName="SO"/>
                                        <asp:Label ID="Label21" runat="server" Width="10px"></asp:Label>
                                        <asp:Label ID="Label22" runat="server" Font-Bold="true">Azione su Venduto:</asp:Label>
                                        <asp:Label ID="Label3" runat="server" Width="13px"></asp:Label>
                                        <asp:RadioButton ID="rbtnSegnoVendP" runat="server" Text="+" AutoPostBack="False" GroupName="SV"/>
                                        <asp:RadioButton ID="rbtnSegnoVendM" runat="server" Text="-" AutoPostBack="False" GroupName="SV"/>
                                        <asp:RadioButton ID="rbtnSegnoVendN" runat="server" Text="N" AutoPostBack="False" GroupName="SV"/>
                                        <asp:Label ID="Label23" runat="server" Width="10px"></asp:Label>
                                        <asp:Label ID="Label24" runat="server" Font-Bold="true">Azione su Depositi:</asp:Label>
                                        <asp:Label ID="Label5" runat="server" Width="27px"></asp:Label>
                                        <asp:RadioButton ID="rbtnSegnoDepP" runat="server" Text="+" AutoPostBack="False" GroupName="SD"/>
                                        <asp:RadioButton ID="rbtnSegnoDepM" runat="server" Text="-" AutoPostBack="False" GroupName="SD"/>
                                        <asp:RadioButton ID="rbtnSegnoDepN" runat="server" Text="N" AutoPostBack="False" GroupName="SD"/>
                                        <asp:Label ID="Label25" runat="server" Width="10px"></asp:Label>
                                        <%--<asp:CheckBox id="chkPassword" AutoPostBack="false" Checked="false" runat="server" Text="Richiesta password" />--%>
                                    </td>
                                </tr>
                                <tr>
                                    <td width ="50%" align="left" style="border-style: outset; border-width: thin">
                                        <asp:CheckBox id="chkDistintaBase" AutoPostBack="true" Checked="false" runat="server" Text="Opera su distinta base:" Font-Bold="true" Visible="true" />
                                    </td>
                                    <td width ="50%" align="left" style="border-style: outset; border-width: thin">
                                        <asp:CheckBox id="chkMovMag" AutoPostBack="true" Checked="false" runat="server" Text="Movimento fra magazzini:" Font-Bold="true" ForeColor="Blue" Visible="true"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td width ="50%" align="left">
                                        <asp:Label ID="Label4" runat="server" Visible="true" Width="200px">Causale indotta componenti</asp:Label>
                                        <asp:DropDownList ID="DDLCausaleIndottaC" runat="server" AppendDataBoundItems="true"
                                            AutoPostBack="true" DataSourceID="SqlDSCausMagC" 
                                            DataTextField="Descrizione" 
                                            DataValueField="Codice" Width="300px" TabIndex="2" Enabled="false" Visible="true">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="SqlDSCausMagC" runat="server"
                                            SelectCommand="SELECT * FROM CausMag ORDER BY Descrizione">
                                        </asp:SqlDataSource>
                                    </td>
                                    <td width ="50%" align="left">
                                        <asp:Label ID="Label6" runat="server" Visible="true" ForeColor="Blue" Width="150px">Causale Magazzino 2</asp:Label>
                                        <asp:DropDownList ID="DDLCausaleMag2" runat="server" AppendDataBoundItems="true"
                                            AutoPostBack="true" DataSourceID="SqlDSCausMagM2" 
                                            DataTextField="Descrizione" 
                                            DataValueField="Codice" Width="350px" TabIndex="2" Enabled="false" Visible="true">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="SqlDSCausMagM2" runat="server"
                                            SelectCommand="SELECT * FROM CausMag ORDER BY Descrizione">
                                        </asp:SqlDataSource>
                                    </td>
                                </tr>
                                <tr>
                                    <td width ="50%" align="left">
                                        <asp:Label ID="Label27" runat="server" Visible="true" Width="200px">Causale indotta codice "Padre"</asp:Label>
                                        <asp:DropDownList ID="DDLCausaleIndottaP" runat="server" AppendDataBoundItems="true"
                                            AutoPostBack="true" DataSourceID="SqlDSCausMagP" 
                                            DataTextField="Descrizione" 
                                            DataValueField="Codice" Width="300px" TabIndex="2" Enabled="false" Visible="true">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="SqlDSCausMagP" runat="server"
                                            SelectCommand="SELECT * FROM CausMag ORDER BY Descrizione">
                                        </asp:SqlDataSource>
                                    </td>
                                    <td width ="50%" align="left">
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