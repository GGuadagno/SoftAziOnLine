<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_DocumentiDett.ascx.vb" Inherits="SoftAziOnLine.WUC_DocumentiDett" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WFP_Articolo_Seleziona.ascx" tagname="WFP_Articolo_Seleziona" tagprefix="uc2" %>
<%@ Register src="~/WebUserControl/WFP_LottiInsert.ascx" tagname="WFPLottiInsert" tagprefix="wuc2" %>

<div align="left" style="border:1 solid White; Width:1090px; height:400px;">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc2:WFP_Articolo_Seleziona ID="WFP_Articolo_Seleziona1" runat="server" />
        <wuc2:WFPLottiInsert ID="WFPLottiInsert1" runat="server" />
        <table style="width:auto; height:auto;" class="sfondopagine" >
            <tr>
                <td colspan="4">
                        <asp:Panel ID="PanelDett" runat="server" Height="300px" Width ="1090px">
                        <asp:UpdatePanel ID="UpdatePanelDett" runat="server"><ContentTemplate>
                        <%--style="overflow: auto; height: 300px; border-style:groove; background-color: Silver;">--%>
                        <%--style="overflow-x:hidden; overflow-y:hidden; width:1090px; height:300px; border-style:groove;">--%>
                        <div id="divGridViewDett" style="overflow: auto; height: 300px; border-style:groove; background-color: Silver;">
                          <asp:GridView ID="GridViewDett" runat="server" AutoGenerateColumns="False" 
                                EmptyDataText="Nessun dato disponibile."  
                                DataKeyNames="Riga" 
                                GridLines="None" CssClass="GridViewStyle" EnableTheming="True"
                                AllowPaging="False" 
                                PagerSettings-Mode="NextPreviousFirstLast"
                                PagerSettings-Visible="True"
                                PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" >
                                <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                                    LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                                    Mode="NextPreviousFirstLast" 
                                    NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                    PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                                <RowStyle CssClass="RowStyle" />
                                <PagerStyle CssClass="PagerStyle" />
                                <HeaderStyle CssClass="HeaderStyle" />
                                <AlternatingRowStyle CssClass="AltRowStyle" />
                                <SelectedRowStyle CssClass="SelectedRowStyle"/>
                                <Columns>
                                    <asp:TemplateField InsertVisible="True"><ItemTemplate>
                                    <asp:Button ID="btnInsRigaDopo" runat="server" CommandName="Select" Text="+" /> 
                                    </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10px" />
                                    </asp:TemplateField>
                                    <asp:CommandField ButtonType="Button" ShowEditButton="True" EditText="Modifica" UpdateText="Aggiorna" CancelText="Annulla"
                                        HeaderStyle-Width="50px" >
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="50px" />
                                    </asp:CommandField>
                                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" 
                                        HeaderStyle-Width="20px" DeleteText="X" > 
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="20px" />
                                    </asp:CommandField>
                                </Columns>
                          </asp:GridView>
                          </div>
                          </ContentTemplate>
                          </asp:UpdatePanel>
                          </asp:Panel>   
                       </td>
                    </tr> 
            <tr>
            <td colspan="4">
            <table >
            <tr>
            <td>
            <asp:Button ID="btnNewRigaUp" runat="server" CommandName="btnNewRigaUp" 
                    Text="+ Riga sopra" ToolTip="Nuova riga sopra" />
            <asp:Label ID="Label1" runat="server" Text="Riga selezionata: "></asp:Label>
            <asp:Label ID="lblRigaSel" runat="server" BorderStyle="Outset" Width="30px" 
                    Font-Bold="True"></asp:Label> 
            <asp:Label ID="lblBase" runat="server" BorderStyle="Outset" Width="20px" Font-Bold="False"></asp:Label>
            <asp:Label ID="lblOpz" runat="server" BorderStyle="Outset" Width="20px" Font-Bold="False"></asp:Label> 
            <asp:Button ID="BtnSelArticolo" runat="server" CommandName="BtnSelArticolo" 
                    Text="? Articoli" ToolTip="Seleziona articoli" />
            <asp:Button ID="btnPrimaRiga" runat="server" CommandName="btnPrimaRiga"
                    Text="+" Visible="false" ToolTip="Inserisci 1° riga" />
            <%--<asp:Label ID="Label9" runat="server" Text="" Width="10px"></asp:Label>--%>
            </td>
            
            <%--<td>TLM.P.Listino</td>--%><td align="right"><asp:Label ID="LblTotaleLordoPL" runat="server" BorderStyle="Outset" Width="1px" Font-Bold="True" Visible="false"></asp:Label></td>
            <%--<td>Totale lordo merce:</td>--%><td align="right"><asp:Label ID="LblTotaleLordo" runat="server" BorderStyle="Outset" Width="1px" Font-Bold="True" Visible="false"></asp:Label></td>
            <td><asp:Label ID="Label4" runat="server" Text="Imponibile"></asp:Label></td> 
            <td align="right"><asp:Label ID="LblImponibile" runat="server" BorderStyle="Outset" Width="120px" Font-Bold="True"></asp:Label></td>
            <td>IVA</td><td align="right"><asp:Label ID="LblImposta" runat="server" BorderStyle="Outset" Width="105px" Font-Bold="True"></asp:Label></td>
            <td>Totale</td><td align="right"><asp:Label ID="LblTotale" runat="server" BorderStyle="Outset" Width="105px" Font-Bold="True"></asp:Label></td>
            </tr></table>
            </td>
            </tr>
        <tr>
        <td colspan="4">
                <asp:TabContainer ID="PanelSubDettArt" runat="server" ActiveTabIndex="1" 
                    Width="1090px" Height="125px" BackColor="Silver">
                     <asp:TabPanel runat="server" ID="PanelDettArtLottiNrSerie" HeaderText="Lotti / N° di serie"  BackColor="Silver">
                     <HeaderTemplate>Lotti / N° di serie</HeaderTemplate>
                     <ContentTemplate>
                     <table class="sfondopagine" style="Width:960px;Height:125px;">
                     <tr>
                     <td align="left">
                        <asp:Panel ID="Panel2" runat="server" Height="125px" Width ="960px">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server"><ContentTemplate>
                        <div id="divGridViewDettL" style="overflow: auto; height:120px; border-style:groove; background-color: Silver;">
                          <asp:GridView ID="GridViewDettL" runat="server" AutoGenerateColumns="False" 
                                EmptyDataText="Nessun dato disponibile."  
                                DataKeyNames="NCollo" 
                                GridLines="None" CssClass="GridViewStyle" EnableTheming="True"
                                AllowPaging="False" 
                                PagerSettings-Mode="NextPreviousFirstLast"
                                PagerSettings-Visible="True"
                                PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" >
                                <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                                    LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                                    Mode="NextPreviousFirstLast" 
                                    NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                    PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                                <RowStyle CssClass="RowStyle" />
                                <PagerStyle CssClass="PagerStyle" />
                                <HeaderStyle CssClass="HeaderStyle" />
                                <AlternatingRowStyle CssClass="AltRowStyle" />
                                <SelectedRowStyle CssClass="SelectedRowStyle"/>
                                <Columns>
                                    <asp:TemplateField InsertVisible="True"><ItemTemplate>
                                    <asp:Button ID="btnInsRigaDopoL" runat="server" CommandName="Select" Text="+" />
                                    </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10px" />
                                    </asp:TemplateField>
                                    <asp:CommandField ButtonType="Button" ShowEditButton="True" 
                                        HeaderStyle-Width="50px" >
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="50px" />
                                    </asp:CommandField>
                                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" 
                                        HeaderStyle-Width="20px" DeleteText="X" > 
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="20px" />
                                    </asp:CommandField>
                                </Columns>
                          </asp:GridView>
                          </div>
                          </ContentTemplate>
                          </asp:UpdatePanel>
                          </asp:Panel>   
                     </td>
                     <td>
                        <div>
                            <asp:Button ID="btnPrimaRigaL" runat="server" CommandName="btnPrimaRigaL" Text="Inserisci lotti" 
                             ToolTip="Inserisci lotti" Height="35px" Width="105px" />
                        </div>
                        <div style=" height:5px; "></div>
                        <div>
                            <asp:Button ID="btnCaricoLotti" runat="server" Text="Lettore Lotti"
                             ToolTip="Carico lotti con lettore" Height="35px" Width="105px" />
                        </div>
                        </td>
                     </tr>
                     </table>
                     </ContentTemplate>
                     </asp:TabPanel>
                     <asp:TabPanel runat="server" ID="PanelDettArtIns" HeaderText="Aggiorna/Inserisci dettagli"  BackColor="Silver">
                     <HeaderTemplate>Aggiorna/Inserisci dettagli</HeaderTemplate>
                     <ContentTemplate>
                     <table class="sfondopagine" style="Width:960px;Height:125px;">
                     <tr>
                     <td align="left">
                        <asp:Panel ID="Panel3" runat="server" Height="125px" Width ="960px">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server"><ContentTemplate>
                        <div id="div2" style="overflow: auto; height:120px; border-style:groove; background-color: Silver;">
                          <div>
                              <asp:Label ID="Label6" runat="server" Width="30px">&nbsp;</asp:Label> 
                              <asp:Label ID="lblCodice" runat="server" Width="110px">Codice</asp:Label> 
                              <asp:Label ID="lblDescrizione" runat="server" Width="425px">Descrizione</asp:Label> 
                              <asp:Label ID="Label7" runat="server" Width="25px">UM</asp:Label>
                              <asp:Label ID="lblQta" runat="server" Width="50px">Ordin.</asp:Label> 
                              <asp:Label ID="lblQtaEv" runat="server" Width="50px">Evasa</asp:Label>
                              <asp:Label ID="lblLabelQtaRe" runat="server" Width="55px">Residua</asp:Label> 
                              <asp:Label ID="LblQtaInv" runat="server" Width="45px">Inviata</asp:Label> 
                          </div>
                          <div>
                              <%--<asp:Label ID="Label8" runat="server" Width="5px">&nbsp;</asp:Label>--%>
                              <asp:Button ID="BtnSelArticoloIns" runat="server" CommandName="BtnSelArticoloIns" Text="?" ToolTip="Ricerca articoli" Enabled="false"/>
                              <asp:TextBox ID="txtCodArtIns" runat="server" Width="105px" MaxLength="20" AutoPostBack="true"  BorderStyle="None"></asp:TextBox>
                              <asp:TextBox ID="txtDesArtIns" runat="server" Width="425px" MaxLength="150" BorderStyle="None"></asp:TextBox>
                              <asp:TextBox ID="txtUMIns" runat="server" MaxLength="2" Width="25px" BorderStyle="None"></asp:TextBox>
                              <asp:TextBox ID="txtQtaIns" runat="server" MaxLength="5" Width="45px" BorderStyle="None"></asp:TextBox>
                              <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" 
                                                    ControlToValidate="txtQtaIns" ErrorMessage="*" 
                                                    ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                              <asp:TextBox ID="txtQtaEv" runat="server" MaxLength="5" Width="45px" BorderStyle="None"></asp:TextBox>
                              <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="txtQtaEv" ErrorMessage="*" 
                                                    ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                              <asp:Label ID="LblQtaRe" runat="server" BorderStyle="Outset" Width="50px" Font-Bold="True"></asp:Label>
                              <asp:TextBox ID="txtQtaInv" runat="server" MaxLength="5" Width="45px" ToolTip="Attenzione, nel caso in cui venga modificato il documento figlio, la quantità in oggetto sarà riaggiornata automaticamente" BorderStyle="None"></asp:TextBox>
                          </div>
                          <div>&nbsp;
                          </div>
                          <div>
                              <asp:Label ID="Label13" runat="server" Width="2px">&nbsp;</asp:Label>
                              <asp:Label ID="Label12" runat="server" Width="30px">IVA</asp:Label> 
                              <asp:Label ID="lblPrezzoAL" runat="server" Width="107px" Text="Prezzo listino"></asp:Label>
                              <asp:Label ID="Label10" runat="server" Width="50px">Sc. (1)</asp:Label> 
                              <asp:Label ID="Label5" runat="server" Width="105px">Prezzo Netto</asp:Label> 
                              <asp:Label ID="Label2" runat="server" Width="110px">Importo riga</asp:Label> 
                              <asp:Label ID="lblDedPerAcc" runat="server" Width="110px"></asp:Label> 
                              <asp:Label ID="Label3" runat="server" Width="68px">Giacenza</asp:Label>
                              <asp:Label ID="Label9" runat="server" Width="75px">Giac.Imp.</asp:Label>
                              <asp:Label ID="Label15" runat="server" Width="90px">Ord.Forn.</asp:Label>
                              <asp:Label ID="Label16" runat="server" Width="120px">Data/Qtà arrivo</asp:Label>
                          </div>
                          <div>
                              <%--<asp:Label ID="Label19" runat="server" Width="5px">&nbsp;</asp:Label>--%>
                              <asp:TextBox ID="txtIVAIns" runat="server" Width="30px" MaxLength="2" AutoPostBack="true" BorderStyle="None"></asp:TextBox>
                              <asp:TextBox ID="txtPrezzoIns" runat="server" Width="100px" MaxLength="15" BorderStyle="None"></asp:TextBox>
                              <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                                                    ControlToValidate="txtPrezzoIns" ErrorMessage="*" 
                                                    ValidationExpression="^[.0-9]+(\,[0-9]{1,3})?$" />--%>
                              <asp:TextBox ID="txtSconto1Ins" runat="server" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>
                              <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                                                    ControlToValidate="txtSconto1Ins" ErrorMessage="*" 
                                                    ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                              <asp:Label ID="LblPrezzoNetto" runat="server" BorderStyle="Outset" Width="100px" Font-Bold="True"></asp:Label>
                              <asp:Label ID="LblImportoRiga" runat="server" BorderStyle="Outset" 
                                  Width="100px" Font-Bold="True"></asp:Label>
                              <asp:CheckBox ID="ChkDedPerAcc" runat="server" Font-Bold="true" Text="Deduzione" Width="110px" Checked="false" AutoPostBack="false"/>
                              <asp:Label ID="lblGiacenza" runat="server" BorderStyle="Outset" 
                                  Width="65px" Font-Bold="False"></asp:Label>
                              <asp:Label ID="lblGiacImp" runat="server" BorderStyle="Outset" 
                                  Width="65px" Font-Bold="False"></asp:Label>
                              <asp:Label ID="lblOrdFor" runat="server" BorderStyle="Outset" 
                                  Width="65px" Font-Bold="False"></asp:Label>
                              <asp:Label ID="lblDataArr" runat="server" BorderStyle="Outset" 
                                  Width="145px" Font-Bold="False"></asp:Label>
                          </div>
                          <%--<div>&nbsp;
                          </div>--%>
                          <div>
                            <asp:Label ID="lblMessAgg" runat="server" BorderStyle="Outset"
                                  Width="915px" Font-Bold="False"></asp:Label>  
                          </div>
                        </div>
                        </ContentTemplate>
                        </asp:UpdatePanel>
                        </asp:Panel>   
                     </td>
                     <td>
                     <div>
                        <asp:Button ID="btnAggArtGridSel" runat="server" CommandName="btnAggArtGridSel" 
                        Text="Aggiorna riga" Height="35px" Width="105px" />
                     </div>

                     <div style="height: 5px"></div>
                     <div>
                        <asp:Label ID="lblSuperatoScMax" runat="server" BorderStyle="None"
                                 Width="100px" Font-Bold="True"></asp:Label>
                     </div>
                     <div style="height: 5px"></div>
                     <div style="height: 25px">
                        <asp:CheckBox ID="checkNoScontoValore" runat="server" Font-Bold="True" 
                             Text="NO Sc.Val."/>
                     </div>    
                     <div>
                        <asp:Label ID="Label11" runat="server" BorderStyle="None"
                                 Width="105px" Font-Bold="False" Text="Costo unitario"></asp:Label>
                     </div>
                      <div>
                        <asp:TextBox ID="txtPrezzoCosto" runat="server" Width="100px" MaxLength="15" 
                              Enabled="False" BorderStyle="None"></asp:TextBox>
                      </div>
                     </td>
                     </tr>
                     </table>
                     </ContentTemplate>
                     </asp:TabPanel>
                </asp:TabContainer>         
        </td>
        </tr>                     
    </table> 
    </ContentTemplate>
</asp:UpdatePanel> 
</div>
<%--<script src="../JScript/jquery-1.4.4.min.js" type="text/javascript"></script>
<script src="../JScript/ScrollableGridPlugin.js" type="text/javascript"></script>
<script type="text/javascript" src="../JScript/gridviewscroll.js"></script>
<script type = "text/javascript">
    function GridScroll() {

        var options2 = new GridViewScrollOptions();
        options2.elementID = "<%=GridViewDett.ClientID %>";
        options2.width = 1090;
        options2.height = 300;
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

</script>	--%>