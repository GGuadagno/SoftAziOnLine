<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ProvvigioniFatture.ascx.vb" Inherits="SoftAziOnLine.WUC_ProvvigioniFatture" %>
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
    .style6
    {
        height: 239px;
    }
    </style>    
<br />    
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" 
    Width="980px" Height="550px" CssClass ="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <asp:SqlDataSource ID="SqlDSAgenti" runat="server" 
            SelectCommand="SELECT Codice, Descrizione FROM Agenti ORDER BY Descrizione,Codice">
        </asp:SqlDataSource>
    <table border="0" cellpadding="0" frame="box" style="width:auto; height:550px; margin-right:0;">
            <tr>
                <td align="left" class="style1" colspan="2">
                    <table>
                    <tr>
                        <td align="left" class="style1" colspan="2">
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" 
                                AutoPostBack="True" Width="240px">
                            </asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            &nbsp;<asp:TextBox ID="txtRicerca" runat="server" Width="275px"></asp:TextBox>
                            &nbsp;<asp:Button ID="btnRicerca" runat="server"  
                                Text="Cerca documento" Width="120px" />
                        </td>
                    </tr>
                    </table> 
                </td>
            </tr>
            <tr>
                <td align="left" class="styleBordo">
                    <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="50px">
                        <table width="100%">
                        <tr>
                            <td>
                            
                                <asp:RadioButton ID="rbtnTuttiAgenti" runat="server" AutoPostBack="True" 
                                    GroupName="Agente" Text="Tutti gli agenti" />
                            
                            </td>
                            <td>
                                &nbsp;</td>
                        </tr>
                         <tr>
                            <td>
                                <div>
                                    <asp:RadioButton ID="rbtnAgente" runat="server" AutoPostBack="True" 
                                        GroupName="Agente" Text="Seleziona agente" Visible="true" />
                                </div>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlAgenti" runat="server" AutoPostBack="True" 
                                        DataSourceID="SqlDSAgenti" DataTextField="Descrizione" DataValueField="Codice" 
                                        Width="250px" AppendDataBoundItems="true">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                </td>
                <td align="left" class="style2">
                    <div>
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left" class="style3">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                    <div id="divGridViewPrevT" style="overflow:auto; width:860px; height:195px; border-style:groove;">
                        <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                            SelectCommand="get_DocTElenco" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="FC" Name="TipoDoc" SessionField="TipoDoc" Type="String" />
                                <asp:SessionParameter DefaultValue="999" Name="StatoDoc" SessionField="StatoDoc" Type="Int32" />
                                <asp:SessionParameter DefaultValue="N" Name="SortPrevTEl" SessionField="SortPrevTEl" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    
                    
                    <asp:GridView ID="GridViewPrevT" runat="server" AllowPaging="true" 
                        AutoGenerateColumns="False" BackColor="Silver" CssClass="GridViewStyle" 
                        DataKeyNames="IDDocumenti" DataSourceID="SqlDSPrevTElenco" 
                        EmptyDataText="Nessun dato disponibile." EnableTheming="True" GridLines="None" 
                        PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                        PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                        PagerSettings-Mode="NextPreviousFirstLast" 
                        PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
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
                            <asp:TemplateField InsertVisible="False">
                                <ItemTemplate>
                                    <asp:Button ID="Button1" runat="server" CausesValidation="False" 
                                        CommandName="Select" Text="&gt;" />
                                </ItemTemplate>
                                <controlstyle font-size="XX-Small" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                            </asp:TemplateField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                                DataFormatString="{0:d}" HeaderText="Num. fatt." ReadOnly="True" 
                                SortExpression="Numero">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="10px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Tipo_Doc" HeaderText="Tipo" 
                                SortExpression="Tipo_Doc">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="05px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Data_Doc" HeaderText="Data" 
                                SortExpression="Data_Doc">
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
                                HeaderText="Denominazione" ReadOnly="True" SortExpression="Denominazione">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="15px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                                HeaderText="Località" ReadOnly="True" SortExpression="Localita">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="15px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" HeaderText="CAP" 
                                ReadOnly="True" SortExpression="CAP">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="5px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Partita_IVA" 
                                HeaderText="Partita IVA" ReadOnly="True" SortExpression="Partita_IVA">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="15px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Codice_Fiscale" 
                                HeaderText="Codice Fiscale" ReadOnly="True" SortExpression="Codice_Fiscale">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="15px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                                SortExpression="Riferimento">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="50px" />
                            </asp:BoundField>
                        </Columns>
                        <%--<HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />--%>
                    </asp:GridView>
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                </td>
                <td align="left">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div>
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div>
                            </div>
                            <div style="height: 15px">
                            </div>
                             <div style="height: 15px">
                            </div>
                             <div style="height: 15px">
                            </div>
                             <div style="height: 15px">
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div>
                                <asp:Button ID="btnCambiaAgente" runat="server" class="btnstyle" Text="Cambia agente" Visible="True" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        <tr>
            <td align="left">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                <div id="divGridViewPrevD" style="overflow:auto; width:860px; height:230px; border-style:groove;">
                    <asp:GridView ID="GridViewPrevD" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato selezionato."  
                        DataKeyNames="Riga" 
                        EnableTheming="True" GridLines="None" 
                        DataSourceID="SqlDSPrevDByIDDocumenti"><AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns>
                        <asp:TemplateField InsertVisible="False">
                        <ItemTemplate><asp:Button ID="Button1" runat="server" 
                        CausesValidation="False" CommandName="Select" Text="&gt;" />
                        </ItemTemplate>
                        <controlstyle font-size="XX-Small" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                        </asp:TemplateField>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="30px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="80px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità evasa" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Evasa"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
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
                        <asp:BoundField DataField="Cod_Agente" HeaderText="Cod.Ag." 
                        SortExpression="Cod_Agente"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="DescrizioneAgente" HeaderText="Agente" 
                        SortExpression="DescrizioneAgente"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Pro_Agente" HeaderText="% Provv." 
                        SortExpression="Pro_Agente"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="ImportoProvvigione" HeaderText="Importo Provv." 
                        SortExpression="ImportoProvvigione"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField  Visible="false" DataField="Riga" HeaderText="Riga" 
                        SortExpression="Riga"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        </Columns>
                        <HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
                    <asp:SqlDataSource ID="SqlDSPrevDByIDDocumenti" runat="server" 
                        SelectCommand="get_PrevDByIDDocumentiAgente" 
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
                    <div id="noradio" style="height: 15px">
                    </div>
                    <div style="height: 15px; text-align:center">
                     </div> 
                    <div style="height: 25px">
                    <asp:Label ID="lblProvv" runat="server" Text="% Provvigione"></asp:Label>
                    </div>  
                    <div style="height: 15px">
                        <div style="height: 15px; text-align:center">
                        <asp:TextBox ID="txtProvv" runat="server" Width="50px" MaxLength="5" Enabled="false" >
                        </asp:TextBox>
                        <asp:RangeValidator ID="RangeValidator3" runat="server" ErrorMessage="*" ControlToValidate="txtProvv" Type="Currency"  MaximumValue="99999" MinimumValue="0"></asp:RangeValidator>                       
                     </div> 
                    </div>  
                    <div style="height: 15px">&nbsp;                        
                        <asp:Button ID="btnModifica" runat="server" class="btnstyle" Text="Modifica" Visible="True" Enabled="false"  />                                    
                    </div> 
                    <div style="height: 15px">&nbsp;</div>
                    <div style="height: 15px">&nbsp;</div>
                    <div style="height: 15px">&nbsp;</div> 
                    <div style="height: 15px">                        
                       <asp:Button ID="btnAggiornaProvv" runat="server" class="btnstyle" Text="Aggiorna" Visible="True" Enabled="false"  />
                    </div>
                    <div style="height: 15px">&nbsp;</div>  
                    <div style="height: 15px">&nbsp;</div>
                        <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" Visible="True" Enabled="false"  />                                    
                    <div style="height: 15px">                        
                        
                    </div>
                </td>
        </tr>
</table>
   </ContentTemplate>
 </asp:UpdatePanel>   
</asp:Panel>