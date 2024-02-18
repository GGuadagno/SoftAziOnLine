<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_FatturaOC.ascx.vb" Inherits="SoftAziOnLine.WUC_FatturaOC" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr> 
        <td >
            <asp:SqlDataSource ID="SqlDSDocT" runat="server" 
                            SelectCommand="get_DocTCliByID" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="0" Name="IDDocumenti" SessionField="IDDocumenti" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
            <asp:Panel ID="Panel1" runat="server" Height="100px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridViewDocT" runat="server" AutoGenerateColumns="False" 
                            CssClass="GridViewStyle" 
                            EmptyDataText="Nessun dato disponibile."  
                            DataKeyNames="IDDocumenti" 
                            DataSourceID="SqlDSDocT" EnableTheming="True" GridLines="None">
                            <AlternatingRowStyle CssClass="AltRowStyle" />
                            <Columns>
                                <asp:BoundField DataField="DesStatoDoc" HeaderText="Stato" 
                                SortExpression="DesStatoDoc"><HeaderStyle Wrap="True" /><ItemStyle 
                                Width="15px" /></asp:BoundField> 
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                                    DataFormatString="{0:d}" HeaderText="Numero" ReadOnly="True" 
                                    SortExpression="Numero">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="RevisioneNDoc" 
                                    DataFormatString="{0:d}" HeaderText="Rev.N°" ReadOnly="True" 
                                    SortExpression="RevisioneNDoc">
                                    <HeaderStyle Wrap="True" />
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
                                    <HeaderStyle Wrap="False" />
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
                            </Columns>
                            <HeaderStyle CssClass="HeaderStyle" />
                            <PagerSettings Mode="NextPrevious" Visible="False" />
                            <PagerStyle CssClass="PagerStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td >
            <asp:SqlDataSource ID="SqlDSDocD" runat="server" 
                SelectCommand="Get_DocDForByID" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="0" Name="IDDocumenti" SessionField="IDDocumenti" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" Height="300px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridViewDocD" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti" DataSourceID="SqlDSDocD">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:TemplateField HeaderStyle-Width="5px">
                            <ItemTemplate>
                                <asp:CheckBox id="checkSel" AutoPostBack="false" Checked="false" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-Width="10px" HeaderText="Quantità da fatturare">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtaEv" runat="server" AutoPostBack="false" HeaderText="Quantità" MaxLength="7"
                                               Width="60px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Riga" DataFormatString="{0:d}" HeaderText="Riga" ReadOnly="True" 
                        SortExpression="Riga"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="20px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="False" /><ItemStyle 
                        Width="80px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità ordinata" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Allestita" HeaderText="Quantità inviata" 
                        SortExpression="Qta_Allestita"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Residua" HeaderText="Quantità residua" 
                        SortExpression="Qta_Residua"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <%--<asp:BoundField DataField="Cod_IVA" HeaderText="IVA" 
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
                        Width="05px" /></asp:BoundField>--%>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
</table>
    