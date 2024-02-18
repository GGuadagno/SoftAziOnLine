<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ModificaSchedaIN.ascx.vb" Inherits="SoftAziOnLine.WUC_ModificaSchedaIN" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr> 
        <td >
            <asp:SqlDataSource ID="SqlDSDocT" runat="server" 
                            SelectCommand="get_DocTByIDDocumenti" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="0" Name="IDDocumenti" SessionField="IDDocumenti" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
            <asp:Panel ID="Panel1" runat="server" Height="45px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridViewDocT" runat="server" AutoGenerateColumns="False" 
                            CssClass="GridViewStyle" 
                            EmptyDataText="Nessun dato disponibile."  
                            DataKeyNames="IDDocumenti" 
                            DataSourceID="SqlDSDocT" EnableTheming="True" GridLines="None">
                            <AlternatingRowStyle CssClass="AltRowStyle" />
                            <Columns>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                                    DataFormatString="{0:d}" HeaderText="N° Inventario" ReadOnly="True" 
                                    SortExpression="Numero">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="RevisioneNDoc" 
                                    DataFormatString="{0:d}" HeaderText="N° Pagina" ReadOnly="True" 
                                    SortExpression="RevisioneNDoc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data_Doc" HeaderText="Data inventario" 
                                    SortExpression="Data_Doc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                               <asp:BoundField DataField="InseritoDa" HeaderText="Creata da" 
                                    SortExpression="InseritoDa">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="ModificatoDa" 
                                    HeaderText="Modificata da" ReadOnly="True" 
                                    SortExpression="ModificatoDa">
                                    <HeaderStyle Wrap="false" />
                                    <ItemStyle Width="100px" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderStyle-Width="200px" HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSpazi0" runat="server" Width="60px"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
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
        <td align="left">
            <asp:Panel ID="Panel3" runat="server" Height="25px" >
                <asp:Label ID="lblIntesta" 
                runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                Style="text-align:left" Text="Riga">
                </asp:Label>
                <asp:Label ID="Label2" 
                runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                Style="text-align:left" Text="Codice articolo" Width="100px">
                </asp:Label>
                <asp:Label ID="Label1" 
                runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                Style="text-align:left" Text="Descrizione" Width="330px">
                </asp:Label>
                <asp:Label ID="Label3" 
                runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                Style="text-align:left" Text="" Width="15px">
                </asp:Label>
                <asp:Label ID="Label4" 
                runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                Style="text-align:left" Text="Giacenza">
                </asp:Label>
                <asp:Label ID="Label5" 
                runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                Style="text-align:left" Text="Inventario">
                </asp:Label>
                <asp:Label ID="Label6" 
                runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                Style="text-align:left" Text="" Width="145px">
                </asp:Label>
                <asp:Label ID="Label7" 
                runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                Style="text-align:left" Text="Inventario - IVA Prezzo">
                </asp:Label>
            </asp:Panel> 
        </td>
    </tr>    
    <tr>
        <td >
            <asp:SqlDataSource ID="SqlDSDocD" runat="server" 
                SelectCommand="get_DocDByIDDocumenti" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="0" Name="IDDocumenti" SessionField="IDDocumenti" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" Height="450px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
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
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Riga" DataFormatString="{0:d}" HeaderText="Riga" ReadOnly="True" 
                        SortExpression="Riga"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo" HeaderStyle-Wrap="false"><HeaderStyle Wrap="false" /><ItemStyle 
                        Width="100px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="False" /><ItemStyle 
                        Width="350px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Giacenza" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:TemplateField HeaderStyle-Width="10px" HeaderText="Quantità inventario">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtaEv" runat="server" AutoPostBack="false" HeaderText="Quantità inventario" MaxLength="10"
                                Width="60px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-Width="10px" HeaderText="">
                            <ItemTemplate>
                                <asp:Label ID="lblSpazi" runat="server" Width="150px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità inventario" 
                        SortExpression="Qta_Evasa"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Cod_IVA" HeaderText="IVA" 
                        SortExpression="Cod_IVA"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo" 
                        SortExpression="Prezzo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <%--<asp:BoundField DataField="Qta_Residua" HeaderText="Differenza inventario" 
                        SortExpression="Qta_Residua"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>--%>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
</table>
    