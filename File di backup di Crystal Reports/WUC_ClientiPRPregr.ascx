<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ClientiPRPregr.ascx.vb" Inherits="SoftAziOnLine.WUC_ClientiPRPregr" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_DocCollegati.ascx" tagname="WFPDocCollegati" tagprefix="wuc1" %>
<%@ Register Src="~/WebUserControl/WFP_CambiaStatoPR.ascx" TagName="WFPCambiaStatoPR" TagPrefix="wuc3" %>
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
    }
    .btnstyle1RLong
    {
        Width: 250px;
        height: 25px;
    margin-left: 0px;
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
    .styleh180
    {
        height: 180px;
    }
    .styleh170
    {
        height: 170px;
    }
    .styleh230
    {
        height: 230px;
    }
    .style9
    {
        height: 50px;
    }
</style> 
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Groove" Width="1210px" Height="500px" CssClass="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <wuc1:WFPDocCollegati ID="WFPDocCollegati" runat="server" />
    <wuc3:WFPCambiaStatoPR ID="WFPCambiaStatoPR" runat="server" />
    <table border="0" cellpadding="0" frame="box" style="width:1205px; height:500px; margin-right:0;">
        <tr>
            <td width ="1205px" style="height:40px" align="left" colspan="2" >
                    <table style="width:1205px;height:40px">
                    <tr>
                        <td>
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" AutoPostBack="true" Width="200px"></asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            &nbsp;<asp:TextBox ID="txtRicerca" runat="server" Width="150px"></asp:TextBox>
                            &nbsp;<asp:Button ID="btnRicerca" runat="server" Text="Cerca preventivo"/>
                            &nbsp;<asp:Button ID="btnDocCollegati" runat="server" Text="Documenti Collegati"/>
                            &nbsp;<asp:Button ID="btnCambiaStato" runat="server" Text="Cambia stato preventivo"/>
                        </td>
                    </tr>
                    </table> 
            </td>
        </tr>
        <tr>
            <td width ="1080px" align="left">
                <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="25px" BorderStyle="Groove">
                <div>
                        <asp:RadioButton ID="rbtnConfermati" runat="server" Text="Confermati" AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label0" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnDaConfermare" runat="server" Text="Da confermare" AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label1" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnNonConferm" runat="server" Text="Non confermabile" AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label2" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnChiusoNoConf" runat="server" Text="Chiuso non confermato" AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label5" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" AutoPostBack="True" GroupName="Tipo" />
                    </div>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td width ="1080px" align="left">
            <%--<div id="divGridViewPrevT" style="overflow-x:hidden; overflow-y:hidden; width:860px; height:195px; border-style:groove;">--%> 
            <div id="divGridViewPrevT" style="overflow:auto; width:1080px; height:180px; border-style:groove;">                
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
                                <asp:BoundField DataField="DesStatoPR" HeaderText="Stato" 
                                SortExpression="DesStatoPR"><HeaderStyle Width="15px" Wrap="True" /><ItemStyle 
                                Width="15px" /></asp:BoundField> 
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                                    DataFormatString="{0:d}" HeaderText="Numero" ReadOnly="True" 
                                    SortExpression="Numero">
                                    <HeaderStyle Wrap="True" Width="10px" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="RevisioneNDoc" 
                                    DataFormatString="{0:d}" HeaderText="Rev.N°" ReadOnly="True" 
                                    SortExpression="RevisioneNDoc">
                                    <HeaderStyle Wrap="True" Width="5px"/>
                                    <ItemStyle Width="5px" />
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
                                <asp:BoundField DataField="Data_Riferimento" HeaderText="Data Rif." 
                                    SortExpression="Data_Riferimento">
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
                            </Columns>
                            <%--<HeaderStyle CssClass="HeaderStyle" />
                            <PagerSettings Mode="NextPrevious" Visible="False" />
                            <PagerStyle CssClass="PagerStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />--%>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                            SelectCommand="get_PrevTElenco" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="PR" Name="TipoDoc" SessionField="TipoDocPR" Type="String" />
                                <asp:SessionParameter DefaultValue="0" Name="StatoDoc" SessionField="StatoDocPR" Type="Int32" />
                                <asp:SessionParameter DefaultValue="N" Name="SortPrevTEl" SessionField="SortPrevTEl" Type="String" />
                                <%--<asp:SessionParameter DefaultValue="" Name="CodiceCliente" SessionField="CodiceCliente" Type="String" />--%>
                                <asp:SessionParameter DefaultValue="" Name="CodiceCliente" SessionField="Codice_CoGeOCPR" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
               </div>
            </td>
            <td align="left" class="styleh180">
                 <div>
                    <asp:Button ID="btnVisualizza" runat="server" class="btnstyle1R" Text="Visualizza" />
                </div>
                <div style="height: 5px"></div>
                <div>
                    <asp:Button ID="btnNuovo" runat="server" class="btnstyle1R" Text="Nuovo" Visible="false" />
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
                    <asp:Button ID="btnCreaOrdine" runat="server" class="btnstyle1R" Text="Crea ordine" />
                </div>
            </td>
        </tr>
        <tr>
            <td width ="1080px" align="left">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
               <div id="divGridViewPrevD" style="overflow-x:hidden; overflow-y:hidden; width:1080px; height:230px; border-style:groove;">                
                    <asp:GridView ID="GridViewPrevD" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato selezionato."  
                        DataKeyNames="IDDocumenti" 
                        EnableTheming="True" GridLines="None" 
                        DataSourceID="SqlDSPrevDByIDDocumenti"><AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="20px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="350px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
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
                            <asp:SessionParameter DefaultValue="0" Name="IDDocumenti" SessionField="IDDocPRCli" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
            </div>
            </ContentTemplate>
                    </asp:UpdatePanel>
            </td>
            <td align="left" class="styleh230">
                 <div style="height: 15px">
                     <asp:Button ID="btnNuovaRev" runat="server" class="btnstyle1R" Text="Nuova Rev.N°" />
                </div>
                <div style="height: 15px">&nbsp;</div>
                <div style="height: 15px">&nbsp;</div>
                <div style="height: 15px">
                     <asp:Button ID="btnCopia" runat="server" class="btnstyle1R" Text="Copia offerta" />
                        </div>
                <div style="height: 15px">&nbsp;</div>
                <div style="height: 15px">&nbsp;</div>
                <%--<div style="height: 15px">&nbsp;</div>--%> 
                <div style="height: 15px; text-align:center">
                <asp:Label ID="lblStampe" runat="server" Font-Bold="true" BorderStyle="None">Stampe</asp:Label>
                <%--<b>Stampe</b>--%></div> 
                <div style="height: 15px">&nbsp;</div> 
                <%--<div style="height: 15px">&nbsp;</div>--%> 
                <div style="height: 15px">
                    <asp:Button ID="btnStampa" runat="server" class="btnstyle1R" Text="Stampa" />
                        </div>
                <div style="height: 15px">&nbsp;</div>
                <div>
                    <a ID="LnkStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Stampa">Apri Stampa</a>
                </div>
                <div style="height: 15px">
                        </div>
            </td>
        </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>
<script src="../JScript/jquery-1.4.4.min.js" type="text/javascript"></script>
<script src="../JScript/ScrollableGridPlugin.js" type="text/javascript"></script>
<script type="text/javascript" src="../JScript/gridviewscroll.js"></script>
<script type = "text/javascript">
    function GridScroll() {
//        var options1 = new GridViewScrollOptions();
//        options1.elementID = "<%=GridViewPrevT.ClientID %>";
//        options1.width = 3000;
//        options1.height = 195;
//        options1.freezeColumn = true;
//        options1.freezeFooter = false;
//        options1.freezeColumnCssClass = "GridViewScrollItemFreeze";
//        options1.freezeFooterCssClass = "GridViewScrollFooterFreeze";
//        options1.freezeHeaderRowCount = 1;
//        options1.freezeColumnCount = 0;

//        gridViewScroll1 = new GridViewScroll(options1);

//        gridViewScroll1.enhance();

        var options2 = new GridViewScrollOptions();
        options2.elementID = "<%=GridViewPrevD.ClientID %>";
        options2.width = 1080;
        options2.height = 230;
        options2.freezeColumn = true;
        options2.freezeFooter = false;
        options2.freezeColumnCssClass = "GridViewScrollItemFreeze";
        options2.freezeFooterCssClass = "GridViewScrollFooterFreeze";
        options2.freezeHeaderRowCount = 1;
        options2.freezeColumnCount = 0;

        gridViewScroll2 = new GridViewScroll(options2);

        gridViewScroll2.enhance();
    }
    $(document).ready(GridScroll);
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(GridScroll);

</script>		