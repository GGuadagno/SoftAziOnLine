<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Listini.ascx.vb" Inherits="SoftAziOnLine.WUC_Listini" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="WUC_ListiniDuplica.ascx" tagname="WUC_ListiniDuplica" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc3" %>
<%@ Register src="../WebUserControl/WUC_SceltaOrdinamentoRiepListino.ascx" tagname="WUC_SceltaOrdinamentoRiepListino" tagprefix="uc4" %>
<style type="text/css">
        .btnstyle
        {
            Width: 108px;
            height: 35px;
            white-space: pre-wrap;      
        }
        .style23
        {
            height: 480px;
        }
        .style22
        {
            width: 960px;
        }
        .styleLblTB0
        {
            height: 30px;
            width: 120px;
        }
         .styleTxtCodTB0
        {
            height: 30px;
            width: 120px;
        }  
        .styleLblTB1
        {
            height: 30px;
            width: 100px;
        }
         .styleTxtCodTB1
        {
            height: 30px;
            width: 120px;
        }
        .style25
        {
            width: 120px;
        }
        .style28
        {
            height: 30px;
            width: 35px;
        }
        .style31
        {
            width: 59px;
        }
</style>
<br />
<%--<input type="hidden" id="SWModificato" runat="server" name="SWModificato" />--%>
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" 
    Width="980px" Height="550px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanelPrincipale" runat="server"><ContentTemplate>
        <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc3:WFP_ElencoCliForn ID="WFPElencoCli" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
        <uc4:WUC_SceltaOrdinamentoRiepListino ID="WUC_SceltaOrdinamentoRiepListino1" 
            runat="server" />
        <table style="width:auto; height:auto;">
            <tr>
                <td class="style23">
                    <asp:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" 
                        AutoPostBack="True" Height="500px" style="margin-top: 0px" Width="860px" 
                        BackColor="Silver">
                        <asp:TabPanel ID="TabPanel1" runat="server" HeaderText="Selezione listino">
                    <ContentTemplate>
                        <asp:Panel ID="PanelListini" runat="server" Height="540px"><asp:UpdatePanel 
                            ID="UpdatePanel1" runat="server"><ContentTemplate><asp:SqlDataSource 
                                ID="SqlDSListini" runat="server" DeleteCommand="[Delete_ListVenT]" 
                                DeleteCommandType="StoredProcedure" InsertCommand="InsertUpdate_ListVenT" 
                                InsertCommandType="StoredProcedure" ProviderName="System.Data.SqlClient" 
                                SelectCommand="get_Listini" SelectCommandType="StoredProcedure" 
                                UpdateCommand="InsertUpdate_ListVenT" UpdateCommandType="StoredProcedure"><DeleteParameters><asp:Parameter 
                                    Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" /><asp:Parameter 
                                    Name="Codice" Type="Int32" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Codice" Type="Int32" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                                <asp:Parameter Name="Tipo" Type="String" />
                                <asp:Parameter Name="Data_Inizio_Validita" Type="DateTime" />
                                <asp:Parameter Name="Abilitato" Type="Boolean" />
                                <asp:Parameter Name="Valuta" Type="String" />
                                <asp:Parameter Name="Cod_Pagamento" Type="Int32" />
                                <asp:Parameter Name="Categoria" Type="Int32" />
                                <asp:Parameter Name="Cliente" Type="String" />
                                <asp:Parameter Name="Note" Type="String" />
                                <asp:Parameter Direction="InputOutput" Name="RetVal" Type="Int32" />
                            </UpdateParameters>
                            <InsertParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Codice" Type="Int32" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                                <asp:Parameter Name="Tipo" Type="String" />
                                <asp:Parameter Name="Data_Inizio_Validita" Type="DateTime" />
                                <asp:Parameter Name="Abilitato" Type="Boolean" />
                                <asp:Parameter Name="Valuta" Type="String" />
                                <asp:Parameter Name="Cod_Pagamento" Type="Int32" />
                                <asp:Parameter Name="Categoria" Type="Int32" />
                                <asp:Parameter Name="Cliente" Type="String" />
                                <asp:Parameter Name="Note" Type="String" />
                                <asp:Parameter Direction="InputOutput" Name="RetVal" Type="Int32" />
                            </InsertParameters>
                            </asp:SqlDataSource><table style="width:100%;"><tr><td colspan="4">
                            <div id="divGridViewListini" style="overflow: auto; height: 235px; border-style:groove; background-color: Silver;">
                                    <asp:GridView ID="GridViewListini" runat="server" 
                                    AutoGenerateColumns="False" CssClass="GridViewStyle" 
                                    EmptyDataText="Nessun dato disponibile."  
                                    DataKeyNames="Codice" 
                                    DataSourceID="SqlDSListini" EnableTheming="False" GridLines="None"><AlternatingRowStyle 
                                    CssClass="AltRowStyle" /><Columns><asp:TemplateField InsertVisible="False"><ItemTemplate><asp:Button 
                                        ID="Button1" runat="server" CausesValidation="False" CommandName="Select" 
                                        Text="&gt;" />
                                    </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                    </asp:TemplateField><asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="Codice" DataFormatString="{0:d}" HeaderText="Codice" ReadOnly="True" 
                                        SortExpression="Codice"><HeaderStyle Wrap="True" /><ItemStyle 
                                        Width="10px" /></asp:BoundField><asp:BoundField DataField="Descrizione" 
                                        HeaderText="Descrizione" SortExpression="Descrizione" /><asp:BoundField 
                                        DataField="Tipo" HeaderText="Tipo" SortExpression="Tipo" /><asp:BoundField 
                                        DataField="Data_Inizio_Validita" HeaderText="Valido dal" 
                                        SortExpression="Data_Inizio_Validita" /><asp:CheckBoxField 
                                        DataField="Abilitato" HeaderText="Abilitato" SortExpression="Abilitato"><ItemStyle 
                                        HorizontalAlign="Center" VerticalAlign="Middle" Width="40px" /></asp:CheckBoxField><asp:BoundField 
                                        DataField="Valuta" HeaderText="Valuta" SortExpression="Valuta" /><asp:BoundField 
                                        DataField="Note" HeaderText="Note" SortExpression="Note" />
                                </Columns>
                                <HeaderStyle CssClass="HeaderStyle" />
                                <PagerSettings Mode="NextPrevious" Visible="False" />
                                <PagerStyle CssClass="PagerStyle" />
                                <RowStyle CssClass="RowStyle" />
                                <SelectedRowStyle CssClass="SelectedRowStyle" />
                                </asp:GridView></div></td></tr><tr><td bgcolor="Silver" class="style25" colspan="4"></td></tr><tr>
                                    <td bgcolor="Silver" class="styleLblTB0" colspan="1">Codice</td>
                                    <td bgcolor="Silver" class="styleTxtCodTB0" colspan="1"><asp:TextBox ID="txtCodice" 
                                            runat="server" AutoPostBack="True" MaxLength="5" Width="100px"></asp:TextBox></td>
                                    <td bgcolor="Silver" class="style32" colspan="2">Descrizione&#160;&#160;&#160;<asp:TextBox 
                                            ID="txtDescrizione" runat="server" AutoPostBack="True" MaxLength="50" 
                                            TabIndex="1" Width="400px"></asp:TextBox></td></tr><tr>
                                        <td bgcolor="Silver" class="styleLblTB0">Tipo</td><td bgcolor="Silver" 
                                        class="styleTxtCodTB0"><asp:DropDownList ID="DDLTipoLis" runat="server" 
                                        AppendDataBoundItems="True" AutoPostBack="True" Height="22px" TabIndex="2" 
                                        Width="100px"><asp:ListItem Text="" Value="0"></asp:ListItem></asp:DropDownList></td>
                                        <td bgcolor="Silver" class="style33">Inizio validità&#160;<asp:TextBox ID="txtDataVal" 
                                                runat="server" MaxLength="10" TabIndex="3" Width="80px"></asp:TextBox><asp:ImageButton 
                                                ID="imgBtnShowCalendar" runat="server" 
                                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                ToolTip="apri il calendario" /><asp:CalendarExtender 
                                                ID="txtDataVal_CalendarExtender" runat="server" Enabled="True" 
                                                Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendar" 
                                                TargetControlID="txtDataVal"></asp:CalendarExtender><asp:RegularExpressionValidator 
                                                ID="DateRegexValidator" runat="server" ControlToValidate="txtDataVal" 
                                                ErrorMessage="data invalida" 
                                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" /></td>
                                        <td bgcolor="Silver" class="style28"><asp:CheckBox ID="ChkAbilitato" runat="server" 
                                                AutoPostBack="True" Text="Abilitato" /></td></tr><tr>
                                        <td bgcolor="Silver" class="styleLblTB0">Valuta</td><td bgcolor="Silver" 
                                        class="styleTxtCodTB0"><asp:TextBox ID="txtValuta" runat="server" 
                                        AutoPostBack="True" MaxLength="4" TabIndex="4" Width="100px"></asp:TextBox></td><td 
                                        bgcolor="Silver" class="style32" colspan="2"><asp:DropDownList ID="DDLValuta" 
                                            runat="server" AppendDataBoundItems="True" AutoPostBack="True" 
                                            DataSourceID="SqlDSValuta" DataTextField="Descrizione" DataValueField="Codice" 
                                            Height="22px" TabIndex="5" Width="450px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList><asp:SqlDataSource 
                                            ID="SqlDSValuta" runat="server" 
                                            SelectCommand="SELECT Codice, [Descrizione] FROM [Valute] ORDER BY [Descrizione]"></asp:SqlDataSource></td></tr><tr>
                                        <td bgcolor="Silver" class="styleLblTB0">Pagamento</td><td bgcolor="Silver" 
                                        class="styleTxtCodTB0"><asp:TextBox ID="txtPagamento" runat="server" 
                                        AutoPostBack="True" MaxLength="5" TabIndex="6" Width="100px"></asp:TextBox></td><td 
                                        bgcolor="Silver" class="style33"><asp:DropDownList ID="DDLPagamento" 
                                            runat="server" AppendDataBoundItems="True" AutoPostBack="True" 
                                            DataSourceID="SqlDSPagamenti" DataTextField="Descrizione" 
                                            DataValueField="Codice" Height="22px" TabIndex="7" Width="450px"><asp:ListItem 
                                            Text="" Value=""></asp:ListItem></asp:DropDownList><asp:SqlDataSource 
                                            ID="SqlDSPagamenti" runat="server" 
                                            SelectCommand="SELECT [Codice], [Descrizione] FROM [Pagamenti] ORDER BY [Descrizione]"></asp:SqlDataSource></td>
                                        <td bgcolor="Silver" class="style28"><asp:CheckBox ID="ChkPagamento" runat="server" 
                                                AutoPostBack="True" Text="Nessuno" /></td></tr><tr>
                                        <td bgcolor="Silver" class="styleLblTB0">Categoria</td><td bgcolor="Silver" 
                                        class="styleTxtCodTB0"><asp:TextBox ID="txtCategoria" runat="server" 
                                        AutoPostBack="True" MaxLength="5" TabIndex="8" Width="100px"></asp:TextBox></td><td 
                                        bgcolor="Silver" class="style33"><asp:DropDownList ID="DDLCategoria" 
                                            runat="server" AppendDataBoundItems="True" AutoPostBack="True" 
                                            DataSourceID="SqlDSCategorie" DataTextField="Descrizione" 
                                            DataValueField="Codice" Height="22px" TabIndex="9" Width="450px"><asp:ListItem 
                                            Text="" Value=""></asp:ListItem></asp:DropDownList><asp:SqlDataSource 
                                            ID="SqlDSCategorie" runat="server" 
                                            SelectCommand="SELECT [Codice], [Descrizione] FROM [Categorie] ORDER BY [Descrizione]"></asp:SqlDataSource></td>
                                        <td bgcolor="Silver" class="style28"><asp:CheckBox ID="ChkCategoria" runat="server" 
                                                AutoPostBack="True" Text="Nessuna" /></td></tr><tr>
                                        <td bgcolor="Silver" class="styleLblTB0">Cliente
                                        <asp:Button ID="btnCercaAnagrafica" runat="server" CausesValidation="False" CommandName="btnCercaAnagrafica" 
                                                Text="?" ToolTip="Ricerca anagrafiche" />
                                        </td><td bgcolor="Silver" 
                                        class="styleTxtCodTB0">
                                        <asp:Label ID="lblCodCliFor" runat="server" 
                                         BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="100px"></asp:Label>
                                         </td><td 
                                        bgcolor="Silver">
                                        <asp:Label ID="lblRagSoc" runat="server" BorderColor="White" 
                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="510px">Ragione Sociale</asp:Label>
                                        </td>
                                        <td bgcolor="Silver" class="style28"><asp:CheckBox ID="ChkClienti" runat="server" 
                                                AutoPostBack="True" Text="Nessuno" /></td></tr><tr>
                                        <td bgcolor="Silver" class="styleLblTB0" colspan="1">Note</td><td 
                                        bgcolor="Silver" class="style31" colspan="3"><asp:TextBox ID="txtNote" 
                                        runat="server" AutoPostBack="True" MaxLength="50" TabIndex="12" Width="665px"></asp:TextBox></td></tr></table>
                        </ContentTemplate>
                        </asp:UpdatePanel></asp:Panel>
    </ContentTemplate>
</asp:TabPanel>
                        <asp:TabPanel ID="TabPanel2" runat="server" 
                            HeaderText="Articoli presenti nel listino">
                <ContentTemplate>
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server"><ContentTemplate>
                        <asp:Panel ID="PanelArtLisRicerca" runat="server" Height="25px">Ordinamento e ricerca per:&#160; <asp:DropDownList 
                            ID="ddlRicerca" runat="server" AutoPostBack="True" Width="240px"></asp:DropDownList>&#160;&#160; <asp:TextBox 
                            ID="txtRicerca" runat="server" Width="250px"></asp:TextBox>&#160;&#160;<asp:Button 
                            ID="btnRicercaArticolo" runat="server" Text="Cerca articolo" /></asp:Panel>
                    </ContentTemplate>
                    <triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnRicercaArticolo" EventName="Click" />
                    </triggers>
                    </asp:UpdatePanel>
                    <asp:Panel ID="PanelArtLis" runat="server" Height="440px">
                    <asp:SqlDataSource 
                            ID="SqlDSArtLis" runat="server" DeleteCommand="[Delete_ListVenD]" 
                            DeleteCommandType="StoredProcedure" InsertCommand="InsertUpdate_ListVenD" 
                            InsertCommandType="StoredProcedure" SelectCommand="get_ListVenDByCodListino" 
                            SelectCommandType="StoredProcedure" UpdateCommand="InsertUpdate_ListVenD" 
                            UpdateCommandType="StoredProcedure"><SelectParameters><asp:SessionParameter 
                                DefaultValue="1" Name="CodLis" SessionField="IDListino" Type="Int32" /><asp:SessionParameter 
                                DefaultValue="C" Name="SortListVenD" SessionField="SortListVenD" 
                                Type="String" />
                        </SelectParameters>
                        <DeleteParameters>
                            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                            <asp:Parameter Name="CodLis" Type="Int32" />
                            <asp:Parameter Name="Cod_Articolo" Type="String" />
                        </DeleteParameters>
                        <UpdateParameters>
                            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                            <asp:Parameter Name="CodLis" Type="Int32" />
                            <asp:Parameter Name="Cod_Articolo" Type="String" />
                            <asp:Parameter Name="Prezzo" Type="Decimal" />
                            <asp:Parameter Name="Prezzo_Valuta" Type="Decimal" />
                            <asp:Parameter Name="Sconto_1" Type="Decimal" />
                            <asp:Parameter Name="Sconto_2" Type="Decimal" />
                            <asp:Parameter Name="PrezzoMinimo" Type="Decimal" />
                            <asp:Parameter Name="Data_Cambio" Type="DateTime" />
                            <asp:Parameter Name="Cambio" Type="Decimal" />
                            <asp:Parameter Direction="InputOutput" Name="RetVal" Type="Int32" />
                        </UpdateParameters>
                        <InsertParameters>
                            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                            <asp:Parameter Name="CodLis" Type="Int32" />
                            <asp:Parameter Name="Cod_Articolo" Type="String" />
                            <asp:Parameter Name="Prezzo" Type="Decimal" />
                            <asp:Parameter Name="Prezzo_Valuta" Type="Decimal" />
                            <asp:Parameter Name="Sconto_1" Type="Decimal" />
                            <asp:Parameter Name="Sconto_2" Type="Decimal" />
                            <asp:Parameter Name="PrezzoMinimo" Type="Decimal" />
                            <asp:Parameter Name="Data_Cambio" Type="DateTime" />
                            <asp:Parameter Name="Cambio" Type="Decimal" />
                            <asp:Parameter Direction="InputOutput" Name="RetVal" Type="Int32" />
                        </InsertParameters>
                        </asp:SqlDataSource><table style="width:100%;"><tr>
                        <td colspan="4"><div id="divGridArtLis" 
                                style="overflow: auto; height: 345px; border-style:groove; background-color: Silver;">
                                <asp:GridView ID="GridViewArtLis" runat="server" 
                                AutoGenerateColumns="False" CssClass="GridViewStyle" 
                                EmptyDataText="Nessun dato disponibile."  
                                AllowPaging="true"
                                    PageSize="12" 
                                    PagerStyle-HorizontalAlign="Center" 
                                    PagerSettings-Mode="NextPreviousFirstLast"
                                    PagerSettings-Visible="true"
                                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                                DataKeyNames="Cod_Articolo" DataSourceID="SqlDSArtLis" EnableTheming="False" 
                                GridLines="None"><AlternatingRowStyle CssClass="AltRowStyle" /><Columns><asp:TemplateField 
                                    InsertVisible="False"><ItemTemplate><asp:Button ID="Button2" runat="server" 
                                        AutoPostBack="false" CausesValidation="False" CommandName="Select" 
                                        Text="&gt;" />
                                </ItemTemplate>
                                <controlstyle font-size="XX-Small" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                </asp:TemplateField><asp:BoundField ApplyFormatInEditMode="True" 
                                    DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" 
                                    ReadOnly="True" SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                                    Width="100px" /></asp:BoundField><asp:BoundField DataField="Descrizione" 
                                    HeaderText="Descrizione" SortExpression="Descrizione" /><asp:BoundField 
                                    DataField="Prezzo" HeaderText="Prezzo €" SortExpression="Prezzo" /><asp:TemplateField 
                                    HeaderText="Sconto 1 %" SortExpression="Sconto_1"><ItemTemplate><asp:Label 
                                        ID="Label2" runat="server" Text='<%# Bind("Sconto_1") %>'></asp:Label>
                                </ItemTemplate>
                                
                                
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Sconto_1") %>'></asp:TextBox>
                                </EditItemTemplate>
                                
                                
                                </asp:TemplateField>
                                <%--<asp:TemplateField HeaderText="Sconto 2 %" 
                                    SortExpression="Sconto_2"><ItemTemplate><asp:Label ID="Label3" runat="server" 
                                        Text='<%# Bind("Sconto_2") %>'></asp:Label>
                                </ItemTemplate>
                                
                                
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Sconto_2") %>'></asp:TextBox>
                                </EditItemTemplate>
                                
                                
                                </asp:TemplateField>--%>
                                <asp:TemplateField HeaderText="Prezzo minimo €" 
                                    SortExpression="PrezzoMinimo"><ItemTemplate><asp:Label ID="Label4" 
                                        runat="server" Text='<%# Bind("PrezzoMinimo") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("PrezzoMinimo") %>'></asp:TextBox>
                                </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Prezzo Acquisto €" 
                                    SortExpression="PrezzoMinimo"><ItemTemplate><asp:Label ID="Label5" 
                                        runat="server" Text='<%# Bind("PrezzoAcquisto") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("PrezzoAcquisto") %>'></asp:TextBox>
                                </EditItemTemplate>
                                </asp:TemplateField>
                            </Columns>
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
                            </asp:GridView></div></td></tr><tr><td bgcolor="Silver" class="style25" colspan="4"></td></tr><tr>
                                <td bgcolor="Silver" class="styleLblTB1" colspan="1">Articolo</td>
                                <td bgcolor="Silver" class="style34" colspan="3">
                                    <asp:Label ID="lblCodArticolo" runat="server" BorderStyle="Outset" 
                                        Font-Bold="True" Text="" Width="150px"></asp:Label> 
                                    <asp:Label ID="lblDescrizione" runat="server" BorderStyle="Outset" 
                                        Font-Bold="True" Text="" Width="500px"></asp:Label></td></tr><tr>
                                    <td bgcolor="Silver" class="styleLblTB1">Prezzo €</td>
                                    <td bgcolor="Silver" class="styleTxtCodTB1"><asp:TextBox ID="txtPrezzo" 
                                            runat="server" MaxLength="10" TabIndex="5" Width="80px"></asp:TextBox></td>
                                    <td bgcolor="Silver" class="style28">Prezzo minimo €</td>
                                    <td bgcolor="Silver" class="style30"><asp:TextBox ID="txtPrezzoMinimo" 
                                            runat="server" AutoPostBack="True" MaxLength="10" TabIndex="5" Width="80px"></asp:TextBox></td></tr><tr>
                                    <td bgcolor="Silver" class="styleLblTB1">Sconto 1 %</td><td 
                                    bgcolor="Silver" class="style25"><asp:TextBox ID="txtSconto1" 
                                    runat="server" MaxLength="5" TabIndex="6" Width="80px"></asp:TextBox></td>
                                    <td bgcolor="Silver" class="style29"></td><%--Sconto 2 %--%>
                                    <td bgcolor="Silver" class="style31"><asp:TextBox ID="txtSconto2" runat="server" 
                                            MaxLength="5" TabIndex="7" Width="80px" Visible="false"></asp:TextBox></td></tr></table>
                    </asp:Panel>
                </ContentTemplate>
</asp:TabPanel>
                        <asp:TabPanel ID="TabPanel3" runat="server" 
                            HeaderText="Includi/Escludi articoli dal listino">
                    <ContentTemplate>
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server"><ContentTemplate>
<asp:Panel ID="Panel1" runat="server" Height="25px">Ordinamento e ricerca per:&#160; <asp:DropDownList 
                                ID="ddlRicercaArtIn" runat="server" AutoPostBack="True" Width="240px"></asp:DropDownList>&#160;&#160; <asp:TextBox 
                                ID="txtRicercaArtIn" runat="server" Width="250px" AutoPostBack="true"></asp:TextBox>&#160;&#160;<asp:Button 
                                ID="btnRicercaArtIn" runat="server" Text="Cerca articolo" /></asp:Panel>
                </ContentTemplate>
</asp:UpdatePanel>
                        <asp:Panel ID="Panel4" runat="server" Height="25px"><asp:Label ID="lblIntesta1" 
                            runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                            Style="text-align:center" Text="Articoli presenti nel listino" Width="100%"></asp:Label>
</asp:Panel>
                        <asp:Panel ID="Panel2" runat="server" Height="440px"><asp:UpdatePanel 
                            ID="UpdatePanel7" runat="server">
                                <ContentTemplate>
                                <asp:SqlDataSource ID="SqlDSArtIn" runat="server" DeleteCommand="[Delete_ListVenD]" 
                                        DeleteCommandType="StoredProcedure" InsertCommand="InsertUpdate_ListVenD" 
                                        InsertCommandType="StoredProcedure" SelectCommand="get_ListVenDByCodListino" 
                                        SelectCommandType="StoredProcedure" UpdateCommand="InsertUpdate_ListVenD" 
                                        UpdateCommandType="StoredProcedure">
    <SelectParameters>
        <asp:SessionParameter DefaultValue="1" Name="CodLis" SessionField="IDListino" 
            Type="Int32" />
        <asp:SessionParameter DefaultValue="C" Name="SortListVenD" 
            SessionField="SortListVenD" Type="String" />
                                    </SelectParameters>
                            <DeleteParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="CodLis" Type="Int32" />
                                <asp:Parameter Name="Cod_Articolo" Type="String" />
                                    </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="CodLis" Type="Int32" />
                                <asp:Parameter Name="Cod_Articolo" Type="String" />
                                <asp:Parameter Name="Prezzo" Type="Decimal" />
                                <asp:Parameter Name="Prezzo_Valuta" Type="Decimal" />
                                <asp:Parameter Name="Sconto_1" Type="Decimal" />
                                <asp:Parameter Name="Sconto_2" Type="Decimal" />
                                <asp:Parameter Name="PrezzoMinimo" Type="Decimal" />
                                <asp:Parameter Name="Data_Cambio" Type="DateTime" />
                                <asp:Parameter Name="Cambio" Type="Decimal" />
                                <asp:Parameter Direction="InputOutput" Name="RetVal" Type="Int32" />
                            </UpdateParameters>
                            <InsertParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="CodLis" Type="Int32" />
                                <asp:Parameter Name="Cod_Articolo" Type="String" />
                                <asp:Parameter Name="Prezzo" Type="Decimal" />
                                <asp:Parameter Name="Prezzo_Valuta" Type="Decimal" />
                                <asp:Parameter Name="Sconto_1" Type="Decimal" />
                                <asp:Parameter Name="Sconto_2" Type="Decimal" />
                                <asp:Parameter Name="PrezzoMinimo" Type="Decimal" />
                                <asp:Parameter Name="Data_Cambio" Type="DateTime" />
                                <asp:Parameter Name="Cambio" Type="Decimal" />
                                <asp:Parameter Direction="InputOutput" Name="RetVal" Type="Int32" />
                            </InsertParameters>
                            </asp:SqlDataSource><table style="width:100%;">
                                        <tr><td>
                                            <div id="divGridViewArtIn" 
                                                style="overflow: auto; height: 200px; border-style:groove; background-color: Silver;">
                                                <asp:GridView ID="GridViewArtIn" runat="server" AutoGenerateColumns="False" 
                                                    CssClass="GridViewStyle" 
                                                    AllowSorting="false" 
                                                    EmptyDataText="Nessun dato disponibile."   
                                                    AllowPaging="true"
                                                    PageSize="5" 
                                                    PagerStyle-HorizontalAlign="Center" 
                                                    PagerSettings-Mode="NextPreviousFirstLast"
                                                    PagerSettings-Visible="true"
                                                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif"   
                                                    DataKeyNames="Cod_Articolo" DataSourceID="SqlDSArtIn" 
                                                    EnableTheming="False" GridLines="None">
                                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                                    <Columns>
                                                        <asp:TemplateField InsertVisible="False">
                                                            <ItemTemplate><asp:Button ID="Button2" runat="server" CausesValidation="False" 
                                                                    CommandName="Select" Text="↓" />
                                                        </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                    </asp:TemplateField><asp:BoundField ApplyFormatInEditMode="True" DataField="Cod_Articolo" 
                                                            DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                                                            SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" />
                                                            <ItemStyle Width="100px" /></asp:BoundField>
                                                        <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" 
                                                            SortExpression="Descrizione" />
                                                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo €" 
                                                            SortExpression="Prezzo" />
                                                        <asp:TemplateField HeaderText="Sconto 1 %" SortExpression="Sconto_1"><ItemTemplate>
                                                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Sconto_1") %>'></asp:Label>
                                                        </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Sconto_1") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                   <%-- <asp:TemplateField HeaderText="Sconto 2 %" SortExpression="Sconto_2"><ItemTemplate>
                                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Sconto_2") %>'></asp:Label>
                                                        </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Sconto_2") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                        
                                                    
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Prezzo minimo €" SortExpression="PrezzoMinimo">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("PrezzoMinimo") %>'></asp:Label>
                                                        </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("PrezzoMinimo") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prezzo Acquisto €" 
                                        SortExpression="PrezzoMinimo"><ItemTemplate><asp:Label ID="Label5" 
                                            runat="server" Text='<%# Bind("PrezzoAcquisto") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("PrezzoAcquisto") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    </asp:TemplateField>
                                                </Columns>
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
                                </asp:GridView></div></td></tr><tr><td bgcolor="Silver" class="style25"></td></tr>
                                        <tr>
                                            <td class="style25"><asp:Panel ID="Panel5" runat="server" Height="25px">
                                                    <asp:Label ID="Label1" runat="server" BorderStyle="Outset" Font-Bold="True" 
                                                    Font-Overline="False" Style="text-align:center" 
                                                    Text="Articoli esclusi dal listino" Width="100%"></asp:Label></asp:Panel></td></tr>
                                        <tr>
                                            <td class="style25">
                                                <asp:Panel ID="Panel3" runat="server" Height="25px" Width="840px">Ordinamento e ricerca per:&#160; 
                                                    <asp:DropDownList ID="DDLRicercaArtOu" runat="server" AutoPostBack="True" 
                                                    Width="240px"></asp:DropDownList>&#160;&#160; 
                                                    <asp:TextBox ID="txtRicercaArtOu" runat="server" Width="250px" AutoPostBack="true"></asp:TextBox>&#160;&#160;<asp:Button 
                                                    ID="btnRicercaArtOu" runat="server" Text="Cerca articolo" />
                                                    <div id="divGridViewArtOu" 
                                                        style="overflow: auto; height: 180px; border-style:groove; background-color: Silver;">
                                                        <asp:SqlDataSource ID="SqlDSArtOu" runat="server" 
                                                            DeleteCommand="[Delete_ListVenD]" DeleteCommandType="StoredProcedure" 
                                                            InsertCommand="InsertUpdate_ListVenD" InsertCommandType="StoredProcedure" 
                                                            SelectCommand="get_ArticoliEsclusi" SelectCommandType="StoredProcedure" 
                                                            UpdateCommand="InsertUpdate_ListVenD" UpdateCommandType="StoredProcedure">
                                                            <SelectParameters>
                                                                <asp:SessionParameter DefaultValue="1" Name="CodLis" SessionField="IDListino" 
                                                                    Type="Int32" />
                                                                <asp:SessionParameter DefaultValue="C" Name="SortArticoliOu" 
                                                                    SessionField="SortArticoliOu" Type="String" />
                                                            </SelectParameters>
                                                            <DeleteParameters>
                                                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                                                <asp:Parameter Name="CodLis" Type="Int32" />
                                                                <asp:Parameter Name="Cod_Articolo" Type="String" />
                                                            </DeleteParameters>
                                                            <UpdateParameters>
                                                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                                                <asp:Parameter Name="CodLis" Type="Int32" />
                                                                <asp:Parameter Name="Cod_Articolo" Type="String" />
                                                                <asp:Parameter Name="Prezzo" Type="Decimal" />
                                                                <asp:Parameter Name="Prezzo_Valuta" Type="Decimal" />
                                                                <asp:Parameter Name="Sconto_1" Type="Decimal" />
                                                                <asp:Parameter Name="Sconto_2" Type="Decimal" />
                                                                <asp:Parameter Name="PrezzoMinimo" Type="Decimal" />
                                                                <asp:Parameter Name="Data_Cambio" Type="DateTime" />
                                                                <asp:Parameter Name="Cambio" Type="Decimal" />
                                                                <asp:Parameter Direction="InputOutput" Name="RetVal" Type="Int32" />
                                                            </UpdateParameters>
                                                            <InsertParameters>
                                                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                                                <asp:Parameter Name="CodLis" Type="Int32" />
                                                                <asp:Parameter Name="Cod_Articolo" Type="String" />
                                                                <asp:Parameter Name="Prezzo" Type="Decimal" />
                                                                <asp:Parameter Name="Prezzo_Valuta" Type="Decimal" />
                                                                <asp:Parameter Name="Sconto_1" Type="Decimal" />
                                                                <asp:Parameter Name="Sconto_2" Type="Decimal" />
                                                                <asp:Parameter Name="PrezzoMinimo" Type="Decimal" />
                                                                <asp:Parameter Name="Data_Cambio" Type="DateTime" />
                                                                <asp:Parameter Name="Cambio" Type="Decimal" />
                                                                <asp:Parameter Direction="InputOutput" Name="RetVal" Type="Int32" />
                                                            </InsertParameters>
                                                        </asp:SqlDataSource>
                                                        <asp:GridView ID="GridViewArtOu" runat="server" AutoGenerateColumns="False" 
                                                            CssClass="GridViewStyle" 
                                                            AllowSorting="false" 
                                                            EmptyDataText="Nessun dato disponibile."   
                                                            AllowPaging="true"
                                                            PageSize="5" 
                                                            PagerStyle-HorizontalAlign="Center" 
                                                            PagerSettings-Mode="NextPreviousFirstLast"
                                                            PagerSettings-Visible="true"
                                                            PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                                            PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                                            PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                                            PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif"   
                                                            DataKeyNames="Cod_Articolo" DataSourceID="SqlDSArtOu" 
                                                            EnableTheming="False" GridLines="None">
                                                            <Columns>
                                                                <asp:TemplateField InsertVisible="False">
                                                                    <ItemTemplate>
                                                                        <asp:Button ID="Button2" runat="server" CausesValidation="False" 
                                                                            CommandName="Select" Text="↑" />
                                                                    </ItemTemplate>
                                                                    <controlstyle font-size="XX-Small" />
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Cod_Articolo" 
                                                                    DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                                                                    SortExpression="Cod_Articolo">
                                                                    <HeaderStyle Wrap="True" />
                                                                    <ItemStyle Width="100px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" 
                                                                    SortExpression="Descrizione" />
                                                            </Columns>
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
                                                        </asp:GridView>
                                                    </div>
                                                </asp:Panel></td><td>
                                                &nbsp;</td></tr></table>
                        </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>
                        </ContentTemplate>
</asp:TabPanel>
                        <asp:TabPanel ID="TabPanel4" runat="server" HeaderText="Duplica listino">
                    <ContentTemplate>
                        <uc2:WUC_ListiniDuplica ID="WUC_ListiniDuplica1" runat="server" />
    </ContentTemplate>
</asp:TabPanel>
            </asp:TabContainer>
                </td>
                <td align="left" class="style23">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
            <div>
            <asp:Label ID="lblDescIncl" runat="server" Text="Per includere un articolo nel listino cliccare ↑, per escluderlo cliccare ↓" Visible="false"></asp:Label>
            </div>
            <div>
            <asp:Button ID="btnEliminaArt" runat="server" class="btnstyle" 
                    Text="Escludi articolo" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnNuovo" runat="server" class="btnstyle" Text="Nuovo" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnModifica" runat="server" class="btnstyle" Text="Modifica" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnAggiorna" runat="server" class="btnstyle" Text="Aggiorna" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <%--<asp:Button ID="btnAnnulla" runat="server" Text="Annulla" OnClientClick = "if (confirm('Vuoi Annullare le modifiche?')) { return true } else { return false };" class="btnstyle"/>--%>
            <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <%--<asp:Button ID="btnElimina" runat="server" Text="Elimina" OnClientClick = "if (confirm('Vuoi cancellare il listino selezionato ? Attenzione saranno cancellati anche gli articoli collegati.')) { return true } else { return false };" class="btnstyle"/>--%>
            <asp:Button ID="btnElimina" runat="server" class="btnstyle" Text="Elimina" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnStampaSc" runat="server" class="btnstyle" Text="Stampa con sc." Visible="false" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnIncludiArt" runat="server" class="btnstyle" 
                    Text="Includi articolo" />
            </div>
             <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnIncludiAll" runat="server" class="btnstyle" 
                    Text="Includi tutti" Visible="false" />
            </div>
                    </ContentTemplate>
            </asp:UpdatePanel>
                </td>
            </tr>
</table>
</ContentTemplate>
</asp:UpdatePanel> 
</asp:Panel>