<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_GesMagazzini.ascx.vb" Inherits="SoftAziOnLine.WUC_GesMagazzini" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register Src="../WebUserControl/WUC_SceltaStampaUbiArt.ascx" TagName="WUC_SceltaStampaUbiArt" TagPrefix="uc4" %>
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
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="550px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanelPrincipale" runat="server"> 
    <ContentTemplate>
        <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc4:WUC_SceltaStampaUbiArt ID="WUC_SceltaStampaUbiArt1" runat="server" />
        <table style="width:auto; height:auto;">
            <tr>
                <td class="style23">
                    <asp:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" 
                        AutoPostBack="True" Height="500px" style="margin-top: 0px" Width="1110px" 
                        BackColor="Silver">
                        <asp:TabPanel ID="TabPanel1" runat="server" HeaderText="Seleziona Magazzino" Font-Bold="true" >
                    <ContentTemplate>
                        <asp:Panel ID="PanelMagazzini" runat="server" Height="540px">
                        <asp:UpdatePanel 
                            ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:SqlDataSource 
                                ID="SqlDSMagazzini" runat="server" 
                                DeleteCommand="Delete_Magazzini" DeleteCommandType="StoredProcedure" 
                                InsertCommand="InsertUpdate_Magazzini" InsertCommandType="StoredProcedure" ProviderName="System.Data.SqlClient" 
                                SelectCommand="get_Magazzini" SelectCommandType="StoredProcedure" 
                                UpdateCommand="InsertUpdate_Magazzini" UpdateCommandType="StoredProcedure">
                            <DeleteParameters>
                                <asp:Parameter 
                                    Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter 
                                    Name="Codice" Type="Int32" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Codice" Type="Int32" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                            </UpdateParameters>
                            <InsertParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Codice" Type="Int32" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                            </InsertParameters>
                        </asp:SqlDataSource>
                        <table style="width:100%;">
                            <tr>
                            <td colspan="4">
                            <div id="divGridViewMagazzini" style="overflow: auto; height: 430px; border-style:groove; background-color: Silver;">
                                    <asp:GridView ID="GridViewMagazzini" runat="server" 
                                    AutoGenerateColumns="False" CssClass="GridViewStyle" 
                                    EmptyDataText="Nessun dato disponibile."  
                                    DataKeyNames="Codice" 
                                    DataSourceID="SqlDSMagazzini" EnableTheming="False" GridLines="None"><AlternatingRowStyle 
                                    CssClass="AltRowStyle" />
                                <Columns>
                                    <asp:TemplateField InsertVisible="False"><ItemTemplate><asp:Button 
                                        ID="Button1" runat="server" CausesValidation="False" CommandName="Select" 
                                        Text="&gt;" />
                                    </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                    </asp:TemplateField>
                                    <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="Codice" DataFormatString="{0:d}" HeaderText="Codice" ReadOnly="True" 
                                        SortExpression="Codice"><HeaderStyle Wrap="True" /><ItemStyle 
                                        Width="10px" /></asp:BoundField>
                                    <asp:BoundField DataField="Descrizione" 
                                        HeaderText="Descrizione" SortExpression="Descrizione" />
                                </Columns>
                                <HeaderStyle CssClass="HeaderStyle" />
                                <PagerSettings Mode="NextPrevious" Visible="False" />
                                <PagerStyle CssClass="PagerStyle" />
                                <RowStyle CssClass="RowStyle" />
                                <SelectedRowStyle CssClass="SelectedRowStyle" />
                                </asp:GridView>
                            </div></td></tr> 
                            <tr>
                            <td bgcolor="Silver" class="style25" colspan="4">
                                <asp:Label ID="Label9" runat="server" Font-Bold="true" ForeColor="Blue" BorderStyle="Outset">NOTA Se inserite un nuovo Magazzino eseguire: 1) includere tutti gli articoli e al termine 2) Ricalcola Giacenze</asp:Label>
                            </td></tr>
                            <tr>
                                <td bgcolor="Silver" class="styleLblTB0" colspan="1">Codice</td>
                                <td bgcolor="Silver" class="styleTxtCodTB0" colspan="1">
                                    <asp:TextBox ID="txtCodice" 
                                        runat="server" AutoPostBack="True" MaxLength="5" Width="100px"></asp:TextBox></td>
                                <td bgcolor="Silver" class="style32" colspan="2">Descrizione&#160;&#160;&#160;<asp:TextBox 
                                        ID="txtDescrizione" runat="server" AutoPostBack="true" MaxLength="50" 
                                        TabIndex="1" Width="400px"></asp:TextBox></td></tr><tr>
                        </table>
                        </ContentTemplate>
                        </asp:UpdatePanel></asp:Panel>
                    </ContentTemplate>
                </asp:TabPanel>
                <asp:TabPanel ID="TabPanel2" runat="server" 
                    HeaderText="Articoli presenti nel Magazzino">
                <ContentTemplate>
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server"><ContentTemplate>
                        <asp:Panel ID="PanelArtMagRicerca" runat="server" Height="25px">Ordinamento e ricerca per:&#160;<asp:DropDownList 
                            ID="ddlRicerca" runat="server" AutoPostBack="True" Width="240px"></asp:DropDownList>&#160;&#160;<asp:TextBox 
                            ID="txtRicerca" runat="server" Width="250px"></asp:TextBox>&#160;&#160;<asp:Button 
                            ID="btnRicercaArticolo" runat="server" Text="Cerca articolo" /></asp:Panel>
                        </ContentTemplate>
                        <triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnRicercaArticolo" EventName="Click" />
                        </triggers>
                    </asp:UpdatePanel>
                    <asp:Panel ID="PanelArtMag" runat="server" Height="440px">
                    <asp:SqlDataSource ID="SqlDSArtDiMag" runat="server" DeleteCommand="[Delete_ArtDiMag]" 
                            DeleteCommandType="StoredProcedure" InsertCommand="InsertUpdate_ArtDiMag" 
                            InsertCommandType="StoredProcedure" SelectCommand="get_ArtDiMagByCMag" 
                            SelectCommandType="StoredProcedure" UpdateCommand="InsertUpdate_ArtDiMag" 
                            UpdateCommandType="StoredProcedure">
                        <SelectParameters><asp:SessionParameter 
                            DefaultValue="0" Name="CodMag" SessionField="IDMAGAZZINI" Type="Int32" /><asp:SessionParameter 
                            DefaultValue="C" Name="SortArtDiMag" SessionField="SortArtDiMag" 
                            Type="String" />
                        </SelectParameters>
                        <DeleteParameters>
                            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                            <asp:Parameter Name="Original_Cod_Articolo" Type="String" />
                            <asp:Parameter Name="Original_Codice_Magazzino" Type="Int32" />
                        </DeleteParameters>
                        <UpdateParameters>
                            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                            <asp:Parameter Name="CodMag" Type="Int32" />
                            <asp:Parameter Name="CodArt" Type="String" />
                        </UpdateParameters>
                        <InsertParameters>
                            <asp:Parameter Name="CodMag" Type="Int32" />
                            <asp:Parameter Name="CodArt" Type="String" />
                        </InsertParameters>
                        </asp:SqlDataSource><table style="width:100%;"><tr>
                        <td colspan="4">
                        <div id="divGridArtMag" style="overflow: auto; height: 420px; border-style:groove; background-color: Silver;">
                                <asp:GridView ID="GridViewArtMag" runat="server" 
                                AutoGenerateColumns="False" CssClass="GridViewStyle" 
                                EmptyDataText="Nessun dato disponibile."  
                                AllowPaging="true"
                                    PageSize="18" 
                                    PagerStyle-HorizontalAlign="Center" 
                                    PagerSettings-Mode="NextPreviousFirstLast"
                                    PagerSettings-Visible="true"
                                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                                DataKeyNames="Cod_Articolo" DataSourceID="SqlDSArtDiMag" EnableTheming="False" 
                                GridLines="None"><AlternatingRowStyle CssClass="AltRowStyle" /><Columns><asp:TemplateField 
                                    InsertVisible="False"><ItemTemplate><asp:Button ID="Button2" runat="server" 
                                        AutoPostBack="false" CausesValidation="False" CommandName="Select" 
                                        Text="&gt;" />
                                </ItemTemplate>
                                <controlstyle font-size="XX-Small" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                </asp:TemplateField>
                                    <asp:BoundField ApplyFormatInEditMode="True" 
                                    DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" 
                                    ReadOnly="True" SortExpression="Cod_Articolo">
                                    <HeaderStyle Wrap="True" /><ItemStyle 
                                    Width="200px" /></asp:BoundField>
                                    <asp:BoundField DataField="Descrizione" 
                                    HeaderText="Descrizione" SortExpression="Descrizione" />
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
                            </asp:GridView></div></td></tr>
                            <tr><td bgcolor="Silver" class="style25" colspan="4"></td></tr>
                            <tr>
                                <td bgcolor="Silver" class="styleLblTB1" colspan="1">Articolo</td>
                                <td bgcolor="Silver" colspan="3">
                                    <asp:Label ID="lblCodArticolo" runat="server" BorderStyle="Outset" 
                                        Font-Bold="True" Text="" Width="200px"></asp:Label> 
                                    <asp:Label ID="lblDescrizione" runat="server" BorderStyle="Outset" 
                                        Font-Bold="True" Text="" Width="700px"></asp:Label>
                                </td>
                             </tr>
                       </table>
                    </asp:Panel>
                </ContentTemplate>
</asp:TabPanel>
<asp:TabPanel ID="TabPanel3" runat="server" 
    HeaderText="Includi/Escludi articoli dal Magazzino">
                    <ContentTemplate>
<asp:UpdatePanel ID="UpdatePanel6" runat="server">
    <ContentTemplate>
    <asp:Panel ID="Panel1" runat="server" Height="25px">Ordinamento e ricerca per:&#160; <asp:DropDownList 
        ID="ddlRicercaArtIn" runat="server" AutoPostBack="True" Width="240px"></asp:DropDownList>&#160;&#160; <asp:TextBox 
        ID="txtRicercaArtIn" runat="server" Width="250px" AutoPostBack="true"></asp:TextBox>&#160;&#160;<asp:Button 
        ID="btnRicercaArtIn" runat="server" Text="Cerca articolo" />
    </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:Panel ID="Panel4" runat="server" Height="25px"><asp:Label ID="lblIntesta1" 
    runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
    Style="text-align:center" Text="Articoli presenti nel Magazzino" Width="100%"></asp:Label>
</asp:Panel>
<asp:Panel ID="Panel2" runat="server" Height="440px" Width="100%">
    <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                <ContentTemplate>
        <asp:SqlDataSource ID="SqlDSArtIn" runat="server" DeleteCommand="[Delete_ArtDiMag]" 
            DeleteCommandType="StoredProcedure" InsertCommand="InsertUpdate_ArtDiMag" 
            InsertCommandType="StoredProcedure" SelectCommand="get_ArtDiMagByCMag" 
            SelectCommandType="StoredProcedure" UpdateCommand="InsertUpdate_ArtDiMag" 
            UpdateCommandType="StoredProcedure">
        <SelectParameters><asp:SessionParameter 
            DefaultValue="0" Name="CodMag" SessionField="IDMAGAZZINI" Type="Int32" /><asp:SessionParameter 
            DefaultValue="C" Name="SortArtDiMag" SessionField="SortArtDiMag" 
            Type="String" />
        </SelectParameters>
        <DeleteParameters>
            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
            <asp:Parameter Name="Original_Cod_Articolo" Type="String" />
            <asp:Parameter Name="Original_Codice_Magazzino" Type="Int32" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
            <asp:Parameter Name="CodMag" Type="Int32" />
            <asp:Parameter Name="CodArt" Type="String" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="CodMag" Type="Int32" />
            <asp:Parameter Name="CodArt" Type="String" />
        </InsertParameters>
        </asp:SqlDataSource>
        <table style="width:100%;">
            <tr><td>
                <div id="divGridViewArtIn" style="overflow: auto; height: 200px; border-style:groove; background-color: Silver;">
                    <asp:GridView ID="GridViewArtIn" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        AllowSorting="false" 
                        EmptyDataText="Nessun dato disponibile."   
                        AllowPaging="true"
                        PageSize="7" 
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
                        </asp:TemplateField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Cod_Articolo" 
                                DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                                SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" />
                                <ItemStyle Width="200px" /></asp:BoundField>
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
                    </asp:GridView></div></td></tr>
                    <tr>
                        <td bgcolor="Silver" class="style25"></td>
                    </tr>
                    <tr>
                        <td class="style25">
                            <asp:Panel ID="Panel5" runat="server" Height="25px">
                                <asp:Label ID="Label1" runat="server" BorderStyle="Outset" Font-Bold="True" 
                                Font-Overline="False" Style="text-align:center" 
                                Text="Articoli esclusi dal Magazzino" Width="100%"></asp:Label>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td class="style25">
                        <asp:Panel ID="Panel3" runat="server" Height="25px" Width="1090px">Ordinamento e ricerca per:&#160; 
                        <asp:DropDownList ID="DDLRicercaArtOu" runat="server" AutoPostBack="True" 
                                Width="240px"></asp:DropDownList>&#160;&#160; 
                        <asp:TextBox ID="txtRicercaArtOu" runat="server" Width="250px" AutoPostBack="true"></asp:TextBox>&#160;&#160;<asp:Button 
                                ID="btnRicercaArtOu" runat="server" Text="Cerca articolo" />
                        <div id="divGridViewArtOu" style="overflow: auto; height: 180px; border-style:groove; background-color: Silver;">
                            <asp:SqlDataSource ID="SqlDSArtOu" runat="server" 
                                DeleteCommand="[Delete_ArtDiMag]" DeleteCommandType="StoredProcedure" 
                                InsertCommand="InsertUpdate_ArtDiMag" InsertCommandType="StoredProcedure" 
                                SelectCommand="get_ArticoliEsclusiADM" SelectCommandType="StoredProcedure" 
                                UpdateCommand="InsertUpdate_ArtDiMag" UpdateCommandType="StoredProcedure">
                            <SelectParameters><asp:SessionParameter 
                                DefaultValue="0" Name="CodMag" SessionField="IDMAGAZZINI" Type="Int32" /><asp:SessionParameter 
                                DefaultValue="C" Name="SortArtDiMag" SessionField="SortArtDiMag" 
                                Type="String" />
                            </SelectParameters>
                            <DeleteParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Original_Cod_Articolo" Type="String" />
                                <asp:Parameter Name="Original_Codice_Magazzino" Type="Int32" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="CodMag" Type="Int32" />
                                <asp:Parameter Name="CodArt" Type="String" />
                            </UpdateParameters>
                            <InsertParameters>
                                <asp:Parameter Name="CodMag" Type="Int32" />
                                <asp:Parameter Name="CodArt" Type="String" />
                            </InsertParameters>
                            </asp:SqlDataSource>
                            <asp:GridView ID="GridViewArtOu" runat="server" AutoGenerateColumns="False" 
                                CssClass="GridViewStyle" 
                                AllowSorting="false" 
                                EmptyDataText="Nessun dato disponibile."   
                                AllowPaging="true"
                                PageSize="7" 
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
                                        <ItemStyle Width="200px" />
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
</asp:TabContainer>
    </td>
<td align="left" class="style23">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
            <div>
            <asp:Label ID="lblDescIncl" runat="server" Text="Per includere un articolo nel Magazzino cliccare ↑, per escluderlo cliccare ↓" Visible="false"></asp:Label>
            </div>
            <div>
            <asp:Button ID="btnEliminaArt" runat="server" class="btnstyle" Text="Escludi articolo" />
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
            <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnElimina" runat="server" class="btnstyle" Text="Elimina" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnIncludiAll" runat="server" class="btnstyle" Text="Includi tutti" Visible="false" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnEscludiAll" runat="server" class="btnstyle" Text="Escludi tutti" Visible="false" />
            </div>
            <div style="height: 15px"></div>
            <div>
            <asp:Button ID="btnStampaElenco" runat="server" class="btnstyle" Text="Stampa" Visible="true"/>
            </div>
                    </ContentTemplate>
            </asp:UpdatePanel>
    </td>
</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel> 
</asp:Panel>