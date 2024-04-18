<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ElencoDistinteSped.ascx.vb" Inherits="SoftAziOnLine.WUC_ElencoDistinteSped" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_Vettori.ascx" tagname="WFP_Vettori" tagprefix="wuc2" %>
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
    .styleBordo
    {
    	margin-left: 0px;
        height: 35px;
        width: 840px;
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
    	margin-left: 0px;
        height: 185px;
    }
</style>
<br />
<asp:SqlDataSource ID="SqlDSVettori" runat="server" 
    SelectCommand="SELECT * FROM [Vettori] ORDER BY [Descrizione]">
</asp:SqlDataSource>
<asp:SqlDataSource ID="SqlDSSpedTElenco" runat="server" 
    SelectCommand="get_SpedTElenco" 
    SelectCommandType="StoredProcedure">
    <SelectParameters>                                
        <asp:SessionParameter DefaultValue="0" Name="StatoSped" SessionField="StatoSped" Type="Int32" />                                
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="SqlDSSpedDByIdSpedizione" runat="server" 
    SelectCommand="get_SpedDByIDSpedizione" 
    SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:SessionParameter DefaultValue="0" Name="IDSpedizione" SessionField="IDSpedizione" Type="Int32" />
    </SelectParameters>
</asp:SqlDataSource>
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" 
    Width="980px" Height="550px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <wuc2:WFP_Vettori ID="WFPVettori" runat="server"/>
    <table border="0" frame="box" style="width:980; height:550px; margin-right:0;">
            <%--<tr>
                <td align="left" colspan="2">
                    &nbsp;
                    </td>
            </tr>--%>
            <tr>
                <td align="left" class="styleBordo">
                    <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="50px" Width="840px">
                    <table >
                    <tr>
                    <td style="width:180px" >                    
                        <asp:RadioButton ID="rbtnAllestimento" runat="server" Text="ALLESTIMENTO" 
                            AutoPostBack="True" GroupName="Tipo" TabIndex="0" Checked="True" />                       
                    </td>                            
                    <td style="width:180px">                    
                        <asp:RadioButton ID="rbtnAllestite" runat="server" Text="Allestite" 
                            AutoPostBack="True" GroupName="Tipo" TabIndex="2" Visible="False" />

                    </td>                            
                    <td>
                        <asp:RadioButton ID="rbtnProntePerBrog" runat="server" Text="Inserite / pronte per stampa brogliaccio" 
                            AutoPostBack="True" GroupName="Tipo" TabIndex="4" Visible="False" />
                    </td>                    
                        <td style="width:180px">    
                        <asp:Label ID="Label2" runat="server" Width="20px" Text=""></asp:Label>
                        <asp:RadioButton ID="rbtnBloccate" runat="server" Text="Bloccate" 
                            AutoPostBack="True" GroupName="Tipo" TabIndex="6" Visible="False" />                   
                    </td>                   
                    </tr>                                                
                    
                    <tr>
                    <td>                    
                        <asp:RadioButton ID="rbtnInAllestimento" runat="server" Text="in Allestimento" 
                            AutoPostBack="True" GroupName="Tipo" TabIndex="1" Visible="False" />
                    </td>                            
                    
                    <td>                    
                        <asp:RadioButton ID="rbtnParzAllestite" runat="server" Text="Parzialmente allestite" 
                            AutoPostBack="True" GroupName="Tipo" TabIndex="3" Visible="False" /> 
                    </td>                            
                    <td>                    
                        <asp:RadioButton ID="rbtnPrintBrogAlle" runat="server" Text="Stampato brogliaccio Allestimento" 
                            AutoPostBack="True" GroupName="Tipo" TabIndex="5" Visible="False" />
                    </td>
                    
                    <td>
                        <asp:Label ID="Label1" runat="server" Width="20px" Text=""></asp:Label>
                        <asp:RadioButton ID="rbtnChiuse" runat="server" Text="Chiuse" 
                            AutoPostBack="True" GroupName="Tipo" TabIndex="7" Visible="False" />
                    </td>                                     
                    </tr> 
                    </table>
                    </asp:Panel>    
                </td>
                <td align="left" class="style2">
                    <div>
                        <asp:Button ID="btnEvadi" runat="server" class="btnstyle" 
                            Text="All./Evadi spedizione" Visible="False" />
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left" class="style3">
                <table width ="100%" >
                <tr>
                <td align="left">
                    <%--<div id="divGridViewSped" style="overflow-x:hidden; overflow-y:hidden; width:auto; height:auto; border-style:groove;">--%>
                    <div id="divGridViewSped" style="overflow:auto; width:auto; height:195px; border-style:groove;">    
                        <asp:GridView ID="GridViewSped" runat="server" AutoGenerateColumns="False" 
                            Caption="" CssClass="GridViewStyle" DataKeyNames="ID" 
                            DataSourceID="SqlDSSpedTElenco" EmptyDataText="Nessun dato disponibile." 
                            EnableTheming="True" GridLines="None" TabIndex="7" AllowSorting="True">
                            <AlternatingRowStyle CssClass="AltRowStyle" />
                            <Columns>
                                <asp:CommandField ButtonType="Button" CausesValidation="False" 
                                    ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                                    ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                                    <controlstyle font-size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                                </asp:CommandField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="NumeroSpedizione" 
                                    DataFormatString="{0:d}" HeaderText="N°Spedizione" ReadOnly="True" 
                                    SortExpression="NumeroSpedizione">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="DataSpedizione" 
                                    DataFormatString="{0:d}" HeaderText="Data" ReadOnly="True" 
                                    SortExpression="DataSpedizione">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                                    DataFormatString="{0:d}" HeaderText="N°Ordine" ReadOnly="True" 
                                    SortExpression="Numero">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="IdDocumenti" HeaderText="Id" ItemStyle-Width="1px" ItemStyle-CssClass="nascondi" >
                                    <HeaderStyle CssClass="nascondi" />
                                </asp:BoundField>                             
                                <%--<asp:BoundField DataField="StatoSped" HeaderText="Stato" ItemStyle-CssClass="nascondi"
                                    SortExpression="StatoSped" >
                                    <HeaderStyle CssClass="nascondi" Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>--%>                             
                            </Columns>
                            <HeaderStyle CssClass="HeaderStyle" />
                            <PagerSettings Mode="NextPrevious" Visible="False" />
                            <PagerStyle CssClass="PagerStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
                   </div>    
                </td>
                <td align="left">
                    <div id="divPanelSelezionaPrev" style="height:195px; border-style:groove;">
                    <table >
                    <tr>
                    <td colspan ="4">
                        &nbsp;
                    </td>
                    </tr>                    
                    <tr>
                    <td>
                        <asp:Label ID="LblDescNumSpe" runat="server" Width="130px" Height="17px"  Text="Numero spedizione"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LblNumSped" runat="server" Width="90px" Height="17px" 
                            BorderStyle="Outset" Font-Bold="True"></asp:Label>
                    </td>
                    <td align="right"> 
                        <asp:Label ID="LblDescDataSped" runat="server" Width="130px" Height="17px"  Text="Data" ></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="LblDataSped" runat="server" Width="90px" Height="17px" 
                            BorderStyle="Outset" Font-Bold="True"></asp:Label>
                    </td>
                    </tr>
                    <tr>
                    <td colspan ="4">    
                        <table width="100%" >
                        <tr>
                        <td colspan ="4">
                        Trasporto a mezzo
                        </td>
                        </tr>
                        <tr>
                        <td>
                        <asp:RadioButton ID="optMittente"  ValidationGroup="0" GroupName="GrpTrasporto" runat="server" Text ="Mittente" AutoPostBack ="true"></asp:RadioButton>
                        </td>
                        <td>
                        <asp:RadioButton ID="optDestinatario" ValidationGroup="0" GroupName="GrpTrasporto" runat="server" Text ="Destinatario" AutoPostBack ="true"></asp:RadioButton>
                        </td>
                        <td align="right">
                        <asp:RadioButton ID="optVettore" ValidationGroup="0" GroupName="GrpTrasporto" runat="server" Text ="Vettore" AutoPostBack ="true"></asp:RadioButton>
                        <asp:Button ID="btnGestVett1" runat="server" CausesValidation="False" CommandName="btnGestVett" Visible="false" Text="+" ToolTip="Gestione anagrafica Vettori" />
                        </td>
                        <td>
                            <asp:DropDownList ID="DDLVettore1" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="True" DataSourceID="SqlDSVettori" DataTextField="Descrizione" 
                                DataValueField="Codice" Height="22px" TabIndex="7" Width="200px">
                            <asp:ListItem Text="" Value="" ></asp:ListItem>
                            </asp:DropDownList>                                
                        </td>
                        </tr>
                        <tr>
                            <td colspan ="4">
                            <table>
                            <tr>
                            <td>
                            Numero colli
                            </td>
                            <td>
                            <asp:TextBox  ID="TxtNColli" runat="server" Width="50px" style="text-align:right" 
                                    MaxLength="05" ></asp:TextBox>
                            <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" 
                                                    ControlToValidate="TxtNColli" ErrorMessage="*" 
                                                    ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                            </td>
                            <td align ="right" >
                            + pezzi
                            </td>
                            <td>
                            <asp:TextBox  ID="TxtNPezzi" runat="server" Width="50px" style="text-align:right" 
                                    MaxLength="05"></asp:TextBox>
                            <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                                                    ControlToValidate="TxtNPezzi" ErrorMessage="*" 
                                                    ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                            </td>
                            <td align ="right" >
                            Peso in Kg.
                            </td>                                                                        
                            <td>
                            <asp:TextBox  ID="TxtPesoKG" runat="server" Width="50px" style="text-align:right" 
                                    MaxLength="05"></asp:TextBox>
                            <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                                                    ControlToValidate="TxtPesoKG" ErrorMessage="*" 
                                                    ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                            </td>    
                            </tr>
                            </table>               
                            </td>    
                        </tr>                          
                        </table>                
                    </td>   
                    </tr>
                    <tr>
                        <td colspan ="4">
                            <table align="right"  >
                            <tr>
                            <td>
                                <asp:Button ID="BtnModAgg" runat="server" Height="30px" Text="Modifica" 
                                    Width="170px" />                            
                            </td>
                            <td>
                                <asp:Button ID="BtnAnnDatiSped" runat="server" Height="30px" Text="Annulla" 
                                    Width="108px" />                            
                            </td>
                            </tr>
                            </table>
                        </td>
                    </tr> 
                    <tr>
                        <td colspan ="4">
                            <table align="right"  >
                                <asp:Label ID="lblMessVettore1" runat="server" Text="Nota il Vettore è propositivo" ForeColor="Blue" Visible="false"></asp:Label>
                            </table>
                        </td>
                    </tr>                   
                    </table>    
                     </div>             
                </td>
                </tr>
                </table>
                </td>
                <td align="left" class="style7">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnNuovo" runat="server" class="btnstyle" Text="Nuovo" Visible ="False"  />
                            </div>
                            
                            <div style="height: 5px">&nbsp;</div> 
                            
                            <div>
                                <asp:Button ID="btnModifica" runat="server" class="btnstyle" Text="Modifica Ordine" />
                            </div>
                            
                            <div style="height: 5px">&nbsp;</div> 
                            
                            <div>
                                <asp:Button ID="btnElimina" runat="server" class="btnstyle" Text="Elimina" Visible ="False"/>
                            </div>
                            
                            <div style="height: 5px">&nbsp;</div> 
                            
                            <div>
                                <asp:Button ID="btnNuovoBC" runat="server" class="btnstyle" Text="Nuovo BC (Neg.)" Visible ="False" />
                            </div>
                            
                            <div style="height: 5px">&nbsp;</div> 
                            
                            <div>
                                <asp:Button ID="btnCreaDDT" runat="server" class="btnstyle" Text="Crea DDT" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        <tr>
            <td align="left" class="style5">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                <div id="divGridViewSpedDett" style="overflow-x:hidden; overflow-y:hidden; width:850px; height:230px; border-style:groove;">
                    <asp:GridView ID="GridViewSpedDett" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato selezionato."  
                        DataKeyNames="IDDocumenti" 
                        EnableTheming="True" GridLines="None" 
                        DataSourceID="SqlDSSpedDByIdSpedizione" 
                        Caption=""><AlternatingRowStyle CssClass="AltRowStyle" />                      
                        <Columns>
                        <%--<asp:CommandField ButtonType="Button" CausesValidation="False" 
                            ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                            ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                            <controlstyle font-size="XX-Small" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                        </asp:CommandField>   --%>                     
                        <asp:BoundField DataField="NumeroOrdine" HeaderText="N°ordine" 
                            SortExpression="NumeroOrdine" ItemStyle-HorizontalAlign="Right">
                            <ItemStyle HorizontalAlign="Right" Width="60px" />
                            </asp:BoundField>      
                        <asp:BoundField DataField="Tipo_Doc" HeaderText="Tipo doc." 
                            SortExpression="Tipo_Doc" ItemStyle-HorizontalAlign="Left">
                            <ItemStyle HorizontalAlign="Left" /><HeaderStyle Wrap="True" /><ItemStyle 
                            Width="60px" />
                            </asp:BoundField>                                                    
                        <asp:BoundField DataField="Localita" HeaderText="Località" 
                                SortExpression="Localita">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="120px" /></asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="120px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="320px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Allestita" HeaderText="Quantità" 
                        SortExpression="Qta_Allestita"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <%--<asp:BoundField DataField="Qta_Impegnata" HeaderText="Quantità impegnata" 
                        SortExpression="Qta_Impegnata"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Residua" HeaderText="Quantità residua" 
                        SortExpression="Qta_Residua"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>--%>
                        </Columns>
                        <HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
                </div>
                <div>
                    <asp:Label ID="lblErrore" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                </div>
                        </ContentTemplate>
            </asp:UpdatePanel>
            </td>
                <td align="left" class="style6">                                        
                    <div style="height: 15px; text-align:center" id="noradio"><b>Stampe</b></div> 
                    <div style="height: 15px">&nbsp;</div> 
                    
                    <div style="height: 15px">
                         <asp:Button ID="btnEtiSing" runat="server" class="btnstyle" Text="Etichetta singola" Visible ="False" />
                    </div>                    
                    <div style="height: 15px">&nbsp;</div> 
                    <div style="height: 15px">
                         <asp:Button ID="btnEtiSped" runat="server" class="btnstyle" Text="Etichette sovracollo" Visible ="False" />
                    </div>                    
                    <div style="height: 15px">&nbsp;</div> 
                    <div>
                        <asp:Button ID="btnPrintBrogl" runat="server" class="btnstyle" Text="Stampa brogliaccio" />
                    </div>                    
                    <div style="height: 15px">&nbsp;</div>  
                    <div>
                        <a ID="LnkStampaSing" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Lista di carico" style="border-color:snow;border-style:outset;background-color:yellow;">Apri Lista di carico</a>
                    </div>              
                    <div style="height: 15px">&nbsp;</div>   
                    <div>
                        <asp:Button ID="btnPrintBroglALL" runat="server" class="btnstyle" Text="Stampa TUTTI" />
                    </div>                    
                    <div style="height: 15px">&nbsp;</div>  
                    <div>
                        <a ID="LnkStampaAll" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri TUTTE le lista di carico" style="border-color:snow;border-style:outset;background-color:yellow;">Apri TUTTE le lista di carico</a>
                    </div>               
                    <div style="height: 15px">&nbsp;</div>    
                    <div>
                        <asp:Button ID="btnChiudiAll" runat="server" class="btnstyle" Text="Chiudi allestimento" Visible ="False" />
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
    //TESTATA
//    $('#<%=GridViewSped.ClientID %>').Scrollable({
//            ScrollHeight: 175,        
//        });
    //DETTAGLIO
    var options2 = new GridViewScrollOptions();
    options2.elementID = "<%=GridViewSpedDett.ClientID %>";
    options2.width = 850;
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