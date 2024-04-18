<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ArticoliUbicazione.ascx.vb" Inherits="SoftAziOnLine.WUC_ArticoliUbicazione" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_Reparti.ascx" tagname="WFP_Reparti" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WFP_Scaffali.ascx" tagname="WFP_Scaffali" tagprefix="uc3" %>
<%@ Register Src="../WebUserControl/WUC_SceltaStampaUbiArt.ascx" TagName="WUC_SceltaStampaUbiArt" TagPrefix="uc4" %>
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
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" 
    Width="1235px" Height="550px" CssClass ="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc2:WFP_Reparti ID="WFP_Reparti1" runat="server" />
    <uc3:WFP_Scaffali ID="WFP_Scaffali1" runat="server" />
    <uc4:WUC_SceltaStampaUbiArt ID="WUC_SceltaStampaUbiArt1" runat="server" />
    <table border="0" cellpadding="0" frame="box" 
            style="width:auto; height:550px; margin-right:0;">
        </td>
    </tr>
            <tr>
                <td align="left" class="style1" colspan="2">
                    <table style="width: 1110px">
                    <tr>
                        <td align="left" class="style1" colspan="2">
                            &nbsp;Magazzino:&nbsp; <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"
                                AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                                DataTextField="Descrizione" 
                                DataValueField="Codice" Width="200px" TabIndex="2">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
                                SelectCommand="SELECT Codice, Descrizione FROM Magazzini ORDER BY Descrizione">
                            </asp:SqlDataSource>
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" 
                                AutoPostBack="True" Width="200px">
                            </asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            <asp:TextBox ID="txtRicerca" runat="server" Width="250px"></asp:TextBox>
                            <asp:Button ID="btnRicerca" runat="server"  
                                Text="Avvia ricerca" Width="100px" />
                        </td>
                    </tr>
                    </table> 
                </td>
            </tr>
            <tr>
                <td align="left">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                    <div id="divGridViewPrevT" style="overflow:auto; width:1110px; height:300px; border-style:groove;">
                        <asp:GridView ID="GridViewPrevT" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
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
                        EnableTheming="True" GridLines="None" DataSourceID="SqlDSTElenco" BackColor="Silver" AllowSorting="True">
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
                        <Columns><asp:TemplateField HeaderStyle-Width="5" InsertVisible="False">
                        <ItemTemplate><asp:Button ID="btnSelAI" runat="server" 
                        CausesValidation="False" CommandName="Select" Text="&gt;" />
                        </ItemTemplate>
                        <controlstyle font-size="XX-Small" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-Width="5" HeaderText="Sel.">
                            <ItemTemplate>
                                <asp:CheckBox id="checkSel" AutoPostBack="false" Checked="false" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                                <asp:BoundField ApplyFormatInEditMode="True" 
                                    DataField="Cod_Articolo" HeaderText="Codice articolo" ReadOnly="True" 
                                    SortExpression="Cod_Articolo"><HeaderStyle Wrap="false" /><ItemStyle 
                                    Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Descrizione" 
                                    HeaderText="Descrizione" SortExpression="Descrizione" HeaderStyle-Wrap="False" ItemStyle-Wrap="False"><HeaderStyle Wrap="false" /><ItemStyle 
                                    Width="200px" />
                                </asp:BoundField>
                                 <asp:BoundField DataField="Sottoscorta" HeaderText="Sottoscorta" 
                                        SortExpression="Sottoscorta"><HeaderStyle Wrap="false" /><ItemStyle 
                                        Width="10px" />
                                </asp:BoundField>  
                                <asp:BoundField DataField="Reparto" DataFormatString="{0:d}" HeaderText="Reparto" 
                                        SortExpression="Reparto"><HeaderStyle Wrap="false" /><ItemStyle 
                                        Width="10px" />
                                </asp:BoundField>    
                                <asp:BoundField DataField="Scaffale" DataFormatString="{0:d}" HeaderText="Scaffale" 
                                        SortExpression="Scaffale"><HeaderStyle Wrap="false" /><ItemStyle 
                                        Width="10px" />
                                </asp:BoundField>  
                                <asp:BoundField DataField="Piano" DataFormatString="{0:d}" HeaderText="Piano" 
                                        SortExpression="Piano"><HeaderStyle Wrap="false" /><ItemStyle 
                                        Width="10px" />
                                </asp:BoundField>      
                        </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDSTElenco" runat="server" 
                            SelectCommand="SELECT ArtdiMag.Codice_Magazzino, AnaMag.Cod_Articolo, AnaMag.Descrizione, ISNULL(ArtdiMag.Reparto, 0) AS Reparto, ISNULL(ArtdiMag.Scaffale, 0) AS Scaffale, ISNULL(ArtdiMag.Piano, 0) AS Piano, 
                            ISNULL(ArtdiMag.Sottoscorta, 0) AS Sottoscorta
                            FROM AnaMag INNER JOIN ArtdiMag ON AnaMag.Cod_Articolo = ArtdiMag.Cod_Articolo WHERE  (ArtdiMag.Codice_Magazzino = @IDMagazzino) ORDER BY AnaMag.Cod_Articolo" 
                            SelectCommandType="Text">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="0" Name="IDMagazzino" SessionField="IDMagazzino" Type="Int32" />
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
                                <asp:Button ID="btnModifica" runat="server" class="btnstyle" Text="Modifica" />
                            </div>
                            <div style="height: 15px"></div>
                            <asp:Button ID="btnSelTutti" runat="server" class="btnstyle" Text="Seleziona tutti"/>
                            <div style="height: 15px"></div>
                            <asp:Button ID="btnDeselTutti" runat="server" class="btnstyle" Text="Deseleziona tutti"/>
                            <div style="height: 15px"></div>
                            <div>
                                <asp:Button ID="btnStampaElenco" runat="server" class="btnstyle" Text="Stampa"/>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td align="left">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                            <asp:Panel ID="PanelDettaglio" style="margin-top: 0px;" runat="server" groupingtext="Dettaglio articolo">
                                <table style="width: 100%">
                                <tr>
                                    <td width="15%" align="left">
                                        <asp:Label ID="Label1" runat="server">Articolo</asp:Label>
                                    </td>
                                    <td width ="85%" align="left">
                                        <asp:Label ID="lblCodArticolo" runat="server" BorderStyle="Outset" 
                                        Font-Bold="True" Text="" Width="300px"></asp:Label> 
                                        <asp:Label ID="Label6" runat="server" Text="" Width="200px"></asp:Label>
                                        <asp:Label ID="Label7" runat="server" Text="Sottoscorta"></asp:Label>
                                        <asp:TextBox ID="txtSottoscorta" runat="server" AutoPostBack="false"
                                            Width="70px" TabIndex="1" MaxLength="8"></asp:TextBox>
                                        <asp:Label ID="lblDescrizione" runat="server" BorderStyle="Outset" 
                                        Font-Bold="True" Text="" Width="900px"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td width="15%" align="left">
                                        <asp:Label ID="Label2" runat="server">Reparto</asp:Label>
                                        <asp:Button ID="btnReparto" runat="server" Height="22px" Text="?" Width="30px" />
                                    </td>
                                    <td width ="85%" align="left">
                                        <asp:TextBox ID="txtReparto" runat="server" AutoPostBack="true"
                                            Width="50px" TabIndex="1" MaxLength="5"></asp:TextBox>
                                        <asp:DropDownList ID="ddlReparto" runat="server" AppendDataBoundItems="true"
                                            AutoPostBack="true" DataSourceID="SqlDSReparto" 
                                            DataTextField="Descrizione" 
                                            DataValueField="Codice" Width="605px" TabIndex="2">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="SqlDSReparto" runat="server"
                                            SelectCommand="SELECT Codice, Descrizione FROM Reparti WHERE Magazzino=@IDMagazzino ORDER BY Descrizione" SelectCommandType="Text">
                                            <SelectParameters>
                                                <asp:SessionParameter DefaultValue="0" Name="IDMagazzino" SessionField="IDMagazzino" Type="Int32" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>
                                    </td>
                                </tr>
                                <tr>
                                    <td width="15%" align="left">
                                        <asp:Label ID="Label3" runat="server">Scaffale</asp:Label>
                                        <asp:Button ID="btnScaffale" runat="server" Height="22px" Text="?" Width="30px" />
                                    </td>
                                    <td width ="85%" align="left">
                                        <asp:TextBox ID="txtScaffale" runat="server" AutoPostBack="true"
                                            Width="50px" TabIndex="2" MaxLength="5"></asp:TextBox>
                                        <asp:DropDownList ID="DDLScaffale" runat="server" AppendDataBoundItems="true"
                                            AutoPostBack="true" DataSourceID="SqlDSScaffale" 
                                            DataTextField="Descrizione" 
                                            DataValueField="Scaffale" Width="605px" TabIndex="2">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="SqlDSScaffale" runat="server"
                                            SelectCommand="SELECT Scaffale, Descrizione FROM Scaffali WHERE Magazzino=@IDMagazzino AND Reparto=@IDReparto ORDER BY Descrizione">
                                            <SelectParameters>
                                                <asp:SessionParameter DefaultValue="0" Name="IDMagazzino" SessionField="IDMagazzino" Type="Int32" />
                                                <asp:SessionParameter DefaultValue="0" Name="IDReparto" SessionField="IDReparto" Type="Int32" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>
                                    </td>
                                </tr>
                                <tr>
                                    <td width="15%" align="left">
                                        <asp:Label ID="Label4" runat="server">Piano</asp:Label>
                                    </td>
                                    <td width ="85%" align="left">
                                        <asp:TextBox ID="txtPiano" runat="server" AutoPostBack="false"
                                            Width="50px" TabIndex="3" MaxLength="5"></asp:TextBox>
                                        <asp:Label ID="Label5" runat="server" Width="500px"></asp:Label>
                                        <asp:CheckBox id="CheckAggiornaSel" AutoPostBack="false" Checked="false" runat="server" Font-Bold="true" ForeColor="Blue" Text="Aggiorna tutti gli articoli selezionati" />
                                    </td>
                                </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td align="left">
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnAggiorna" runat="server" class="btnstyle" Text="Aggiorna" />
                            </div>
                            <div style="height: 15px"></div>
                            <div>
                                <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>     
</asp:Panel>