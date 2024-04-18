<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ElencoSchedeInventario.ascx.vb" Inherits="SoftAziOnLine.WUC_ElencoSchedeInventario" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register Src="~/WebUserControl/WFP_MovMagDaCancellare.ascx" TagName="WFPMovMagDaCancellare" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControl/WFP_ModificaSchedaIN.ascx" TagName="WFPModificaSchedaIN" TagPrefix="wuc2" %>
<%@ Register src="../WebUserControl/WUC_Attesa.ascx" tagname="Attesa" tagprefix="uc3" %>
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
    Width="980px" Height="550px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <wuc:WFPMovMagDaCancellare ID="WFPMovMagDaCanc" runat="server" />
    <wuc2:WFPModificaSchedaIN ID="WFPModificaSchedaIN1" runat="server" />
    <uc3:Attesa ID="Attesa" runat="server" />
<table border="0" cellpadding="0" frame="box" 
            style="width:auto; height:550px; margin-right:0;">
        </td>
    </tr>
            <tr>
                <td align="left" class="style1" colspan="2">
                    &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" 
                        AutoPostBack="True" Width="240px">
                    </asp:DropDownList>
                    &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                    <asp:TextBox ID="txtRicerca" runat="server" Width="275px"></asp:TextBox>
                    <asp:Button ID="btnRicerca" runat="server" Text="Cerca scheda" Width="120px" />
                </td>
            </tr>
            <tr>
                <td align="left" class="styleBordo">
                    <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="25px">
                    <div>
                        <asp:RadioButton ID="rbtnEvaso" runat="server" Text="Chiusa e aggiornato magazzino" AutoPostBack="True" 
                            GroupName="Tipo" />
                        <asp:Label ID="Label0" runat="server" Width="20px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnDaEvadere" runat="server" Text="Da inventariare" 
                            AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label1" runat="server" Width="20px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnChiusoNoEvaso" runat="server" Text="Chiusa non inventariata" 
                            AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label5" runat="server" Width="20px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnInCarico" runat="server" Text="In modifica" 
                            AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label7" runat="server" Width="20px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" AutoPostBack="True" GroupName="Tipo" />
                    </div>
                    </asp:Panel>
                </td>
                <td align="left" class="style2">
                    <div>
                        <asp:Button ID="btnSblocca" runat="server" class="btnstyle" Text="Sblocca" />
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left" class="style3">
                <div id="divGridViewPrevT" style="overflow:auto; width:860px; height:215px; border-style:groove;">
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
                            <Columns>
                                <asp:CommandField ButtonType="Button" CausesValidation="False" 
                                    ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                                    ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                                    <ControlStyle Font-Size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                                </asp:CommandField>
                                <asp:BoundField DataField="DesStatoDoc" HeaderText="Stato" 
                                SortExpression="DesStatoDoc"><HeaderStyle Wrap="True" /><ItemStyle 
                                Width="15px" /></asp:BoundField> 
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                                    DataFormatString="{0:d}" HeaderText="N° Inventario" ReadOnly="True" 
                                    SortExpression="Numero">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="RevisioneNDoc" 
                                    DataFormatString="{0:d}" HeaderText="N° Pagina" ReadOnly="True" 
                                    SortExpression="RevisioneNDoc">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data_Doc" HeaderText="Data inventario" 
                                    SortExpression="Data_Doc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="InseritoDa" HeaderText="Creata da" 
                                    SortExpression="InseritoDa">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="250px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="ModificatoDa" 
                                    HeaderText="Modificata da" ReadOnly="True" 
                                    SortExpression="ModificatoDa">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="250px" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                            SelectCommand="get_PrevTElencoIN" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="IN" Name="TipoDoc" SessionField="TipoDoc" Type="String" />
                                <asp:SessionParameter DefaultValue="0" Name="StatoDoc" SessionField="StatoDoc" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                </td>
                <td align="left" class="style7">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnModificaScheda" runat="server" class="btnstyle" Text="Modifica scheda" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnChiudiScheda" runat="server" class="btnstyle" Text="Chiusa non inventariata" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnApriScheda" runat="server" class="btnstyle" Text="Apri Scheda chiusa" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnElimina" runat="server" class="btnstyle" Text="Elimina" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        <tr>
            <td align="left" class="style5">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                <div id="divGridViewPrevD" style="overflow:auto; width:860px; height:230px; border-style:groove;">
                    <asp:GridView ID="GridViewPrevD" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato selezionato."  
                        DataKeyNames="IDDocumenti" 
                        EnableTheming="True" GridLines="None" 
                        DataSourceID="SqlDSPrevDByIDDocumenti"><AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Riga" DataFormatString="{0:d}" HeaderText="Riga" ReadOnly="True" 
                        SortExpression="Riga"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="20px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="false" /><ItemStyle 
                        Width="250px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Giacenza" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità inventario" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <%--<asp:BoundField DataField="Qta_Residua" HeaderText="Differenza inventario" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>--%>
                        <asp:BoundField DataField="Cod_IVA" HeaderText="IVA" 
                        SortExpression="Cod_IVA"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo" 
                        SortExpression="Prezzo"><HeaderStyle Wrap="True" /><ItemStyle 
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
                        <asp:Button ID="btnCreaMMRettifica" runat="server" class="btnstyle" Text="Elimina movimento" Visible="false" />
                    </div>
                    <div style="height: 15px"></div>
                    <div>
                        <asp:Button ID="btnEliminaMM" runat="server" class="btnstyle" Text="Elimina movimento" />
                    </div>
                    <div style="height: 15px"></div>
                    <div style="height: 15px">
                        <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Scheda" />
                    </div>
                </td>
           </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>
