<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ElencoOrdini.ascx.vb" Inherits="SoftAziOnLine.WUC_ElencoOrdini" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="~/WebUserControl/WFP_SceltaSpedizione.ascx" tagname="WFP_SceltaSpedizione" tagprefix="uc2" %>
<%@ Register Src="~/WebUserControl/WFP_CambiaStatoOC.ascx" TagName="WFPCambiaStatoOC" TagPrefix="wuc3" %>
<%@ Register Src="~/WebUserControl/WFP_FatturaOC.ascx" TagName="WFPFatturaOC" TagPrefix="wuc4" %>
<%@ Register Src="~/WebUserControl/WFP_DocCollegati.ascx" TagName="WFPDocCollegati" TagPrefix="wuc5" %>
<style type="text/css">
    .btnstyle1RL
        {
            Width: 140px;
            height: 30px;
        margin-left: 0px;
        }
     .btnstyle1RLL
        {
            Width: 180px;
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
    <uc2:WFP_SceltaSpedizione ID="WFPSceltaSpedizione" runat="server" />
    <wuc3:WFPCambiaStatoOC ID="WFPCambiaStatoOC" runat="server" />
    <wuc4:WFPFatturaOC ID="WFPFatturaOC" runat="server" />
    <wuc5:WFPDocCollegati ID="WFPDocCollegati" runat="server" />
    <table border="0" cellpadding="0" frame="box" style="width:auto; height:550px; margin-right:0;">
        <tr>
            <td  width ="100%" align="left" colspan="2" >
                    <table style="width:1230px">
                    <tr>
                        <td>
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" AutoPostBack="true" Width="145px"></asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            &nbsp;<asp:TextBox ID="txtRicerca" runat="server" Width="200px"></asp:TextBox>&nbsp;<asp:Button ID="btnRicerca" runat="server" Text="Cerca ordine" class="btnstyle1R" />
                            &nbsp;<asp:Button ID="btnFatturaOC" runat="server" Text="Emissione Fattura" class="btnstyle1RL" />
                            &nbsp;<asp:Button ID="btnFatturaOCAC" runat="server" Text="Fattura per Acconto/Saldo" class="btnstyle1RLL" />
                            &nbsp;<asp:Button ID="btnDocCollegati" runat="server" Text="Documenti Collegati" class="btnstyle1RL" />
                        </td>
                    </tr>
                    </table> 
            </td>
        </tr>
        <tr>
            <td  width ="100%" align="left">
                <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="50px" BorderStyle="Groove">
                <div>
                    <asp:Label ID="Label8" runat="server" Width="68px">&nbsp;Seleziona:</asp:Label>
                    <asp:RadioButton ID="rbtnEvaso" runat="server" Text="Evasi" AutoPostBack="True" 
                        GroupName="Tipo" />
                    <asp:Label ID="Label0" runat="server" Width="50px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnDaEvadere" runat="server" Text="Da evadere" 
                        AutoPostBack="True" GroupName="Tipo" />
                    <asp:Label ID="Label1" runat="server" Width="50px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnParzEvaso" runat="server" Text="Parzialmente evasi" 
                        AutoPostBack="True" GroupName="Tipo" />
                    <asp:Label ID="Label2" runat="server" Width="30px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnChiusoNoEvaso" runat="server" Text="Chiuso non evasi" 
                        AutoPostBack="True" GroupName="Tipo" />
                    <asp:Label ID="Label5" runat="server" Width="40px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnNonEvadibile" runat="server" Text="Non evadibile" 
                        AutoPostBack="True" GroupName="Tipo" />
                    <asp:Label ID="Label11" runat="server" Width="5px">&nbsp;</asp:Label>
                    <asp:CheckBox ID="checkInContratti" runat="server" Font-Bold="true" ForeColor="Blue" Font-Size="XX-Small" Text="Ordini In Contratto sono visualizzati solo in *Tutti* e *In Contratto*" Checked="true" Visible="true" Enabled="false"/>
                </div>
                <div>
                    <asp:Label ID="Label3" runat="server" Width="68px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnImpegnati" runat="server" Text="Impegnati" 
                        AutoPostBack="True" GroupName="Tipo" />
                    <asp:Label ID="Label6" runat="server" Width="22px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnImpegnati100" runat="server" Text="Impegnati 100%" 
                        AutoPostBack="True" GroupName="Tipo" />
                    <asp:Label ID="Label7" runat="server" Width="16px">&nbsp;</asp:Label>    
                    <asp:RadioButton ID="rbntInAllestimento" runat="server" Text="In Allestimento" 
                        AutoPostBack="True" GroupName="Tipo" />
                    <asp:Label ID="Label10" runat="server" Width="50px">&nbsp;</asp:Label>  
                    <asp:RadioButton ID="rbtnInContratto" runat="server" Text="In Contratto" Font-Bold="true" ForeColor="Blue"
                        AutoPostBack="True" GroupName="Tipo" />
                    <asp:Label ID="Label4" runat="server" Width="65px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" AutoPostBack="True" GroupName="Tipo" />
                     <asp:Label ID="Label9" runat="server" Width="80px">&nbsp;</asp:Label>
                     <asp:Button ID="btnNuovoDaOC" runat="server" class="btnstyle1RLong" Text="Nuovo Contratto da Ordine selezionato" />
                </div>
                </asp:Panel>
            </td>
            <td align="left" class="style2">
                <div>
                    <asp:Button ID="btnSblocca" runat="server" class="btnstyle2R" Text="Sblocca Doc." />
                </div>
                <div>
                    <asp:Button ID="btnCambiaStato" runat="server" class="btnstyle1R" Text="Cambia stato" />
                </div>
            </td>
        </tr>
        <tr>
            <td align="left" class="style3">
            <%--<div id="divGridViewPrevT" style="overflow-x:hidden; overflow-y:hidden; width:860px; height:195px; border-style:groove;">--%> 
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
                            SortExpression="DesStatoDoc"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                            Width="5px" /></asp:BoundField> 
                            <asp:BoundField DataField="Numero" HeaderText="Numero" 
                                SortExpression="Numero">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="10px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="RevisioneNDoc" 
                                DataFormatString="{0:d}" HeaderText="Rev. N°"  
                                SortExpression="RevisioneNDoc">
                                <HeaderStyle Wrap="True" Width="5px"/>
                                <ItemStyle Width="1px" Wrap="false" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Data_Doc" HeaderText="Data" 
                                SortExpression="Data_Doc">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="DataOraConsegna" HeaderText="Data consegna" 
                                SortExpression="DataOraConsegna">
                                <HeaderStyle Wrap="true"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="PercImp" HeaderText="%Imp. Quantità" 
                                SortExpression="PercImp"><HeaderStyle Wrap="True"/><ItemStyle 
                                Width="15px" Wrap="false"/></asp:BoundField>
                            <asp:BoundField DataField="PercImPorto" HeaderText="%Imp. Importo" 
                                SortExpression="PercImPorto"><HeaderStyle Wrap="True"/><ItemStyle 
                                Width="15px" Wrap="false"/></asp:BoundField>
                            <asp:BoundField DataField="Cod_Cliente" HeaderText="Codice Cliente" 
                                SortExpression="Cod_Cliente">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="10px" Wrap="false"/>
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
                                HeaderText="Località"  
                                SortExpression="Localita">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" 
                                HeaderText="CAP"  
                                SortExpression="CAP">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="5px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Partita_IVA" 
                                HeaderText="Partita IVA"  
                                SortExpression="Partita_IVA">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Codice_Fiscale" 
                                HeaderText="Codice Fiscale"  
                                SortExpression="Codice_Fiscale">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>  
                            <asp:BoundField DataField="Data_Validita" HeaderText="Data validità" 
                                SortExpression="Data_Validita">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="10px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                                SortExpression="Riferimento">                                    
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="50px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Data_Riferimento" HeaderText="Data Rif." 
                                SortExpression="Data_Riferimento">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="10px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione1" 
                                HeaderText="Destinazione(1)"  
                                SortExpression="Destinazione1">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="25px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione2" 
                                HeaderText="Destinazione(2)"  
                                SortExpression="Destinazione2">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="25px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione3" 
                                HeaderText="Destinazione(3)"  
                                SortExpression="Destinazione3">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="25px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="NumeroCA" HeaderText="NumeroCA" 
                                SortExpression="NumeroCA">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="10px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="Data_DocCA" HeaderText="Data_DocCA" 
                                SortExpression="Data_DocCA">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="10px" Wrap="false"/>
                            </asp:BoundField>
                        </Columns>
                        <%--<HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />--%>
                    </asp:GridView>
                    <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                        SelectCommand="get_OrdiniTElenco" 
                        SelectCommandType="StoredProcedure">
                        <SelectParameters>
                            <asp:SessionParameter DefaultValue="OC" Name="TipoDoc" SessionField="TipoDoc" Type="String" />
                            <asp:SessionParameter DefaultValue="0" Name="StatoDoc" SessionField="StatoDoc" Type="Int32" />
                            <asp:SessionParameter DefaultValue="N" Name="SortPrevTEl" SessionField="SortPrevTEl" Type="String" />
                        </SelectParameters>
                    </asp:SqlDataSource>
               </div>
            </td>
            <td align="left" class="style7">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div>
                            <asp:Button ID="btnVisualizza" runat="server" class="btnstyle1R" Text="Visualizza" />
                        </div>
                        <div style="height: 5px"></div>
                        <div>
                            <asp:Button ID="btnNuovo" runat="server" class="btnstyle1R" Text="Nuovo" />
                        </div>
                        <div style="height: 5px">&nbsp;</div> 
                        </div>
                        <div>
                            <asp:Button ID="btnModifica" runat="server" class="btnstyle1R" Text="Modifica" />
                        </div>
                        <div style="height: 5px">&nbsp;</div> 
                        </div>
                        <div>
                            <asp:Button ID="btnElimina" runat="server" class="btnstyle1R" Text="Elimina" />
                        </div>
                        <div style="height: 5px">&nbsp;</div> 
                        </div>
                        <div>
                            <asp:Button ID="btnAllestimento" runat="server" class="btnstyle1R" Text="Allestimento" />
                        </div>
                        <div style="height: 5px">&nbsp;</div> 
                        <div>
                            <asp:Button ID="btnCreaDDT" runat="server" class="btnstyle1R" Text="Crea DDT" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="left" class="style5">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
               <div id="divGridViewPrevD" style="overflow-x:hidden; overflow-y:hidden; width:1110px; height:200px; border-style:groove;">                
                    <asp:GridView ID="GridViewPrevD" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato selezionato."  
                        DataKeyNames="IDDocumenti" 
                        EnableTheming="True" GridLines="None" 
                        DataSourceID="SqlDSPrevDByIDDocumenti"><AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns>
                        <asp:BoundField DataField="Cod_Articolo" 
                        HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo">
                        <HeaderStyle Wrap="false" Width="20px"/>
                        <ItemStyle Width="20px" Wrap="false" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" Width="350px"/><ItemStyle 
                        Width="350px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità ordinata" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Impegnata" HeaderText="Quantità impegnata" 
                        SortExpression="Qta_Impegnata"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità allestita" 
                        SortExpression="Qta_Evasa"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Residua" HeaderText="Quantità residua" 
                        SortExpression="Qta_Residua"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Allestita" HeaderText="Quantità inviata" 
                        SortExpression="Qta_Allestita"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Cod_IVA" HeaderText="IVA" 
                        SortExpression="Cod_IVA"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo" 
                        SortExpression="Prezzo"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoValore" HeaderText="Sconto valore" 
                        SortExpression="ScontoValore"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Sconto_1" HeaderText="Sc.(1)" 
                        SortExpression="Sconto_1"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Importo" HeaderText="Importo" 
                        SortExpression="Importo"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoReale" HeaderText="Sc.Riga" 
                        SortExpression="ScontoReale"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
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
                    <div>
                         <asp:Button ID="btnNuovaRev" runat="server" class="btnstyle1R" Text="Nuova Rev.N°" />
                            </div>
                    <div style="height: 5px">&nbsp;</div>
                    <div>
                         <asp:Button ID="btnCopia" runat="server" class="btnstyle1R" Text="Copia ordine" />
                            </div>
                    <div style="height: 5px">&nbsp;</div>
                    <div>
                         <asp:Button ID="btnCreaOF" runat="server" class="btnstyle2R" Text="Crea ordine fornitore" />
                            </div> 
                    <div style="height: 20px; text-align:center"><%--<b>Stampe</b>--%>
                        <asp:Label ID="lblStampe" runat="server" BorderColor="White"
                        BorderStyle="None" Font-Bold="True" ForeColor="Black">Stampe</asp:Label>
                    </div> 
                    <div>
                        <a ID="LnkStampa" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Documento" style="border-color:snow;border-style:outset;background-color:yellow;">Apri Documento</a>
                    </div>
                    <div style="height: 5px">&nbsp;</div> 
                    <div>
                        <asp:Button ID="btnConfOrdine" runat="server" class="btnstyle2R" Text="Conferma" Visible="false" />
                            </div>
                    <div>
                        <asp:Button ID="btnStampa" runat="server" class="btnstyle2R" Text="Conferma Ordine" />
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
        options2.width = 1110;
        options2.height = 200;
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